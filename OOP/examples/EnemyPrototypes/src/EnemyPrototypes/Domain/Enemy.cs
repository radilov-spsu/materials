namespace EnemyPrototypes.Domain;

/// <summary>
/// ЭТАП 0. Каждый вид врага — отдельный класс. Посмотрите внимательно:
/// отличаются ли они ПОВЕДЕНИЕМ или только значениями полей?
///
/// Задание этапа 0 — НЕ переписывать сразу, а сначала разобрать:
///   1) сколько здесь классов и чем именно они отличаются;
///   2) сколько классов появится ещё при четырёх вариантах скелета
///      и трёх вариантах упыря;
///   3) почему фабричный метод из лабораторной 1 эту проблему не решает.
/// </summary>
public abstract class Enemy
{
    public string Name { get; set; } = "";
    public int Health { get; set; }
    public int Damage { get; set; }
    public Equipment Equipment { get; set; } = new();
    public List<string> Loot { get; set; } = [];

    public override string ToString() =>
        $"{Name}: HP {Health}, урон {Damage} | {Equipment} | лут: " +
        (Loot.Count == 0 ? "нет" : string.Join(", ", Loot));
}

public sealed class Skeleton : Enemy
{
    public Skeleton()
    {
        Name = "Скелет";
        Health = 60;
        Damage = 8;
        Equipment = new Equipment { WeaponName = "Ржавый меч", ArmorRating = 5 };
        Loot = ["Кость"];
    }
}

public sealed class ArmoredSkeleton : Enemy
{
    public ArmoredSkeleton()
    {
        Name = "Скелет в доспехах";
        Health = 90;
        Damage = 8;
        Equipment = new Equipment
        {
            WeaponName = "Ржавый меч",
            ArmorRating = 25,
            Modifiers = [new Modifier { Title = "Прочность", Bonus = 10 }]
        };
        Loot = ["Кость", "Обломок доспеха"];
    }
}

public sealed class SkeletonArcher : Enemy
{
    public SkeletonArcher()
    {
        Name = "Скелет-лучник";
        Health = 50;
        Damage = 12;
        Equipment = new Equipment { WeaponName = "Кривой лук", ArmorRating = 2 };
        Loot = ["Кость", "Стрела"];
    }
}

public sealed class CryptGuardian : Enemy
{
    public CryptGuardian()
    {
        Name = "Страж склепа";
        Health = 120;
        Damage = 14;
        Equipment = new Equipment
        {
            WeaponName = "Тяжёлая секира",
            ArmorRating = 30,
            Modifiers = [new Modifier { Title = "Прочность", Bonus = 15 },
                         new Modifier { Title = "Устрашение", Bonus = 5 }]
        };
        Loot = ["Кость", "Ключ от склепа"];
    }
}

public sealed class Ghoul : Enemy
{
    public Ghoul()
    {
        Name = "Упырь";
        Health = 75;
        Damage = 11;
        Equipment = new Equipment { WeaponName = "Когти", ArmorRating = 8 };
        Loot = ["Гнилая плоть"];
    }
}
