using StrategicPlanningDashboard.Models;

namespace StrategicPlanningDashboard.Services;

/// <summary>
/// Aggregates normalized attainment only; never sums percentages, currencies
/// or quantities across unrelated indicators.
/// </summary>
public static class GoalMetrics
{
    public static PeriodSummary Summarize(IEnumerable<Goal> goals, DateTime period)
    {
        var items = goals.Where(g => g.IsActive && g.Year == period.Year && g.Month == period.Month).ToList();
        var achieved = items.Count(g => g.Progress >= 100);
        var attention = items.Count(g => g.Progress >= 80 && g.Progress < 100);
        var below = items.Count(g => g.Progress < 80);
        // Each indicator has the same weight and is capped at 100% in the aggregate.
        var average = items.Count == 0
            ? 0
            : Math.Round(items.Average(g => Math.Min(100m, g.Progress)), 1);

        return new PeriodSummary(period, items.Count, achieved, attention, below, average);
    }
}

public sealed record PeriodSummary(
    DateTime Period,
    int Count,
    int Achieved,
    int Attention,
    int Below,
    decimal AverageProgress);
