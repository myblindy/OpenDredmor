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

    public readonly record struct MouseClickedEventArgs(float X, float Y, int Button);
    public event EventHandler<MouseClickedEventArgs>? OnMouseClicked;
    protected void FireOnMouseClicked(float x, float y, int button) => OnMouseClicked?.Invoke(this, new MouseClickedEventArgs(x, y, button));

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

public record struct Sprite(string Path, Rect2 DstRect);