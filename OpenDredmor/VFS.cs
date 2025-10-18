using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using OpenDredmor.CommonInterfaces;

namespace OpenDredmor;

class VFS : BaseVFS
{
    readonly int expansionCount;

    public VFS([FromKeyedServices(ConfigurationKeys.GameDirectory)] IFileProvider fileProvider) : base(fileProvider)
    {
        // Count expansions by checking for directories named "expansionX"
        while (true)
        {
            var nextExpansionDirectoryName = expansionCount == 0 ? "expansion" : $"expansion{expansionCount + 1}";
            var dirInfo = fileProvider.GetDirectoryContents(nextExpansionDirectoryName);
            if (dirInfo.Exists)
                ++expansionCount;
            else
                break;
        }
    }

    public override Stream OpenLatestFile(string path)
    {
        for (int expansionIndex = expansionCount; expansionIndex >= 0; --expansionIndex)
        {
            var expansionPrefix = expansionIndex == 0 ? "" : expansionIndex == 1 ? "expansion" : $"expansion{expansionIndex}";
            var fullPath = Path.Combine(expansionPrefix, path);
            var fileInfo = fileProvider.GetFileInfo(fullPath);
            if (fileInfo.Exists)
                return fileInfo.CreateReadStream();
        }

        throw new IOException($"File not found in any expansion: {path}");
    }
}
