using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using OpenDredmor;
using OpenDredmor.CommonInterfaces;
using OpenDredmor.Renderer.SDL3;


var appBuilder = Host.CreateApplicationBuilder(args);
appBuilder.Logging.AddConsole();
appBuilder.Services
    .AddSingleton(TimeProvider.System)
    .AddSingleton<BaseVFS, VFS>()
    .AddSingleton<BaseRenderer, RendererSDL3>()
    .AddSingleton<BaseGame, Game>();

if (OperatingSystem.IsWindows())
{
    using var steamRegistryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam");
    var steamInstallPath = steamRegistryKey?.GetValue("InstallPath") as string ?? throw new NotImplementedException();

    // TODO handle libraries
    appBuilder.Services.AddKeyedSingleton<IFileProvider>(ConfigurationKeys.GameDirectory,
        new PhysicalFileProvider(Path.Combine(steamInstallPath, "steamapps", "common", "Dungeons of Dredmor")));
}
else
    throw new NotImplementedException();

var app = appBuilder.Build();

app.Services.GetRequiredService<BaseGame>().Run();