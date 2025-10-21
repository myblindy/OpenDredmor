using Microsoft.Extensions.FileProviders;
using System.Collections.Immutable;

namespace OpenDredmor.CommonInterfaces;

public abstract class BaseVFS(IFileProvider fileProvider)
{
    public ImmutableArray<string> ExpansionDirectoryNames { get; protected set; }
    protected IFileProvider FileProvider { get; } = fileProvider;

    public abstract Stream OpenStream(string path, int expansion = -1);
}
