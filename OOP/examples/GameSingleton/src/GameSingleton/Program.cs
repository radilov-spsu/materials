using GameSingleton;
using GameSingleton.Domain;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Демонстрация гонки: 50 потоков одновременно запрашивают Instance
var ids = new System.Collections.Concurrent.ConcurrentBag<Guid>();

Parallel.For(0, 50, _ => ids.Add(GameConfig.Instance.Id));

var unique = ids.Distinct().Count();
Console.WriteLine($"Запрошено экземпляров: {ids.Count}");
Console.WriteLine($"Уникальных экземпляров: {unique}  (ожидался 1)");
Console.WriteLine();

var spawner = new WaveSpawner();
Console.WriteLine($"Сила волны: {spawner.SpawnPower(10)}");

// TODO этап 1: три корректные реализации, тест проходит для каждой
// TODO этап 2: показать тестами три проблемы одиночки
// TODO этап 3: перейти на контейнер, зависимости — через конструктор
