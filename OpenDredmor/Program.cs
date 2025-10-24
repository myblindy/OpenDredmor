using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenDredmor.Audio.SDL3;
using OpenDredmor.CommonInterfaces.Models;
using OpenDredmor.CommonInterfaces.Services.Interfaces;
using OpenDredmor.Renderer.SDL3;
using OpenDredmor.Services;

var appBuilder = Host.CreateApplicationBuilder(args);
appBuilder.Logging.AddConsole();
appBuilder.Services
    .AddSingleton(TimeProvider.System)
    .AddSingleton<BaseVFS, VFS>()
    .AddSingleton<BaseAudio, AudioSDL3>()
    .AddSingleton<BaseRenderer, RendererSDL3>()
    .AddSingleton<IGameLocation, GameLocation>()
    .AddSingleton<BaseUI, UI>()
    .AddSingleton<BaseGameData, GameData>()
    .AddHostedService(p => p.GetRequiredService<BaseGameData>())

    .AddSingleton<GameModel>()
    .AddKeyedSingleton<IGameScene, GameSceneMainMenu>(GameScene.MainMenu)
    .AddKeyedSingleton<IGameScene, GameSceneNewGameChooseDifficultyMenu>(GameScene.NewGameChooseDifficultyMenu)
    .AddKeyedSingleton<IGameScene, GameSceneNewGameNameMenu>(GameScene.NewGameNameMenu)
    .AddKeyedSingleton<IGameScene, GameSceneNewGameSkillSelectionMenu>(GameScene.NewGameSkillSelectionMenu)

    .AddHostedService<Game>();

var app = appBuilder.Build();
await app.RunAsync();
