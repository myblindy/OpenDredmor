using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenDredmor;
using OpenDredmor.Audio.SDL3;
using OpenDredmor.CommonInterfaces;
using OpenDredmor.Renderer.SDL3;

var appBuilder = Host.CreateApplicationBuilder(args);
appBuilder.Logging.AddConsole();
appBuilder.Services
    .AddSingleton(TimeProvider.System)
    .AddSingleton<BaseVFS, VFS>()
    .AddSingleton<BaseAudio, AudioSDL3>()
    .AddSingleton<BaseRenderer, RendererSDL3>()
    .AddSingleton<IGameLocation, GameLocation>()
    .AddSingleton<BaseUI, UI>()
    .AddHostedService<Game>();

var app = appBuilder.Build();
await app.RunAsync();
