using Microsoft.Extensions.Hosting;
using Nito.AsyncEx;
using OpenDredmor.CommonInterfaces.Services.Interfaces;
using OpenDredmor.CommonInterfaces.Support;
using OpenDredmor.SDL3.Helpers;
using SDL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;
using Image = SixLabors.ImageSharp.Image;

namespace OpenDredmor.Renderer.SDL3;

public class RendererSDL3(TimeProvider timeProvider, BaseVFS vfs, IHostApplicationLifetime appLifetime)
    : BaseRenderer(timeProvider, vfs, appLifetime)
{
    unsafe SDL_Window* window;
    unsafe SDL_Renderer* renderer;
    unsafe TTF_TextEngine* textEngine;
    readonly Dictionary<float, IntPtr> fonts = [];
    readonly AsyncManualResetEvent shutdownCompleteEvent = new();
    readonly List<Sprite> sprites = [];
    readonly Dictionary<(string Path, int Expansion), nint> loadedTextures = [];

    public override unsafe void Run()
    {
        base.Run();

        var args = Environment.GetCommandLineArgs();
        if (SDL.SDL3.SDL_EnterAppMainCallbacks(args.Length, args, SdlAppInit, SdlAppIterate, SdlAppEvent, SdlAppQuit) != 0)
            throw new InvalidOperationException($"Failed to enter SDL app main callbacks: {SDL.SDL3.SDL_GetError()}");
        shutdownCompleteEvent.Set();
    }

    public override async Task StopAsync()
    {
        SDL.SDL3.SDL_PushEvent(new SDL_Event { type = (uint)SDL_EventType.SDL_EVENT_QUIT });
        await shutdownCompleteEvent.WaitAsync();
    }

    unsafe TTF_Font* GetFont(float size)
    {
        size *= Height / VirtualHeight;

        if (!fonts.TryGetValue(size, out var fontPtr))
        {
            var font = SDL.SDL3_ttf.TTF_OpenFontIO(SDL3StreamManager.CreateSDLIOStreamFromStream(VFS.OpenStream("fonts/Austin.ttf")), true, size);
            if (font == null)
                throw new InvalidOperationException($"Failed to load font: {SDL.SDL3.SDL_GetError()}");
            fonts[size] = fontPtr = (IntPtr)font;
        }
        return (TTF_Font*)fontPtr;
    }

    unsafe SDL_AppResult SdlAppInit(void** appState, int argc, byte** argv)
    {
        if (!SDL.SDL3.SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_AUDIO))
            throw new InvalidOperationException($"Failed to initialize SDL: {SDL.SDL3.SDL_GetError()}");
        if (!SDL.SDL3.SDL_CreateWindowAndRenderer("OpenDredmor", Width = 1920, Height = 1080, 0, out window, out renderer))
            throw new InvalidOperationException($"Failed to create SDL window and renderer: {SDL.SDL3.SDL_GetError()}");

        if (!SDL3_ttf.TTF_Init())
            throw new InvalidOperationException($"Failed to initialize SDL_ttf: {SDL.SDL3.SDL_GetError()}");
        textEngine = SDL3_ttf.TTF_CreateRendererTextEngine(renderer);

        SDL.SDL3.SDL_SetRenderDrawBlendMode(renderer, SDL_BlendMode.SDL_BLENDMODE_BLEND);

        return SDL_AppResult.SDL_APP_CONTINUE;
    }

    unsafe SDL_AppResult SdlAppIterate(void* appState)
    {
        SynchronizationContext!.ExecutePendingWorkItems();

        sprites.Clear();

        SDL.SDL3.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
        SDL.SDL3.SDL_RenderClear(renderer);
        FireOnNewFrame();
        SDL.SDL3.SDL_RenderPresent(renderer);

        return SDL_AppResult.SDL_APP_CONTINUE;
    }

    unsafe SDL_AppResult SdlAppEvent(void* appState, SDL_Event* @event)
    {
        if (@event->Type == SDL_EventType.SDL_EVENT_QUIT)
            return SDL_AppResult.SDL_APP_SUCCESS;
        else if (@event->Type == SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN)
            FireOnMouseAction(@event->button.x, @event->button.y, @event->button.button, true);
        else if (@event->Type == SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP)
            FireOnMouseAction(@event->button.x, @event->button.y, @event->button.button, false);
        else if (@event->Type == SDL_EventType.SDL_EVENT_MOUSE_MOTION)
            FireOnMouseMoved(@event->motion.x, @event->motion.y);

        return SDL_AppResult.SDL_APP_CONTINUE;
    }

    unsafe void SdlAppQuit(void* appState, SDL_AppResult result)
    {
        SDL.SDL3.SDL_DestroyRenderer(renderer);
        renderer = null;

        SDL.SDL3.SDL_DestroyWindow(window);
        window = null;

        AppLifetime.StopApplication();
    }

    public override unsafe Rect2 TransformRect2(string? image, in Rect2 rect, SpriteAnchor anchor, int expansion)
    {
        var result = rect;
        if (result.W <= 0 || result.H <= 0)
            if (image is not null && loadedTextures.TryGetValue((image, expansion), out var texture))
            {
                SDL.SDL3.SDL_GetTextureSize((SDL_Texture*)texture, out var texW, out var texH);

                if (result.W <= 0 && result.H <= 0)
                    result = result with
                    {
                        W = rect.W <= 0 ? texW : rect.W,
                        H = rect.H <= 0 ? texH : rect.H,
                    };
                else if (result.W > 0 && result.H <= 0)
                    result = result with
                    {
                        H = rect.W * texH / texW,
                    };
                else
                    throw new NotImplementedException();
            }

        return anchor switch
        {
            SpriteAnchor.TopLeft => result,
            SpriteAnchor.TopCenter => result with { X = result.X - result.W / 2 },
            SpriteAnchor.TopRight => result with { X = result.X - result.W },
            SpriteAnchor.Center => result with
            {
                X = result.X - result.W / 2,
                Y = result.Y - result.H / 2,
            },
            SpriteAnchor.LeftCenter => result with { Y = result.Y - result.H / 2 },
            _ => throw new NotImplementedException(),
        };
    }

    static readonly DecoderOptions imageDecoderOptions = new() { Configuration = { PreferContiguousImageBuffers = true } };
    public override unsafe void RenderSprites(params scoped ReadOnlySpan<Sprite> sprites)
    {
        foreach (var sprite in sprites)
        {
            if (!loadedTextures.TryGetValue((sprite.Path, sprite.Expansion), out var texture))
            {
                Image<Bgra32> image;
                using (var imageStream = VFS.OpenStream(sprite.Path, sprite.Expansion))
                    image = Image.Load<Bgra32>(imageDecoderOptions, imageStream);

                if (!image.DangerousTryGetSinglePixelMemory(out var pixelMemory))
                    throw new NotImplementedException();

                loadedTextures[(sprite.Path, sprite.Expansion)] = texture =
                    (IntPtr)SDL.SDL3.SDL_CreateTexture(renderer, SDL_PixelFormat.SDL_PIXELFORMAT_ARGB8888,
                        (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STATIC, image.Width, image.Height);

                SDL.SDL3.SDL_UpdateTexture((SDL_Texture*)texture, new SDL_Rect { w = image.Width, h = image.Height },
                    MemoryMarshal.AsBytes(pixelMemory.Span), image.Width * sizeof(Bgra32));
            }

            var transformedRect = TransformRect2(sprite.Path, sprite.DstRect, sprite.Anchor, sprite.Expansion);
            var sdlDstFRect = new SDL_FRect
            {
                x = transformedRect.X,
                y = transformedRect.Y,
                w = transformedRect.W,
                h = transformedRect.H,
            };

            if (sprite.Alpha != 1f)
                SDL.SDL3.SDL_SetTextureAlphaModFloat((SDL_Texture*)texture, sprite.Alpha);

            if (!sprite.Tile)
                SDL.SDL3.SDL_RenderTexture(renderer, (SDL_Texture*)texture, null, sdlDstFRect);
            else
                SDL.SDL3.SDL_RenderTextureTiled(renderer, (SDL_Texture*)texture, null, 1, sdlDstFRect);

            if (sprite.Alpha != 1f)
                SDL.SDL3.SDL_SetTextureAlphaModFloat((SDL_Texture*)texture, 1f);
        }
    }

    public override unsafe void RenderText(string? text, float emSize, in Rect2 rect, SpriteAnchor anchor)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        var font = GetFont(emSize);
        SDL3_ttf.TTF_SetFontWrapAlignment(font, anchor switch
        {
            SpriteAnchor.TopLeft or SpriteAnchor.LeftCenter => TTF_HorizontalAlignment.TTF_HORIZONTAL_ALIGN_LEFT,
            SpriteAnchor.TopCenter or SpriteAnchor.Center => TTF_HorizontalAlignment.TTF_HORIZONTAL_ALIGN_CENTER,
            SpriteAnchor.TopRight => TTF_HorizontalAlignment.TTF_HORIZONTAL_ALIGN_RIGHT,
            _ => throw new NotImplementedException()
        });
        var sdlText = SDL3_ttf.TTF_CreateText(textEngine, font, text, (nuint)text.Length);
        SDL3_ttf.TTF_SetTextWrapWidth(sdlText, (int)Math.Ceiling(rect.W));
        SDL3_ttf.TTF_GetTextSize(sdlText, out var textW, out var textH);

        var (x, y) = anchor switch
        {
            SpriteAnchor.TopLeft => (rect.X, rect.Y),
            SpriteAnchor.TopCenter => (rect.X + (rect.W - textW) / 2, rect.Y),
            SpriteAnchor.TopRight => (rect.X + rect.W - textW, rect.Y),
            SpriteAnchor.Center => (rect.X + (rect.W - textW) / 2, rect.Y + (rect.H - textH) / 2),
            SpriteAnchor.LeftCenter => (rect.X, rect.Y + (rect.H - textH) / 2),
            _ => throw new NotImplementedException()
        };

        SDL3_ttf.TTF_SetTextColorFloat(sdlText, 0, 0, 0, 1);
        SDL3_ttf.TTF_DrawRendererText(sdlText, x, y);

        SDL3_ttf.TTF_DestroyText(sdlText);
    }

    #region IDisposable
    bool disposedValue;

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
