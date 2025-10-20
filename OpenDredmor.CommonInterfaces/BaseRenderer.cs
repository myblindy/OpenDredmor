using OpenDredmor.CommonInterfaces.Support;

namespace OpenDredmor.CommonInterfaces;

public abstract class BaseRenderer : IDisposable
{
    protected readonly TimeProvider timeProvider;
    protected readonly BaseVFS vfs;

    public event Action? OnNewFrame;

    public readonly record struct MouseClickedEventArgs(float X, float Y, int Button);
    public event EventHandler<MouseClickedEventArgs>? OnMouseClicked;

    protected SingleThreadSynchronizationContext synchronizationContext;

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public BaseRenderer(TimeProvider timeProvider, BaseVFS vfs)
    {
        this.timeProvider = timeProvider;
        this.vfs = vfs;
    }

    protected void FireOnNewFrame() => OnNewFrame?.Invoke();
    protected void FireOnMouseClicked(float x, float y, int button) => OnMouseClicked?.Invoke(this, new MouseClickedEventArgs(x, y, button));

    public virtual void Run()
    {
        synchronizationContext = new SingleThreadSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(synchronizationContext);
    }
    public abstract Task StopAsync();

    public abstract void RenderSprites(params ReadOnlySpan<Sprite> sprites);

    public abstract void Dispose();
}

public record struct Sprite(string Path, Rect2 DstRect);