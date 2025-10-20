using OpenDredmor.CommonInterfaces;

namespace OpenDredmor;

sealed class VFS : BaseVFS
{
    readonly string[] expansionDirectoryNames;

    public VFS(IGameLocation gameLocation) : base(gameLocation.FileProvider)
    {
        // Count expansions by checking for directories named "expansionX"
        var expansionCount = 0;
        List<string> expansionDirectoryNames = [""];
        while (true)
        {
            var nextExpansionDirectoryName = expansionCount == 0 ? "expansion" : $"expansion{expansionCount + 1}";
            var dirInfo = FileProvider.GetDirectoryContents(nextExpansionDirectoryName);
            if (dirInfo.Exists)
            {
                ++expansionCount;
                expansionDirectoryNames.Add(nextExpansionDirectoryName);
            }
            else
                break;
        }

        this.expansionDirectoryNames = [.. expansionDirectoryNames.AsEnumerable().Reverse()];
    }

    public override Stream OpenLatestFile(string path)
    {
        foreach (var expansionDirectoryName in expansionDirectoryNames)
        {
            var fullPath = Path.Combine(expansionDirectoryName, path);
            var fileInfo = FileProvider.GetFileInfo(fullPath);
            if (fileInfo.Exists)
                return fileInfo.CreateReadStream();
        }

        throw new IOException($"File not found in any expansion: {path}");
    }
}
