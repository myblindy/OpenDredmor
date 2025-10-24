using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using OpenDredmor.CommonInterfaces.Services.Interfaces;

namespace OpenDredmor.Services;

sealed class GameLocation : IGameLocation
{
    public IFileProvider FileProvider { get; }

    public GameLocation(IConfiguration configuration, ILogger<GameLocation> logger)
    {
        var gameDirectory = configuration["game-directory"];

        if (gameDirectory is null)
            if (OperatingSystem.IsWindows())
            {
                // get the game from Steam
                using var steamRegistryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam");
                if (steamRegistryKey?.GetValue("InstallPath") is string { } steamInstallPath
                    && Path.Combine(steamInstallPath, "steamapps", "common", "Dungeons of Dredmor") is { } potentialGameDirectory
                    && Directory.Exists(potentialGameDirectory))
                {
                    gameDirectory = potentialGameDirectory;
                }
            }

        gameDirectory ??= Path.Combine(Environment.ProcessPath!, "Dungeons of Dredmor");
        FileProvider = new PhysicalFileProvider(gameDirectory);

        logger.LogInformation("Using game directory: {GameDirectory}", gameDirectory);
    }
}
