using GameFactories;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var spawner = new WaveSpawner();

Console.WriteLine("Волна в лесу:");
var forestWave = spawner.Spawn("forest", 4);

Console.WriteLine();
Console.WriteLine("Волна в пустыне:");
var desertWave = spawner.Spawn("desert", 4);

Console.WriteLine();
Console.WriteLine($"Итого врагов: {forestWave.Count + desertWave.Count}");

// TODO этап 2: здесь должен создаваться уровень, а волну запускает он
// TODO этап 3: локация получает семейство продуктов через конструктор
