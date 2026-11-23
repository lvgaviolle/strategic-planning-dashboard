using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace StrategicPlanningDashboard.Models;

public sealed class Goal
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public string Area { get; set; } = "";
    public string Branch { get; set; } = "";
    public string Owner { get; set; } = "";
    public string Unit { get; set; } = "%";
    public decimal Target { get; set; }
    public decimal Actual { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public bool IsActive { get; set; } = true;

    // All demo metrics are "higher is better".
    public decimal Progress => Target <= 0
        ? 0
        : Math.Round(Actual / Target * 100, 1);

    public string Status => Progress >= 100 ? "Atingida"
        : Progress >= 80 ? "Em atenção"
        : "Abaixo da meta";

    public string StatusClass => Progress >= 100 ? "success"
        : Progress >= 80 ? "warning"
        : "danger";

    public string Format(decimal value)
    {
        var culture = CultureInfo.GetCultureInfo("pt-BR");
        return Unit switch
        {
            "R$" => value.ToString("C0", culture),
            "%" => value.ToString("N1", culture) + "%",
            _ => value.ToString("N0", culture)
        };
    }
}

public sealed class GoalDraft
{
    [Required(ErrorMessage = "Informe um título.")]
    [StringLength(90, MinimumLength = 3, ErrorMessage = "Use entre 3 e 90 caracteres.")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Informe um responsável.")]
    public string Owner { get; set; } = "";

    public string Area { get; set; } = "Comercial";
    public string Branch { get; set; } = "Matriz";
    public string Unit { get; set; } = "%";

    [Range(0.01, 100000000, ErrorMessage = "A meta deve ser maior que zero.")]
    public decimal Target { get; set; } = 100;

    [Range(0, 100000000, ErrorMessage = "O realizado não pode ser negativo.")]
    public decimal Actual { get; set; } = 0;
}
