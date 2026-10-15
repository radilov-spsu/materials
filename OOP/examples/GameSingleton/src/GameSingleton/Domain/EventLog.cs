namespace GameSingleton.Domain;

/// <summary>
/// Вторая «единственная» сущность. Пока тоже одиночка, причём с вводом-выводом
/// в конструкторе — обратите внимание, во что это превращает тесты потребителей.
/// </summary>
public class EventLog
{
    private static EventLog? _instance;
    public static EventLog Instance => _instance ??= new EventLog();

    private readonly List<string> _entries = [];

    public IReadOnlyList<string> Entries => _entries;

    public void Write(string message)
    {
        _entries.Add(message);
        Console.WriteLine($"[log] {message}");
    }

    private EventLog() { }
}
