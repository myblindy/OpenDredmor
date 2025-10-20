using Microsoft.Extensions.Hosting;
using OpenDredmor.CommonInterfaces.Support;

namespace OpenDredmor.CommonInterfaces;

public abstract class BaseRenderer(TimeProvider timeProvider, BaseVFS vfs, IHostApplicationLifetime appLifetime) : IDisposable
{
    protected readonly TimeProvider timeProvider = timeProvider;
    protected readonly BaseVFS vfs = vfs;
    protected readonly IHostApplicationLifetime appLifetime = appLifetime;

    public event Action? OnNewFrame;
    protected void FireOnNewFrame() => OnNewFrame?.Invoke();

    public readonly record struct MouseClickedEventArgs(float X, float Y, int Button);
    public event EventHandler<MouseClickedEventArgs>? OnMouseClicked;
    protected void FireOnMouseClicked(float x, float y, int button) => OnMouseClicked?.Invoke(this, new MouseClickedEventArgs(x, y, button));

    protected SingleThreadSynchronizationContext? synchronizationContext;

    public int Width { get; protected set; }
    public int Height { get; protected set; }

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