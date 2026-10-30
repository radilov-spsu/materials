using SaveAdapters;
using SaveAdapters.Domain;
using Xunit;

namespace SaveAdapters.Tests;

public class SaveMenuTests
{
    private static SaveGame Sample(string name = "quicksave") => new()
    {
        Name = name,
        PlayerName = "Аня",
        Level = 7,
        Health = 84
    };

    [Fact]
    public void Сохранение_читается_обратно()
    {
        var menu = new SaveMenu();

        menu.Save(Sample());
        var loaded = menu.Load("quicksave");

        Assert.NotNull(loaded);
        Assert.Equal(7, loaded!.Level);
    }

    [Fact]
    public void Несуществующее_сохранение_даёт_null()
    {
        var menu = new SaveMenu();

        Assert.Null(menu.Load("нет такого"));
    }

    // Этапы 1–2: тесты пишутся против ISaveStorage и прогоняются против ОБОИХ
    // адаптеров через [Theory]. Заготовка источника данных:
    //
    // public static IEnumerable<object[]> Storages() =>
    // [
    //     [new LegacySaveAdapter(new LegacySaveSystem())],
    //     [new CloudSaveAdapter(new CloudStorageSdk())]
    // ];
    //
    // [Theory]
    // [MemberData(nameof(Storages))]
    // public async Task Сохранение_читается_обратно_в_любом_хранилище(ISaveStorage storage) { }

    // TODO этап 1: каждый код ошибки легаси превращается в своё исключение
    // TODO этап 2: превышение квоты облака превращается в исключение
    // TODO этап 2: отмена по CancellationToken
    // TODO этап 2: список сохранений одинаков по смыслу в обоих хранилищах
}
