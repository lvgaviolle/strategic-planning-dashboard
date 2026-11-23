using StrategicPlanningDashboard.Models;
using StrategicPlanningDashboard.Services;

namespace StrategicPlanningDashboard.Tests;

public sealed class GoalMetricsTests
{
    [Theory]
    [InlineData(100, 120, 120, "Atingida")]
    [InlineData(100, 90, 90, "Em atenção")]
    [InlineData(100, 79, 79, "Abaixo da meta")]
    [InlineData(0, 0, 0, "Abaixo da meta")]
    public void GoalProgress_ReturnsExpectedValues(
        decimal target, decimal actual, decimal expectedProgress, string expectedStatus)
    {
        var goal = new Goal { Target = target, Actual = actual };
        Assert.Equal(expectedProgress, goal.Progress);
        Assert.Equal(expectedStatus, goal.Status);
    }

    [Fact]
    public void MonthlySummary_UsesIndividualAttainmentWithoutSummingPercentages()
    {
        var month = new DateTime(2026, 11, 1);
        var goals = new[]
        {
            new Goal { Target = 100, Actual = 150, Year = 2026, Month = 11 },
            new Goal { Target = 100, Actual = 50, Year = 2026, Month = 11 },
            new Goal { Target = 100, Actual = 100, Year = 2026, Month = 10 }
        };

        var result = GoalMetrics.Summarize(goals, month);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result.Achieved);
        Assert.Equal(1, result.Below);
        Assert.Equal(75m, result.AverageProgress);
    }

    [Fact]
    public void EmptyMonth_ReturnsZeros()
    {
        var result = GoalMetrics.Summarize([], new DateTime(2026, 11, 1));
        Assert.Equal(0, result.Count);
        Assert.Equal(0, result.AverageProgress);
    }
}
