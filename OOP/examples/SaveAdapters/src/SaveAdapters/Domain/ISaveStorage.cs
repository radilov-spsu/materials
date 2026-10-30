namespace SaveAdapters.Domain;

/// <summary>
/// Контракт игры. К нему нужно привести обе чужие системы.
/// Весь игровой код должен зависеть только от него.
/// </summary>
public interface ISaveStorage
{
    Task SaveAsync(SaveGame save, CancellationToken ct = default);
    Task<SaveGame?> LoadAsync(string name, CancellationToken ct = default);
    Task<IReadOnlyList<string>> ListAsync(CancellationToken ct = default);
}
