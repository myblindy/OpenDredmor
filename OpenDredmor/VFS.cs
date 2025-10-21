using OpenDredmor.CommonInterfaces;

namespace OpenDredmor;

sealed class VFS : BaseVFS
{
    static string GetExpansionDirectoryName(int expansion) =>
        expansion switch
        {
            0 => "",
            1 => "expansion",
            > 1 => $"expansion{expansion}",
            _ => throw new ArgumentOutOfRangeException(nameof(expansion), "Expansion must be non-negative"),
        };

    public VFS(IGameLocation gameLocation) : base(gameLocation.FileProvider)
    {
        // Count expansions by checking for directories named "expansionX"
        var expansionCount = 0;
        List<string> expansionDirectoryNames = [""];
        while (true)
        {
            var nextExpansionDirectoryName = GetExpansionDirectoryName(expansionCount + 1);
            var dirInfo = FileProvider.GetDirectoryContents(nextExpansionDirectoryName);
            if (dirInfo.Exists)
            {
                ++expansionCount;
                expansionDirectoryNames.Add(nextExpansionDirectoryName);
            }
            else
                break;
        }

        ExpansionDirectoryNames = [.. expansionDirectoryNames];
    }

    public override Stream OpenStream(string path, int expansion)
    {
        if (expansion < 0)
            foreach (var expansionDirectoryName in ExpansionDirectoryNames.Reverse())
            {
                var fullPath = Path.Combine(expansionDirectoryName, path);
                var fileInfo = FileProvider.GetFileInfo(fullPath);
                if (fileInfo.Exists)
                    return fileInfo.CreateReadStream();
            }
        else
            return FileProvider.GetFileInfo(Path.Combine(GetExpansionDirectoryName(expansion), path)).CreateReadStream();

        throw new IOException($"File not found in any expansion: {path}");
    }
}
