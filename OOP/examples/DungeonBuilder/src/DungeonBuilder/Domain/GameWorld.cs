namespace DungeonBuilder.Domain;

/// <summary>
/// Продукт для WorldBuilder (этап 1): объекты игрового мира.
/// Заготовка — дополняйте под свой вариант.
/// </summary>
public sealed class GameWorld
{
    private readonly List<WorldObject> _objects = [];

    public string Title { get; set; } = "";
    public IReadOnlyList<WorldObject> Objects => _objects;

    public void Add(WorldObject obj) => _objects.Add(obj);

    public override string ToString() =>
        $"Мир «{Title}»: объектов {_objects.Count}";
}

public sealed record WorldObject(string Kind, int RoomId, string Name);
