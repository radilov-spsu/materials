using System.Collections.Concurrent;
using GameSingleton;
using GameSingleton.Domain;
using Xunit;

namespace GameSingleton.Tests;

public class GameConfigTests
{
    [Fact]
    public void Последовательное_обращение_даёт_один_экземпляр()
    {
        var first = GameConfig.Instance;
        var second = GameConfig.Instance;

        Assert.Same(first, second);     // в один поток наивный одиночка работает
    }

    // TODO этап 0: ПАДАЮЩИЙ тест — 50 потоков одновременно читают Instance.
    //              Заготовка ниже: раскомментируйте, запустите, зафиксируйте падение
    //              отдельным коммитом, и только потом переходите к этапу 1.
    //
    // [Fact]
    // public void Параллельное_обращение_тоже_даёт_один_экземпляр()
    // {
    //     var ids = new ConcurrentBag<Guid>();
    //
    //     Parallel.For(0, 50, _ => ids.Add(GameConfig.Instance.Id));
    //
    //     Assert.Single(ids.Distinct());
    // }

    // TODO этап 2: тесты влияют друг на друга (результат зависит от порядка)
    // TODO этап 2: зависимость не видна в сигнатуре WaveSpawner
    // TODO этап 2: подменить конфигурацию в тесте нечем
    // TODO этап 3: те же проверки на версии с контейнером — теперь независимы
    // TODO этап 3: контейнер отдаёт один и тот же экземпляр
}
