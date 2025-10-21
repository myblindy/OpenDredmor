using Microsoft.Extensions.Hosting;
using OpenDredmor.CommonInterfaces.Support;

namespace OpenDredmor.CommonInterfaces;

public abstract class BaseRenderer(TimeProvider timeProvider, BaseVFS vfs, IHostApplicationLifetime appLifetime) : IDisposable
{
    protected TimeProvider TimeProvider { get; } = timeProvider;
    protected BaseVFS VFS { get; } = vfs;
    protected IHostApplicationLifetime AppLifetime { get; } = appLifetime;

    public event Action? OnNewFrame;
    protected void FireOnNewFrame() => OnNewFrame?.Invoke();

    public readonly record struct MouseActionEventArgs(float X, float Y, int Button, bool Pressed);
    public event EventHandler<MouseActionEventArgs>? OnMouseAction;
    protected void FireOnMouseAction(float x, float y, int button, bool pressed) =>
        OnMouseAction?.Invoke(this, new MouseActionEventArgs(x, y, button, pressed));

    public readonly record struct MouseMovedEventArgs(float X, float Y);
    public event EventHandler<MouseMovedEventArgs>? OnMouseMoved;
    protected void FireOnMouseMoved(float x, float y) =>
        OnMouseMoved?.Invoke(this, new MouseMovedEventArgs(x, y));

    protected SingleThreadSynchronizationContext? SynchronizationContext { get; private set; }

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public const int VirtualWidth = 1920;
    public const int VirtualHeight = 1080;

    public virtual void Run()
    {
        SynchronizationContext = new SingleThreadSynchronizationContext();
        System.Threading.SynchronizationContext.SetSynchronizationContext(SynchronizationContext);
    }
    public abstract Task StopAsync();

    public abstract void RenderSprites(params ReadOnlySpan<Sprite> sprites);

    public abstract void Dispose();
}

public record struct Sprite(string Path, Rect2 DstRect, int Expansion = -1);