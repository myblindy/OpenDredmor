using SDL;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OpenDredmor.SDL3.Helpers;

public static class SDL3StreamManager
{
    unsafe delegate long SizeGetterDelegate(void* userdata);
    unsafe delegate SDLBool CloseDelegate(void* userdata);
    unsafe delegate SDLBool FlushDelegate(void* userdata, SDL_IOStatus* status);
    unsafe delegate nuint ReadDelegate(void* userdata, void* ptr, nuint size, SDL_IOStatus* status);
    unsafe delegate nuint WriteDelegate(void* userdata, void* ptr, nuint size, SDL_IOStatus* status);
    unsafe delegate long SeekDelegate(void* userdata, long offset, SDL_IOWhence whence);
    class StreamData
    {
        public required SizeGetterDelegate SizeGetter { get; set; }
        public required CloseDelegate Close { get; set; }
        public required FlushDelegate Flush { get; set; }
        public required ReadDelegate Read { get; set; }
        public required WriteDelegate Write { get; set; }
        public required SeekDelegate Seek { get; set; }
        unsafe public SDL_IOStream* SdlStream;
    }
    readonly static ConcurrentDictionary<Stream, StreamData> streams = [];

    public unsafe static SDL_IOStream* CreateSDLIOStreamFromStream(Stream stream, bool own = true)
    {
        StreamData streamData = new()
        {
            SizeGetter = _ => 
                stream.Length,
            Close = _ =>
            {
                if (own) stream.Dispose();
                return streams.Remove(stream, out var _);
            },
            Flush = (_, _) =>
            {
                stream.Flush();
                return true;
            },
            Read = (_, ptr, size, status) =>
            {
                try
                {
                    var bytesRead = stream.Read(new Span<byte>((byte*)ptr, (int)size));
                    if ((nuint)bytesRead < size && status != null)
                        *status = SDL_IOStatus.SDL_IO_STATUS_EOF;
                    return (nuint)bytesRead;
                }
                catch
                {
                    if (status != null)
                        *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
                    return 0;
                }
            },
            Write = (_, ptr, size, status) =>
            {
                try
                {
                    stream.Write(new ReadOnlySpan<byte>((byte*)ptr, (int)size));
                    return size;
                }
                catch
                {
                    if (status != null)
                        *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
                    return 0;
                }
            },
            Seek = (_, offset, whence) =>
            {
                SeekOrigin origin = whence switch
                {
                    SDL_IOWhence.SDL_IO_SEEK_SET => SeekOrigin.Begin,
                    SDL_IOWhence.SDL_IO_SEEK_CUR => SeekOrigin.Current,
                    SDL_IOWhence.SDL_IO_SEEK_END => SeekOrigin.End,
                    _ => throw new ArgumentOutOfRangeException(nameof(whence), "Invalid seek origin.")
                };
                return stream.Seek(offset, origin);
            }
        };
        streams[stream] = streamData;

        var sizeGetterPtr = (delegate* unmanaged[Cdecl]<nint, long>)Marshal.GetFunctionPointerForDelegate(streamData.SizeGetter);
        var closePtr = (delegate* unmanaged[Cdecl]<nint, SDLBool>)Marshal.GetFunctionPointerForDelegate(streamData.Close);
        var flushPtr = (delegate* unmanaged[Cdecl]<nint, SDL_IOStatus*, SDLBool>)Marshal.GetFunctionPointerForDelegate(streamData.Flush);
        var readPtr = (delegate* unmanaged[Cdecl]<nint, nint, nuint, SDL_IOStatus*, nuint>)Marshal.GetFunctionPointerForDelegate(streamData.Read);
        var writePtr = (delegate* unmanaged[Cdecl]<nint, nint, nuint, SDL_IOStatus*, nuint>)Marshal.GetFunctionPointerForDelegate(streamData.Write);
        var seekPtr = (delegate* unmanaged[Cdecl]<nint, long, SDL_IOWhence, long>)Marshal.GetFunctionPointerForDelegate(streamData.Seek);

        var sdlStreamInterface = new SDL_IOStreamInterface
        {
            version = (uint)Unsafe.SizeOf<SDL_IOStreamInterface>(),
            size = sizeGetterPtr,
            close = closePtr,
            flush = flushPtr,
            read = readPtr,
            write = writePtr,
            seek = seekPtr
        };
        return streamData.SdlStream = SDL.SDL3.SDL_OpenIO(sdlStreamInterface, 0);
    }
}
