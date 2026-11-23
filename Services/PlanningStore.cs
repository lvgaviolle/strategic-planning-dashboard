using StrategicPlanningDashboard.Models;

namespace StrategicPlanningDashboard.Services;

/// <summary>
/// Session-scoped sample data. No database, credentials or external services.
/// This is a fresh, independent demonstration implementation.
/// </summary>
public sealed class PlanningStore
{
    private readonly List<Goal> _goals = [];
    public IReadOnlyList<Goal> Goals => _goals;

    public static readonly string[] Areas =
        ["Comercial", "Operações", "Marketing", "Atendimento"];

    public static readonly string[] Branches =
        ["Matriz", "Unidade Norte", "Unidade Sul"];

    public PlanningStore() => RestoreDemo();

    public void Add(GoalDraft draft)
    {
        var today = DateTime.Today;
        _goals.Insert(0, new Goal
        {
            Title = draft.Title.Trim(),
            Owner = draft.Owner.Trim(),
            Area = draft.Area,
            Branch = draft.Branch,
            Unit = draft.Unit,
            Target = draft.Target,
            Actual = draft.Actual,
            Month = today.Month,
            Year = today.Year
        });
    }

    public bool SetActual(Guid id, decimal amount)
    {
        if (amount < 0) return false;
        var goal = _goals.FirstOrDefault(x => x.Id == id);
        if (goal is null) return false;
        goal.Actual = amount;
        return true;
    }

    public void ToggleActive(Guid id)
    {
        var goal = _goals.FirstOrDefault(x => x.Id == id);
        if (goal is not null) goal.IsActive = !goal.IsActive;
    }

    public void RestoreDemo()
    {
        _goals.Clear();
        var today = DateTime.Today;
        Seed("Receita de vendas", "Comercial", "Matriz", "Camila", "R$", 250000, 263500, today);
        Seed("Novos contratos", "Comercial", "Unidade Norte", "Rafael", "un", 120, 112, today);
        Seed("Conversão de oportunidades", "Comercial", "Unidade Sul", "Bianca", "%", 32, 27.8m, today);
        Seed("Entregas no prazo", "Operações", "Matriz", "Marcos", "%", 95, 96.2m, today);
        Seed("Volume processado", "Operações", "Unidade Norte", "Daniela", "un", 1600, 1310, today);
        Seed("Produtividade da equipe", "Operações", "Unidade Sul", "Luiza", "%", 88, 75, today);
        Seed("Leads qualificados", "Marketing", "Matriz", "Felipe", "un", 540, 558, today);
        Seed("Alcance de campanhas", "Marketing", "Unidade Sul", "Joana", "un", 10000, 8200, today);
        Seed("Satisfação do atendimento", "Atendimento", "Matriz", "Renata", "%", 92, 94, today);
        Seed("Resoluções no primeiro contato", "Atendimento", "Unidade Norte", "André", "%", 85, 66, today);

        var previousMonth = today.AddMonths(-1);
        Seed("Receita de vendas", "Comercial", "Matriz", "Camila", "R$", 240000, 245000, previousMonth);
        Seed("Entregas no prazo", "Operações", "Matriz", "Marcos", "%", 95, 90, previousMonth);
    }

    private void Seed(string title, string area, string branch, string owner,
        string unit, decimal target, decimal actual, DateTime period)
    {
        _goals.Add(new Goal
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
        });
    }
}
