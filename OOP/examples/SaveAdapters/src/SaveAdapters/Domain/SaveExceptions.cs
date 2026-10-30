namespace SaveAdapters.Domain;

/// <summary>
/// Исключения предметной области. Коды возврата чужих систем
/// адаптеры должны превращать именно в них.
/// </summary>
public class SaveStorageException : Exception
{
    public SaveStorageException(string message) : base(message) { }
}

public sealed class StorageFullException : SaveStorageException
{
    public StorageFullException() : base("В хранилище нет места") { }
}

public sealed class SaveLockedException : SaveStorageException
{
    public SaveLockedException(string name) : base($"Сохранение «{name}» заблокировано") { }
}

public sealed class SaveNotFoundException : SaveStorageException
{
    public SaveNotFoundException(string name) : base($"Сохранение «{name}» не найдено") { }
}
