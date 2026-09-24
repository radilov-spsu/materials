using DungeonBuilder;
using DungeonBuilder.Domain;
using Xunit;

namespace DungeonBuilder.Tests;

public class DungeonGeneratorTests
{
    [Fact]
    public void Карта_содержит_название_подземелья()
    {
        var map = new DungeonGenerator().Generate(DungeonPlan.Sample());

        Assert.Contains("Забытый склеп", map);
    }

    [Fact]
    public void Карта_содержит_строку_на_каждую_комнату()
    {
        var plan = DungeonPlan.Sample();

        var map = new DungeonGenerator().Generate(plan);

        foreach (var room in plan.Rooms)
            Assert.Contains($"[{room.Id}] комната", map);
    }

    // TODO этап 1: два строителя на одном проходе дают согласованные результаты
    // TODO этап 2: третий строитель считает комнаты, врагов и сундуки
    // TODO этап 3: успешная сборка предмета
    // TODO этап 3: нарушенный инвариант приводит к исключению
    // TODO этап 3: повторный Build() запрещён
}
