using SaveAdapters.Domain;
using SaveAdapters.Foreign;

namespace SaveAdapters;

/// <summary>
/// ЭТАП 0. Игровое меню работает с легаси-системой напрямую и знает про всё:
/// про номера слотов, про формат строки, про коды ошибок.
///
/// Задание этапа 0 — НЕ переписывать сразу, а сначала разобрать:
///   1) выписать ВСЕ места, где игровой код знает детали чужой системы;
///   2) прикинуть, что придётся изменить при подключении облачного SDK;
///   3) назвать нарушенный принцип и запах кода из каталога.
/// </summary>
public class SaveMenu
{
    private readonly LegacySaveSystem _legacy = new();

    // знание о чужой системе №1: имя сохранения превращается в номер слота ЗДЕСЬ
    private static int SlotOf(string name) => Math.Abs(name.GetHashCode()) % LegacySaveSystem.SlotCount;

    public void Save(SaveGame save)
    {
        // знание о чужой системе №2: формат хранения собирается ЗДЕСЬ
        var raw = $"{save.Name}|{save.PlayerName}|{save.Level}|{save.Health}";

        var code = _legacy.WriteSlot(SlotOf(save.Name), raw);

        // знание о чужой системе №3: коды ошибок разбираются ЗДЕСЬ
        if (code == LegacySaveSystem.ErrDiskFull) Console.WriteLine("Нет места в хранилище");
        else if (code == LegacySaveSystem.ErrLocked) Console.WriteLine("Слот заблокирован");
        else if (code == LegacySaveSystem.ErrBadSlot) Console.WriteLine("Неверный слот");
        else Console.WriteLine($"Сохранено: {save.Name}");
    }

    public SaveGame? Load(string name)
    {
        var raw = _legacy.ReadSlot(SlotOf(name));

        // знание о чужой системе №4: пустая строка означает «нет сохранения»
        if (string.IsNullOrEmpty(raw)) return null;

        // знание о чужой системе №5: разбор формата — снова здесь
        var parts = raw.Split('|');
        return new SaveGame
        {
            Name = parts[0],
            PlayerName = parts[1],
            Level = int.Parse(parts[2]),
            Health = int.Parse(parts[3])
        };
    }

    public IReadOnlyList<string> List()
    {
        var names = new List<string>();

        // знание о чужой системе №6: обход слотов и разбор формата
        foreach (var slot in _legacy.UsedSlots())
            names.Add(_legacy.ReadSlot(slot).Split('|')[0]);

        return names;
    }

    /// <summary>Для демонстрации ошибки: заблокировать слот под этим именем.</summary>
    public void LockFor(string name) => _legacy.LockSlot(SlotOf(name));
}
