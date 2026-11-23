using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Data.Sqlite;
using StrategicPlanningDashboard.Models;

namespace StrategicPlanningDashboard.Services;

/// <summary>
/// Small SQLite-backed store written specifically for this independent sample.
/// Each method uses a short-lived connection. No company data or external API.
/// </summary>
public sealed class PlanningStore
{
    private readonly string _connectionString;
    private List<Goal> _goals = [];

    public IReadOnlyList<Goal> Goals => _goals;

    public static readonly string[] Areas =
        ["Comercial", "Operações", "Marketing", "Atendimento"];

    public static readonly string[] Branches =
        ["Matriz", "Unidade Norte", "Unidade Sul"];

    public PlanningStore(string databaseFile)
    {
        if (string.IsNullOrWhiteSpace(databaseFile))
            throw new ArgumentException("Informe o caminho do banco.", nameof(databaseFile));

        var fullPath = Path.GetFullPath(databaseFile);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = fullPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Default,
            Pooling = false,
            DefaultTimeout = 10
        }.ToString();

        Initialize();
        Reload();
    }

    public void Add(GoalDraft draft, DateTime? period = null)
    {
        ArgumentNullException.ThrowIfNull(draft);
        Validator.ValidateObject(draft, new ValidationContext(draft), validateAllProperties: true);
        if (!Areas.Contains(draft.Area) || !Branches.Contains(draft.Branch)
            || !new[] { "%", "R$", "un" }.Contains(draft.Unit))
            throw new ValidationException("Área, unidade ou tipo de valor inválido.");

        var when = period ?? DateTime.Today;
        var goal = new Goal
        {
            Title = draft.Title.Trim(),
            Owner = draft.Owner.Trim(),
            Area = draft.Area,
            Branch = draft.Branch,
            Unit = draft.Unit,
            Target = draft.Target,
            Actual = draft.Actual,
            Month = when.Month,
            Year = when.Year
        };

        using var db = Open();
        Insert(db, null, goal);
        Reload();
    }

    public bool SetActual(Guid id, decimal amount)
    {
        if (amount < 0 || amount > 100_000_000m) return false;

        using var db = Open();
        using var command = db.CreateCommand();
        command.CommandText = """
            UPDATE goals SET actual = $actual
            WHERE id = $id AND is_active = 1;
            """;
        command.Parameters.AddWithValue("$actual", amount.ToString(CultureInfo.InvariantCulture));
        command.Parameters.AddWithValue("$id", id.ToString("D"));
        var updated = command.ExecuteNonQuery() > 0;
        if (updated) Reload();
        return updated;
    }

    public bool ToggleActive(Guid id)
    {
        using var db = Open();
        using var command = db.CreateCommand();
        command.CommandText = """
            UPDATE goals SET is_active = CASE is_active WHEN 1 THEN 0 ELSE 1 END
            WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", id.ToString("D"));
        var updated = command.ExecuteNonQuery() > 0;
        if (updated) Reload();
        return updated;
    }

    /// <summary>
    /// Destructively replaces all rows with synthetic sample data.
    /// The UI requires a separate confirmation before calling this method.
    /// </summary>
    public void RestoreDemo()
    {
        using var db = Open();
        using (var transaction = db.BeginTransaction())
        {
            using (var delete = db.CreateCommand())
            {
                delete.Transaction = transaction;
                delete.CommandText = "DELETE FROM goals;";
                delete.ExecuteNonQuery();
            }

            foreach (var goal in BuildSampleGoals())
                Insert(db, transaction, goal);

            transaction.Commit();
        }
        Reload();
    }

    private void Initialize()
    {
        using var db = Open();
        using (var command = db.CreateCommand())
        {
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS goals (
                    id TEXT PRIMARY KEY NOT NULL,
                    title TEXT NOT NULL,
                    area TEXT NOT NULL,
                    branch TEXT NOT NULL,
                    owner TEXT NOT NULL,
                    unit TEXT NOT NULL,
                    target TEXT NOT NULL,
                    actual TEXT NOT NULL,
                    month INTEGER NOT NULL CHECK (month BETWEEN 1 AND 12),
                    year INTEGER NOT NULL,
                    is_active INTEGER NOT NULL DEFAULT 1 CHECK (is_active IN (0, 1))
                );
                CREATE INDEX IF NOT EXISTS ix_goals_period ON goals(year, month, is_active);
                """;
            command.ExecuteNonQuery();
        }

        using var countCommand = db.CreateCommand();
        countCommand.CommandText = "SELECT COUNT(*) FROM goals;";
        var count = (long)(countCommand.ExecuteScalar() ?? 0L);
        if (count == 0) RestoreDemo();
    }

    private SqliteConnection Open()
    {
        var db = new SqliteConnection(_connectionString);
        db.Open();
        using var pragma = db.CreateCommand();
        pragma.CommandText = "PRAGMA busy_timeout = 10000;";
        pragma.ExecuteNonQuery();
        return db;
    }

    private void Reload()
    {
        var loaded = new List<Goal>();
        using var db = Open();
        using var command = db.CreateCommand();
        command.CommandText = """
            SELECT id, title, area, branch, owner, unit,
                   target, actual, month, year, is_active
            FROM goals ORDER BY year DESC, month DESC, title;
            """;
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            loaded.Add(new Goal
            {
                Id = Guid.Parse(reader.GetString(0)),
                Title = reader.GetString(1),
                Area = reader.GetString(2),
                Branch = reader.GetString(3),
                Owner = reader.GetString(4),
                Unit = reader.GetString(5),
                Target = decimal.Parse(reader.GetString(6), CultureInfo.InvariantCulture),
                Actual = decimal.Parse(reader.GetString(7), CultureInfo.InvariantCulture),
                Month = reader.GetInt32(8),
                Year = reader.GetInt32(9),
                IsActive = reader.GetInt32(10) == 1
            });
        }
        _goals = loaded;
    }

    private static void Insert(SqliteConnection db, SqliteTransaction? transaction, Goal goal)
    {
        using var command = db.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO goals
                (id, title, area, branch, owner, unit, target, actual, month, year, is_active)
            VALUES
                ($id, $title, $area, $branch, $owner, $unit,
                 $target, $actual, $month, $year, $active);
            """;
        command.Parameters.AddWithValue("$id", goal.Id.ToString("D"));
        command.Parameters.AddWithValue("$title", goal.Title);
        command.Parameters.AddWithValue("$area", goal.Area);
        command.Parameters.AddWithValue("$branch", goal.Branch);
        command.Parameters.AddWithValue("$owner", goal.Owner);
        command.Parameters.AddWithValue("$unit", goal.Unit);
        command.Parameters.AddWithValue("$target", goal.Target.ToString(CultureInfo.InvariantCulture));
        command.Parameters.AddWithValue("$actual", goal.Actual.ToString(CultureInfo.InvariantCulture));
        command.Parameters.AddWithValue("$month", goal.Month);
        command.Parameters.AddWithValue("$year", goal.Year);
        command.Parameters.AddWithValue("$active", goal.IsActive ? 1 : 0);
        command.ExecuteNonQuery();
    }

    private static IEnumerable<Goal> BuildSampleGoals()
    {
        var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var samples = new (string Title, string Area, string Branch,
            string Owner, string Unit, decimal Target, decimal Actual)[]
        {
            ("Receita de vendas", "Comercial", "Matriz", "Camila", "R$", 250000, 263500),
            ("Novos contratos", "Comercial", "Unidade Norte", "Rafael", "un", 120, 112),
            ("Conversão de oportunidades", "Comercial", "Unidade Sul", "Bianca", "%", 32, 27.8m),
            ("Entregas no prazo", "Operações", "Matriz", "Marcos", "%", 95, 96.2m),
            ("Volume processado", "Operações", "Unidade Norte", "Daniela", "un", 1600, 1310),
            ("Produtividade da equipe", "Operações", "Unidade Sul", "Luiza", "%", 88, 75),
            ("Leads qualificados", "Marketing", "Matriz", "Felipe", "un", 540, 558),
            ("Alcance de campanhas", "Marketing", "Unidade Sul", "Joana", "un", 10000, 8200),
            ("Satisfação do atendimento", "Atendimento", "Matriz", "Renata", "%", 92, 94),
            ("Resoluções no primeiro contato", "Atendimento", "Unidade Norte", "André", "%", 85, 66)
        };

        foreach (var sample in samples)
            yield return MakeGoal(sample.Title, sample.Area, sample.Branch, sample.Owner,
                sample.Unit, sample.Target, sample.Actual, month);

        // Five preceding months, intentionally varied to showcase historical charts.
        // Not real business performance or data from any employer.
        var factors = new decimal[] { 0.91m, 0.86m, 0.94m, 0.82m, 0.88m };
        for (var offset = 1; offset <= 5; offset++)
        {
            var period = month.AddMonths(-offset);
            for (var index = 0; index < samples.Length; index++)
            {
                var s = samples[index];
                var variance = index % 3 == 0 ? 0.08m : (index % 3 == 1 ? -0.04m : 0);
                var value = Math.Round(s.Target * (factors[offset - 1] + variance), 2);
                yield return MakeGoal(s.Title, s.Area, s.Branch, s.Owner,
                    s.Unit, s.Target, value, period);
            }
        }
    }

    private static Goal MakeGoal(string title, string area, string branch, string owner,
        string unit, decimal target, decimal actual, DateTime period) => new()
    {
        Title = title,
        Area = area,
        Branch = branch,
        Owner = owner,
        Unit = unit,
        Target = target,
        Actual = actual,
        Month = period.Month,
        Year = period.Year
    };
}
