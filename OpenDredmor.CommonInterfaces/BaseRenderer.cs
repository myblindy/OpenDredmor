using OpenDredmor.CommonInterfaces.Support;

namespace OpenDredmor.CommonInterfaces;

public abstract class BaseRenderer : IDisposable
{
    protected readonly TimeProvider timeProvider;
    protected readonly BaseVFS vfs;

    public event Action? OnNewFrame;

    protected readonly SingleThreadSynchronizationContext synchronizationContext;

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public BaseRenderer(TimeProvider timeProvider, BaseVFS vfs)
    {
        this.timeProvider = timeProvider;
        this.vfs = vfs;
        synchronizationContext = new SingleThreadSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(synchronizationContext);
    }

    protected void FireOnNewFrame() => OnNewFrame?.Invoke();

    public abstract void Run();

    public abstract void RenderSprites(params ReadOnlySpan<Sprite> sprites);

    public abstract void Dispose();
}

public record struct Sprite(string Path, Rect2 DstRect);