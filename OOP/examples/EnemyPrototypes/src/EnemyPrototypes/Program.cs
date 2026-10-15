using EnemyPrototypes;
using EnemyPrototypes.Domain;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var spawner = new WaveSpawner();

foreach (var kind in new[] { "skeleton", "crypt-guardian" })
{
    Console.WriteLine($"Волна «{kind}»:");
    foreach (var enemy in spawner.Spawn(kind, 2))
        Console.WriteLine($"  {enemy}");
    Console.WriteLine();
}

// TODO этап 1: спавнер работает с реестром прототипов, а не со switch
// TODO этап 2: проверить, что клоны не делят между собой лут и экипировку
// TODO этап 3: реестр наполняется из enemies.json
