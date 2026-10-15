using EnemyPrototypes;
using EnemyPrototypes.Domain;
using Xunit;

namespace EnemyPrototypes.Tests;

public class EnemyTests
{
    [Fact]
    public void Волна_содержит_запрошенное_количество()
    {
        var wave = new WaveSpawner().Spawn("skeleton", 3);

        Assert.Equal(3, wave.Count);
    }

    [Fact]
    public void Неизвестный_вид_приводит_к_ошибке()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WaveSpawner().Spawn("dragon", 1));
    }

    // TODO этап 1: клон — другой объект, но с теми же значениями
    // TODO этап 1: правка клона не меняет образец в реестре
    // TODO этап 2: ПАДАЮЩИЙ тест — добавили лут одному клону, у второго он не появился
    // TODO этап 2: вложенный уровень (экипировка → модификаторы) тоже клонируется
    // TODO этап 3: неизвестный ключ в реестре даёт внятное исключение
}
