namespace GameFactories.Domain;

/// <summary>Базовый враг. Менять этот файл можно и нужно.</summary>
public abstract class Enemy
{
    public abstract string Name { get; }
    public int Health { get; protected set; } = 100;

    public Weapon? Weapon { get; set; }
    public Loot? Loot { get; set; }

    public void Spawn() =>
        Console.WriteLine($"  {Name} появляется " +
                          $"(оружие: {Weapon?.Title ?? "нет"}, лут: {Loot?.Title ?? "нет"})");

    public void Attack() =>
        Console.WriteLine($"  {Name} атакует на {Weapon?.Damage ?? 1} урона");
}

public sealed class Wolf : Enemy
{
    public override string Name => "Волк";
}

public sealed class Bandit : Enemy
{
    public override string Name => "Разбойник";
}

public sealed class SandWorm : Enemy
{
    public override string Name => "Песчаный червь";
}

public sealed class Nomad : Enemy
{
    public override string Name => "Кочевник";
}
