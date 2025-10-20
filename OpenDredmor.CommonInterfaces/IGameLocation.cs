using Microsoft.Extensions.FileProviders;

namespace OpenDredmor.CommonInterfaces;

public interface IGameLocation
{
    IFileProvider FileProvider { get; }
}
