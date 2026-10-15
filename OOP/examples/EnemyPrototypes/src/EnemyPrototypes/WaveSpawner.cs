using EnemyPrototypes.Domain;

namespace EnemyPrototypes;

/// <summary>
/// ЭТАП 0. Спавнер знает каждый конкретный класс врага.
/// Новый вид от дизайнера = новый класс + правка этого switch.
/// </summary>
public class WaveSpawner
{
    public IReadOnlyList<Enemy> Spawn(string kind, int count)
    {
        var wave = new List<Enemy>();

        for (var i = 0; i < count; i++)
        {
            Enemy enemy = kind switch
            {
                "skeleton"         => new Skeleton(),
                "armored-skeleton" => new ArmoredSkeleton(),
                "skeleton-archer"  => new SkeletonArcher(),
                "crypt-guardian"   => new CryptGuardian(),
                "ghoul"            => new Ghoul(),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Неизвестный вид врага")
            };

            wave.Add(enemy);
        }

        return wave;
    }
}
