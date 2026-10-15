using GameSingleton.Domain;

namespace GameSingleton;

/// <summary>
/// ЭТАП 2. Потребитель конфигурации. Посмотрите на конструктор и сигнатуры:
/// можно ли по ним понять, что этот класс зависит от настроек игры и от журнала?
/// </summary>
public class WaveSpawner
{
    public int SpawnPower(int baseCount)
    {
        // зависимость спрятана внутри метода — снаружи её не видно
        var multiplier = GameConfig.Instance.DifficultyMultiplier;
        EventLog.Instance.Write($"Спавн волны: {baseCount} x{multiplier}");

        return baseCount * multiplier;
    }
}
