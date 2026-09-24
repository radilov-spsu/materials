namespace DungeonBuilder.Domain;

/// <summary>
/// ЭТАП 3. Сейчас это обычный изменяемый класс с конструктором на восемь параметров.
/// Ваша задача — сделать продукт неизменяемым и собирать его текучим строителем,
/// а инварианты проверять в Build().
/// </summary>
public class Item
{
    public Item(string title, Rarity rarity, int damage, double weight,
                bool isTwoHanded, int requiredLevel, int price, string? description)
    {
        Title = title;
        Rarity = rarity;
        Damage = damage;
        Weight = weight;
        IsTwoHanded = isTwoHanded;
        RequiredLevel = requiredLevel;
        Price = price;
        Description = description;
    }

    public string Title { get; set; }
    public Rarity Rarity { get; set; }
    public int Damage { get; set; }
    public double Weight { get; set; }
    public bool IsTwoHanded { get; set; }
    public int RequiredLevel { get; set; }
    public int Price { get; set; }
    public string? Description { get; set; }
    public List<Effect> Effects { get; } = [];

    public override string ToString() =>
        $"{Title} [{Rarity}] урон {Damage}, вес {Weight}, эффекты: " +
        (Effects.Count == 0 ? "нет" : string.Join(", ", Effects));
}

public enum Rarity { Common, Rare, Epic, Legendary }

public enum Effect { Burning, Freezing, Lifesteal, Poison }
