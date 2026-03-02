namespace CapsMapper.Services;

/// <summary>
/// Ensures only one instance of the application can run at a time.
/// </summary>
public sealed class SingleInstanceGuard : IDisposable
{
    private readonly Mutex _mutex;
    public bool IsFirstInstance { get; }

    public SingleInstanceGuard(string name)
    {
        _mutex = new Mutex(true, $"Global\\{name}", out var createdNew);
        IsFirstInstance = createdNew;
    }

    public void Dispose() => _mutex.Dispose();
}
