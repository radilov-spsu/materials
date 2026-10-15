namespace EnemyPrototypes.Domain;

/// <summary>
/// Вложенный уровень для этапа 2: у врага есть экипировка,
/// у экипировки — список модификаторов. Клонироваться должны оба уровня.
/// </summary>
public sealed class Equipment
{
    public string WeaponName { get; set; } = "Кулаки";
    public int ArmorRating { get; set; }
    public List<Modifier> Modifiers { get; set; } = [];

    public override string ToString() =>
        $"{WeaponName}, броня {ArmorRating}, модификаторы: " +
        (Modifiers.Count == 0 ? "нет" : string.Join(", ", Modifiers));
}

public sealed class Modifier
{
    public string Title { get; set; } = "";
    public int Bonus { get; set; }

    public override string ToString() => $"{Title}+{Bonus}";
}
