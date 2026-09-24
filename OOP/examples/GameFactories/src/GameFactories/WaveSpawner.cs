using GameFactories.Domain;

namespace GameFactories;

/// <summary>
/// ЭТАП 0. Заведомо плохой код: создание врагов, выдача оружия и лута
/// смешаны с ветвлением по локации. С него начинается лабораторная работа.
///
/// Задание этапа 0 — НЕ переписывать сразу, а сначала посчитать:
///   1) от скольких конкретных классов зависит этот класс сейчас;
///   2) сколько их станет после добавления третьей локации;
///   3) какие части метода меняются при добавлении локации, а какие — никогда.
/// </summary>
public class WaveSpawner
{
    public IReadOnlyList<Enemy> Spawn(string location, int count)
    {
        var enemies = new List<Enemy>();

        for (var i = 0; i < count; i++)
        {
            Enemy enemy;

            if (location == "forest")
            {
                enemy = i % 3 == 0 ? new Wolf() : new Bandit();
                enemy.Weapon = new RustySword();
                enemy.Loot = new Herb();
            }
            else if (location == "desert")
            {
                enemy = i % 3 == 0 ? new SandWorm() : new Nomad();
                enemy.Weapon = new CurvedDagger();
                enemy.Loot = new CactusJuice();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(location), location,
                    "Неизвестная локация");
            }

            enemy.Spawn();
            enemies.Add(enemy);
        }

        return enemies;
    }
}
