using GameFactories;
using GameFactories.Domain;
using Xunit;

namespace GameFactories.Tests;

public class WaveSpawnerTests
{
    [Fact]
    public void Волна_содержит_запрошенное_количество_врагов()
    {
        var spawner = new WaveSpawner();

        var wave = spawner.Spawn("forest", 5);

        Assert.Equal(5, wave.Count);
    }

    [Fact]
    public void Неизвестная_локация_приводит_к_ошибке()
    {
        var spawner = new WaveSpawner();

        Assert.Throws<ArgumentOutOfRangeException>(() => spawner.Spawn("atlantis", 1));
    }

    // TODO этап 1: тест с подменённой фабрикой-заглушкой
    // TODO этап 2: тест состава волны для каждой локации
    // TODO этап 3: тест согласованности семейства — враг и оружие из одного мира
}
