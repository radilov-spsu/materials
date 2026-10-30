namespace SaveAdapters.Foreign;

// ╔══════════════════════════════════════════════════════════════════════╗
// ║  ЧУЖОЙ КОД — НЕ МЕНЯТЬ                                               ║
// ║  Наш же модуль десятилетней давности. Синхронный, работает со        ║
// ║  слотами по номеру, хранит плоскую строку, об ошибках сообщает       ║
// ║  кодами возврата. Исходники формально наши, но модуль используют     ║
// ║  ещё три проекта, поэтому править его нельзя.                        ║
// ╚══════════════════════════════════════════════════════════════════════╝
public sealed class LegacySaveSystem
{
    public const int Ok = 0;
    public const int ErrDiskFull = 1;
    public const int ErrLocked = 2;
    public const int ErrBadSlot = 3;

    public const int SlotCount = 8;

    private readonly string?[] _slots = new string?[SlotCount];
    private readonly bool[] _locked = new bool[SlotCount];

    /// <summary>Возвращает код ошибки, а не бросает исключение.</summary>
    public int WriteSlot(int slot, string data)
    {
        if (slot < 0 || slot >= SlotCount) return ErrBadSlot;
        if (_locked[slot]) return ErrLocked;
        if (data.Length > 256) return ErrDiskFull;

        _slots[slot] = data;
        return Ok;
    }

    /// <summary>Пустой слот — это пустая строка, а не null.</summary>
    public string ReadSlot(int slot)
    {
        if (slot < 0 || slot >= SlotCount) return "";
        return _slots[slot] ?? "";
    }

    public int[] UsedSlots()
    {
        var used = new List<int>();
        for (var i = 0; i < SlotCount; i++)
            if (!string.IsNullOrEmpty(_slots[i])) used.Add(i);
        return used.ToArray();
    }

    /// <summary>Для тестов: заблокировать слот, чтобы получить ErrLocked.</summary>
    public void LockSlot(int slot) => _locked[slot] = true;
}
