using Microsoft.Extensions.FileProviders;

namespace OpenDredmor.CommonInterfaces;

public abstract class BaseVFS(IFileProvider fileProvider)
{
    protected readonly IFileProvider fileProvider = fileProvider;

    public abstract Stream OpenLatestFile(string path);
}
