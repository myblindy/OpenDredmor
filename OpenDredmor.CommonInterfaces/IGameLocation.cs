using Microsoft.Extensions.FileProviders;

namespace OpenDredmor.CommonInterfaces;

public interface IGameLocation
{
    public IFileProvider FileProvider { get; }
}
