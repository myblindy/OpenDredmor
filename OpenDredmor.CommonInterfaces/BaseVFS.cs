using Microsoft.Extensions.FileProviders;

namespace OpenDredmor.CommonInterfaces;

public abstract class BaseVFS(IFileProvider fileProvider)
{
    protected IFileProvider FileProvider { get; } = fileProvider;

    public abstract Stream OpenLatestFile(string path);
}
