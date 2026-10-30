namespace SaveAdapters.Domain;

/// <summary>Модель сохранения — то, чем оперирует игра.</summary>
public sealed class SaveGame
{
    public required string Name { get; init; }
    public required string PlayerName { get; init; }
    public required int Level { get; init; }
    public required int Health { get; init; }
    public DateTime SavedAt { get; init; } = DateTime.UtcNow;

    public override string ToString() =>
        $"{Name}: {PlayerName}, уровень {Level}, HP {Health}";
}
