namespace GameFactories.Domain;

public abstract class Loot
{
    public abstract string Title { get; }
}

public sealed class Herb : Loot
{
    public override string Title => "Лесная трава";
}

public sealed class CactusJuice : Loot
{
    public override string Title => "Сок кактуса";
}
