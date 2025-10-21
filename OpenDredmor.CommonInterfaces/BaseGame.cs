using Microsoft.Extensions.Hosting;

namespace OpenDredmor.CommonInterfaces;

public abstract class BaseGame : IHostedService
{
    protected BaseVFS VFS { get; }
    protected TimeProvider TimeProvider { get; }
    protected BaseRenderer Renderer { get; }
    protected BaseUI UI { get; }
    protected BaseAudio Audio { get; }

    public BaseGame(BaseVFS vfs, BaseRenderer renderer, BaseUI ui, BaseAudio audio, TimeProvider timeProvider)
    {
        VFS = vfs;
        Renderer = renderer;
        TimeProvider = timeProvider;
        UI = ui;
        Audio = audio;

        Renderer.OnMouseAction += (s, e) => OnMouseClicked(e.X, e.Y);
    }

    protected abstract void OnMouseClicked(float x, float y);

    public GameScene CurrentScene { get; set; }

    Thread? rendererThread;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        rendererThread = new Thread(Renderer.Run) { Name = "Main Renderer Thread" };
        rendererThread.Start();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken) =>
        await Renderer.StopAsync();
}

public enum GameScene
{
    MainMenu,
    InGame,
}