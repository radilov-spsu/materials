using DungeonBuilder;
using DungeonBuilder.Domain;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var plan = DungeonPlan.Sample();

var generator = new DungeonGenerator();
Console.WriteLine(generator.Generate(plan));

// TODO этап 1: распорядитель получает строителя, карта собирается AsciiMapBuilder
// TODO этап 2: здесь же печатается сводка от третьего строителя
// TODO этап 3: собрать предмет текучим строителем и вывести его

var sword = new Item("Клинок зари", Rarity.Epic, 42, 5.5, true, 7, 1200, null);
Console.WriteLine(sword);
