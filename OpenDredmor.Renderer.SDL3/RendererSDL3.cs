using OpenDredmor.CommonInterfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
using System.Runtime.InteropServices;
using Image = SixLabors.ImageSharp.Image;

using static SDL3.SDL;

namespace OpenDredmor.Renderer.SDL3;

public unsafe class RendererSDL3 : BaseRenderer
{
    private nint window;
    private nint renderer;

    public RendererSDL3(TimeProvider timeProvider, BaseVFS vfs)
        : base(timeProvider, vfs)
    {
        if (!Init(InitFlags.Video))
            throw new ApplicationException($"Failed to initialize SDL: {GetError()}");
        if (!CreateWindowAndRenderer("OpenDredmor", Width = 800, Height = 600, 0, out window, out renderer))
            throw new ApplicationException($"Failed to create SDL window and renderer: {GetError()}");
    }

    readonly List<Sprite> sprites = [];
    readonly Dictionary<string, nint> loadedTextures = [];

    public override void Run()
    {
        var args = Environment.GetCommandLineArgs();
        if (EnterAppMainCallbacks(args.Length, args, SdlAppInit, SdlAppIterate, SdlAppEvent, SdlAppQuit) != 0)
            throw new ApplicationException($"Failed to enter SDL app main callbacks: {GetError()}");
    }

    AppResult SdlAppInit(nint appstate, int argc, string[] argv) =>
        AppResult.Continue;

    AppResult SdlAppIterate(nint appstate)
    {
        synchronizationContext.ExecutePendingWorkItems();

        sprites.Clear();

        SetRenderDrawColor(renderer, 0, 0, 0, 255);
        RenderClear(renderer);
        FireOnNewFrame();
        RenderQueuedSprites();
        RenderPresent(renderer);

        return AppResult.Continue;
    }

    AppResult SdlAppEvent(nint appstate, ref Event @event)
    {
        if ((EventType)@event.Type == EventType.Quit)
            return AppResult.Success;

        return AppResult.Continue;
    }

    void SdlAppQuit(nint appstate, AppResult result)
    {
        DestroyRenderer(renderer);
        renderer = 0;

        DestroyWindow(window);
        window = 0;

        Quit();
    }

    public override void RenderSprites(params scoped ReadOnlySpan<Sprite> sprites)
    {
        this.sprites.AddRange(sprites);
    }

    static readonly DecoderOptions imageDecoderOptions = new() { Configuration = { PreferContiguousImageBuffers = true } };
    void RenderQueuedSprites()
    {
        foreach (var sprite in sprites)
        {
            if (!loadedTextures.TryGetValue(sprite.Path, out var texture))
            {
                Image<Bgra32> image;
                using (var imageStream = vfs.OpenLatestFile(sprite.Path))
                    image = Image.Load<Bgra32>(imageDecoderOptions, imageStream);

                if (!image.DangerousTryGetSinglePixelMemory(out var pixelMemory))
                    throw new NotImplementedException();

                loadedTextures[sprite.Path] = texture = CreateTexture(renderer, PixelFormat.ARGB8888, (int)TextureAccess.Static, image.Width, image.Height);
                UpdateTexture(texture, new Rect { W = image.Width, H = image.Height }, MemoryMarshal.AsBytes(pixelMemory.Span), image.Width * sizeof(Bgra32));
            }

            RenderTexture(renderer, texture, 0, new FRect
            {
                X = sprite.DstRect.X,
                Y = sprite.DstRect.Y,
                W = sprite.DstRect.W,
                H = sprite.DstRect.H
            });
        }
    }

    #region IDisposable
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // managed state
            }

            // unmanaged state
            disposedValue = true;
        }
    }

    ~RendererSDL3()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public override void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion 
}
