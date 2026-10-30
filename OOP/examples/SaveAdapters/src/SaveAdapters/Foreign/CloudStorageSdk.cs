namespace SaveAdapters.Foreign;

// ╔══════════════════════════════════════════════════════════════════════╗
// ║  ЧУЖОЙ КОД — НЕ МЕНЯТЬ                                               ║
// ║  Сторонний SDK облачного хранилища. Работает с массивами байтов,     ║
// ║  асинхронный через коллбэки (Task не знает), ключи строковые,        ║
// ║  статус возвращает первым аргументом коллбэка.                       ║
// ╚══════════════════════════════════════════════════════════════════════╝
public sealed class CloudStorageSdk
{
    public const int StatusOk = 200;
    public const int StatusNotFound = 404;
    public const int StatusQuotaExceeded = 507;

    private readonly Dictionary<string, byte[]> _objects = new();
    private readonly int _quotaBytes;

    public CloudStorageSdk(int quotaBytes = 4096) => _quotaBytes = quotaBytes;

    public void Upload(string key, byte[] payload, Action<int> onComplete)
    {
        // SDK выполняет работу в своём потоке и зовёт коллбэк оттуда
        ThreadPool.QueueUserWorkItem(_ =>
        {
            Thread.Sleep(20);                               // имитация сети

            var used = _objects.Values.Sum(v => v.Length);
            if (used + payload.Length > _quotaBytes)
            {
                onComplete(StatusQuotaExceeded);
                return;
            }

            lock (_objects) _objects[key] = payload;
            onComplete(StatusOk);
        });
    }

    public void Download(string key, Action<int, byte[]?> onComplete)
    {
        ThreadPool.QueueUserWorkItem(_ =>
        {
            Thread.Sleep(20);

            byte[]? payload;
            lock (_objects)
            {
                if (!_objects.TryGetValue(key, out payload))
                {
                    onComplete(StatusNotFound, null);
                    return;
                }
            }

            onComplete(StatusOk, payload);
        });
    }

    public void ListKeys(Action<int, string[]> onComplete)
    {
        ThreadPool.QueueUserWorkItem(_ =>
        {
            Thread.Sleep(20);
            lock (_objects) onComplete(StatusOk, _objects.Keys.ToArray());
        });
    }
}
