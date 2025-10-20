using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
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
    .AddSingleton<BaseGame, Game>()
    .AddSingleton<IGameLocation, GameLocation>();

var app = appBuilder.Build();

app.Services.GetRequiredService<BaseGame>().Run();