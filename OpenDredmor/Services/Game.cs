using Microsoft.Extensions.DependencyInjection;
using OpenDredmor.CommonInterfaces.Models;
using OpenDredmor.CommonInterfaces.Services.Interfaces;

namespace OpenDredmor.Services;

sealed partial class Game : BaseGame
{
    readonly GameModel gameModel;
    readonly IServiceProvider serviceProvider;

    public Game(BaseVFS vfs, BaseRenderer renderer, GameModel gameModel, IServiceProvider serviceProvider)
        : base(renderer)
    {
        this.gameModel = gameModel;
        this.serviceProvider = serviceProvider;
        renderer.OnNewFrame += OnNewFrame;
        gameModel.MainMenuBackgroundExpansion = vfs.ExpansionDirectoryNames.Length - 1;
    }

    void OnNewFrame()
    {
        serviceProvider.GetRequiredKeyedService<IGameScene>(gameModel.CurrentScene)
            .RenderScene();
    }
}
