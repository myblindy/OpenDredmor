using Microsoft.Extensions.Hosting;

namespace OpenDredmor.CommonInterfaces.Services.Interfaces;

public abstract class BaseGame(BaseRenderer renderer) : IHostedService
{
    protected BaseRenderer Renderer { get; } = renderer;

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