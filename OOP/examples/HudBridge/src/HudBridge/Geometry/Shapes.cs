namespace HudBridge.Geometry;

/// <summary>Координаты. Пригодятся, когда будете проектировать примитивы.</summary>
public readonly record struct Point(int X, int Y);

public readonly record struct Rect(int X, int Y, int Width, int Height)
{
    public Point TopLeft => new(X, Y);
    public Point BottomRight => new(X + Width, Y + Height);
}
