using Microsoft.Extensions.FileProviders;

namespace OpenDredmor.CommonInterfaces.Services.Interfaces;

public interface IGameLocation
{
    IFileProvider FileProvider { get; }
}
