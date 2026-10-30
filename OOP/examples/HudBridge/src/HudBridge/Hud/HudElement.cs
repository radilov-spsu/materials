using System.Text;
using HudBridge.Geometry;

namespace HudBridge.Hud;

/// <summary>
/// ЭТАП 0. Интерфейс игры сделан наследованием по ДВУМ осям:
/// что рисуем (полоска, миникарта, диалог) × чем рисуем (консоль, SVG).
/// Три элемента и два способа дают шесть классов.
///
/// Задание этапа 0 — НЕ переписывать сразу, а сначала разобрать:
///   1) записать формулу: N элементов × M способов = сколько классов;
///   2) заполнить таблицу роста при добавлении элемента / способа / обоих;
///   3) сравнить ConsoleHealthBar.Render() и SvgHealthBar.Render():
///      что в них одинаково, а что различается. Это и есть будущая граница
///      между абстракцией и реализацией.
/// </summary>
public abstract class HudElement
{
    public abstract string Render();
}

// ── Полоска здоровья ────────────────────────────────────────────────────

public sealed class ConsoleHealthBar : HudElement
{
    public int Current { get; set; }
    public int Max { get; set; } = 100;
    public int Width { get; set; } = 20;

    public override string Render()
    {
        // РАСЧЁТ: сколько клеток закрашено
        var ratio = Max == 0 ? 0 : (double)Current / Max;
        var filled = (int)Math.Round(ratio * Width);

        // РИСОВАНИЕ: псевдографика
        var bar = new string('#', filled) + new string('.', Width - filled);
        return $"HP [{bar}] {Current}/{Max}";
    }
}

public sealed class SvgHealthBar : HudElement
{
    public int Current { get; set; }
    public int Max { get; set; } = 100;
    public int Width { get; set; } = 20;

    public override string Render()
    {
        // РАСЧЁТ: тот же самый, скопированный
        var ratio = Max == 0 ? 0 : (double)Current / Max;
        var filled = (int)Math.Round(ratio * Width);

        // РИСОВАНИЕ: другое
        var px = 10;
        var svg = new StringBuilder();
        svg.Append($"<rect x='0' y='0' width='{Width * px}' height='20' fill='#eee'/>");
        svg.Append($"<rect x='0' y='0' width='{filled * px}' height='20' fill='#c33'/>");
        svg.Append($"<text x='4' y='15' font-size='12'>HP {Current}/{Max}</text>");
        return svg.ToString();
    }
}

// ── Миникарта ───────────────────────────────────────────────────────────

public sealed class ConsoleMinimap : HudElement
{
    public List<Point> Marks { get; } = [];
    public Rect Area { get; set; } = new(0, 0, 16, 6);

    public override string Render()
    {
        // РАСЧЁТ: раскладка меток по сетке
        var grid = new char[Area.Height, Area.Width];
        for (var y = 0; y < Area.Height; y++)
            for (var x = 0; x < Area.Width; x++)
                grid[y, x] = ' ';

        foreach (var mark in Marks)
            if (mark.X < Area.Width && mark.Y < Area.Height)
                grid[mark.Y, mark.X] = '*';

        // РИСОВАНИЕ: псевдографика
        var map = new StringBuilder();
        for (var y = 0; y < Area.Height; y++)
        {
            for (var x = 0; x < Area.Width; x++) map.Append(grid[y, x]);
            map.AppendLine();
        }
        return map.ToString();
    }
}

public sealed class SvgMinimap : HudElement
{
    public List<Point> Marks { get; } = [];
    public Rect Area { get; set; } = new(0, 0, 16, 6);

    public override string Render()
    {
        // РАСЧЁТ: тот же отбор меток, снова скопированный
        var visible = Marks.Where(m => m.X < Area.Width && m.Y < Area.Height);

        // РИСОВАНИЕ: другое
        var px = 10;
        var svg = new StringBuilder();
        svg.Append($"<rect x='0' y='0' width='{Area.Width * px}' height='{Area.Height * px}' fill='#111'/>");
        foreach (var mark in visible)
            svg.Append($"<circle cx='{mark.X * px + px / 2}' cy='{mark.Y * px + px / 2}' r='3' fill='#6c6'/>");
        return svg.ToString();
    }
}

// ── Окно диалога ────────────────────────────────────────────────────────

public sealed class ConsoleDialogBox : HudElement
{
    public string Speaker { get; set; } = "";
    public string Text { get; set; } = "";
    public int Width { get; set; } = 32;

    public override string Render()
    {
        // РАСЧЁТ: перенос текста по словам
        var lines = Wrap(Text, Width - 4);

        // РИСОВАНИЕ: рамка из символов
        var box = new StringBuilder();
        box.AppendLine("+" + new string('-', Width - 2) + "+");
        box.AppendLine($"| {Speaker.PadRight(Width - 4)} |");
        foreach (var line in lines) box.AppendLine($"| {line.PadRight(Width - 4)} |");
        box.AppendLine("+" + new string('-', Width - 2) + "+");
        return box.ToString();
    }

    internal static List<string> Wrap(string text, int width)
    {
        var lines = new List<string>();
        var current = new StringBuilder();

        foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (current.Length + word.Length + 1 > width && current.Length > 0)
            {
                lines.Add(current.ToString());
                current.Clear();
            }
            if (current.Length > 0) current.Append(' ');
            current.Append(word);
        }

        if (current.Length > 0) lines.Add(current.ToString());
        return lines;
    }
}

public sealed class SvgDialogBox : HudElement
{
    public string Speaker { get; set; } = "";
    public string Text { get; set; } = "";
    public int Width { get; set; } = 32;

    public override string Render()
    {
        // РАСЧЁТ: тот же перенос — зовём чужой internal-метод, лишь бы не копировать третий раз
        var lines = ConsoleDialogBox.Wrap(Text, Width - 4);

        // РИСОВАНИЕ: другое
        var px = 8;
        var svg = new StringBuilder();
        svg.Append($"<rect x='0' y='0' width='{Width * px}' height='{(lines.Count + 2) * 16}' " +
                   "fill='#fff' stroke='#333'/>");
        svg.Append($"<text x='6' y='14' font-size='12' font-weight='bold'>{Speaker}</text>");
        for (var i = 0; i < lines.Count; i++)
            svg.Append($"<text x='6' y='{30 + i * 16}' font-size='12'>{lines[i]}</text>");
        return svg.ToString();
    }
}
