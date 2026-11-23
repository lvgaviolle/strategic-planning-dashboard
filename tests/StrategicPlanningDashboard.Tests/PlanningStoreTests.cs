using StrategicPlanningDashboard.Models;
using StrategicPlanningDashboard.Services;

namespace StrategicPlanningDashboard.Tests;

public sealed class PlanningStoreTests
{
    [Fact]
    public void GoalsPersistAcrossStoreInstances()
    {
        WithDatabase(path =>
        {
            var first = new PlanningStore(path);
            var goal = first.Goals.First(x => x.Month == DateTime.Today.Month
                && x.Year == DateTime.Today.Year && x.IsActive);
            Assert.True(first.SetActual(goal.Id, 123.45m));

            var second = new PlanningStore(path);
            Assert.Equal(123.45m, second.Goals.Single(x => x.Id == goal.Id).Actual);
        });
    }

    [Fact]
    public void NewGoalIsSavedToSelectedMonth()
    {
        WithDatabase(path =>
        {
            var store = new PlanningStore(path);
            var period = new DateTime(2025, 5, 1);
            store.Add(new GoalDraft
            {
                Title = "Indicador de teste",
                Owner = "Usuário fictício",
                Area = "Marketing",
                Branch = "Matriz",
                Unit = "un",
                Target = 100,
                Actual = 25
            }, period);

            var saved = new PlanningStore(path).Goals.Single(g => g.Title == "Indicador de teste");
            Assert.Equal(2025, saved.Year);
            Assert.Equal(5, saved.Month);
            Assert.Equal(25m, saved.Actual);
        });
    }

    [Fact]
    public void InactiveGoalCannotHaveItsActualChanged()
    {
        WithDatabase(path =>
        {
            var store = new PlanningStore(path);
            var goal = store.Goals.First();
            Assert.True(store.ToggleActive(goal.Id));
            Assert.False(store.SetActual(goal.Id, 999));
            Assert.False(store.Goals.Single(g => g.Id == goal.Id).IsActive);
        });
    }

    [Fact]
    public void ResetReplacesUserEntriesWithSyntheticData()
    {
        WithDatabase(path =>
        {
            var store = new PlanningStore(path);
            store.Add(new GoalDraft { Title = "Meta personalizada", Owner = "Teste" });
            Assert.Contains(store.Goals, x => x.Title == "Meta personalizada");
            store.RestoreDemo();
            Assert.DoesNotContain(store.Goals, x => x.Title == "Meta personalizada");
        });
    }

    private static void WithDatabase(Action<string> test)
    {
        var directory = Path.Combine(Path.GetTempPath(), "northstar-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try { test(Path.Combine(directory, "demo.db")); }
        finally { Directory.Delete(directory, recursive: true); }
    }
}
