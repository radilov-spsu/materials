namespace GameSingleton.Domain;

/// <summary>
/// ЭТАП 0. Наивный одиночка «как в интернете».
///
/// Задание этапа 0:
///   1) написать тест: 50 потоков одновременно читают Instance,
///      собрать все полученные Id в множество — ожидается один уникальный;
///   2) зафиксировать ПАДЕНИЕ теста отдельным коммитом;
///   3) объяснить в отчёте, почему число экземпляров нестабильно от запуска к запуску.
///
/// Thread.Sleep ниже стоит намеренно: он расширяет окно гонки.
/// Без него баг проявляется редко — и именно поэтому доживает до продакшена.
/// </summary>
public class GameConfig
{
    private static GameConfig? _instance;

    public static GameConfig Instance
    {
        get
        {
            if (_instance is null)          // два потока могут войти сюда одновременно
            {
                Thread.Sleep(10);           // имитация долгой загрузки настроек
                _instance = new GameConfig();
            }

            return _instance;
        }
    }

    /// <summary>Чтобы различать экземпляры в тестах.</summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>Изменяемое состояние — понадобится на этапе 2.</summary>
    public int DifficultyMultiplier { get; set; } = 1;

    public string Language { get; set; } = "ru";

    private GameConfig() { }
}
