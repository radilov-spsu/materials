using System.Text;
using HudBridge.Geometry;
using HudBridge.Hud;

Console.OutputEncoding = Encoding.UTF8;

// ── Консольная версия интерфейса ────────────────────────────────────────
var consoleHud = new HudElement[]
{
    new ConsoleHealthBar { Current = 42, Max = 100 },
    new ConsoleMinimap { Marks = { new Point(2, 1), new Point(9, 4) } },
    new ConsoleDialogBox
    {
        Speaker = "Страж склепа",
        Text = "Дальше хода нет. Поверни назад, пока цел."
    }
};

foreach (var element in consoleHud)
    Console.WriteLine(element.Render());

// ── Та же самая раскладка, но в SVG — и ради этого пришлось завести
//    ещё три класса. В этом и проблема этапа 0.
var svgHud = new HudElement[]
{
    new SvgHealthBar { Current = 42, Max = 100 },
    new SvgMinimap { Marks = { new Point(2, 1), new Point(9, 4) } },
    new SvgDialogBox
    {
        Speaker = "Страж склепа",
        Text = "Дальше хода нет. Поверни назад, пока цел."
    }
};

var svg = new StringBuilder("<svg xmlns='http://www.w3.org/2000/svg' width='400' height='300'>");
var offset = 0;
foreach (var element in svgHud)
{
    svg.Append($"<g transform='translate(0,{offset})'>{element.Render()}</g>");
    offset += 90;
}
svg.Append("</svg>");

File.WriteAllText("hud.svg", svg.ToString());
Console.WriteLine("SVG сохранён в hud.svg");

// TODO этап 1: одна иерархия элементов + одна иерархия рендереров
// TODO этап 2: новый элемент не трогает рендереры, новый рендерер не трогает элементы
// TODO этап 3: сменить рендерер у живого объекта, не теряя состояние
