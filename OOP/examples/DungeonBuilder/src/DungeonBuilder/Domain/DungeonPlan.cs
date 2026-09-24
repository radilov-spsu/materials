namespace DungeonBuilder.Domain;

/// <summary>План подземелья: входные данные генератора. Менять можно.</summary>
public sealed class DungeonPlan
{
    public required string Title { get; init; }
    public required IReadOnlyList<Room> Rooms { get; init; }
    public required IReadOnlyList<Corridor> Corridors { get; init; }

    /// <summary>Небольшой план для демонстрации и тестов.</summary>
    public static DungeonPlan Sample() => new()
    {
        Title = "Забытый склеп",
        Rooms =
        [
            new Room { Id = 1, Width = 6, Height = 4, Enemies = ["Скелет", "Скелет"], HasChest = true },
            new Room { Id = 2, Width = 8, Height = 5, Enemies = ["Упырь"], HasChest = false },
            new Room { Id = 3, Width = 4, Height = 4, Enemies = [], HasChest = true }
        ],
        Corridors =
        [
            new Corridor { From = 1, To = 2, Length = 12 },
            new Corridor { From = 2, To = 3, Length = 7 }
        ]
    };
}

public sealed class Room
{
    public required int Id { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required IReadOnlyList<string> Enemies { get; init; }
    public required bool HasChest { get; init; }
}

public sealed class Corridor
{
    public required int From { get; init; }
    public required int To { get; init; }
    public required int Length { get; init; }
}
