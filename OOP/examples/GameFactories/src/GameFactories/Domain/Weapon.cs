namespace GameFactories.Domain;

public abstract class Weapon
{
    public abstract string Title { get; }
    public abstract int Damage { get; }
}

public sealed class RustySword : Weapon
{
    public override string Title => "Ржавый меч";
    public override int Damage => 7;
}

public sealed class CurvedDagger : Weapon
{
    public override string Title => "Кривой кинжал";
    public override int Damage => 5;
}
