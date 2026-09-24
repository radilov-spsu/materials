using System.Text;
using DungeonBuilder.Domain;

namespace DungeonBuilder;

/// <summary>
/// ЭТАП 0. Генератор одновременно обходит план И форматирует ASCII-карту:
/// процесс конструирования смешан с представлением результата.
///
/// Задание этапа 0 — НЕ переписывать сразу, а сначала разобрать:
///   1) какие шаги обхода здесь есть (это будущий интерфейс строителя);
///   2) какие строки относятся к процессу, а какие — к представлению;
///   3) что придётся сделать, если тот же обход понадобится с выводом в JSON;
///      а если оба формата нужны одновременно?
/// </summary>
public class DungeonGenerator
{
    public string Generate(DungeonPlan plan)
    {
        var map = new StringBuilder();

        map.AppendLine($"# Подземелье «{plan.Title}»");
        map.AppendLine();

        foreach (var room in plan.Rooms)
        {
            map.AppendLine($"[{room.Id}] комната {room.Width}x{room.Height}");

            foreach (var enemy in room.Enemies)
                map.AppendLine($"    враг: {enemy}");

            if (room.HasChest)
                map.AppendLine("    сундук");
        }

        map.AppendLine();

        foreach (var corridor in plan.Corridors)
            map.AppendLine($"[{corridor.From}] --{corridor.Length}-- [{corridor.To}]");

        return map.ToString();
    }
}
