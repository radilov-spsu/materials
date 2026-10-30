using SaveAdapters;
using SaveAdapters.Domain;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var menu = new SaveMenu();

var quicksave = new SaveGame
{
    Name = "quicksave",
    PlayerName = "Аня",
    Level = 7,
    Health = 84
};

menu.Save(quicksave);
Console.WriteLine($"Загружено: {menu.Load("quicksave")}");
Console.WriteLine($"Список: {string.Join(", ", menu.List())}");

Console.WriteLine();
Console.WriteLine("А теперь ошибка: слот заблокирован");

var autosave = new SaveGame
{
    Name = "autosave",
    PlayerName = "Аня",
    Level = 7,
    Health = 84
};

menu.LockFor(autosave.Name);
menu.Save(autosave);          // игра сама разбирает код ошибки — в этом и проблема

// TODO этап 1: игра работает через ISaveStorage, легаси спрятан в адаптере
// TODO этап 2: второй адаптер над облачным SDK, выбор хранилища — одной строкой
// TODO этап 3: адаптер классов — в папке python/
