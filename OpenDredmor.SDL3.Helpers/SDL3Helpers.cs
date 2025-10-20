using SDL;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenDredmor.SDL3.Helpers;

public static unsafe class SDL3Helpers
{
    public delegate SDL_AppResult SDLAppInitFunc(void** appState, int argc, byte** argv);
    public delegate SDL_AppResult SDLAppIterateFunc(void* appState);
    public delegate SDL_AppResult SDLAppEventFunc(void* appState, SDL_Event* sdlEvent);
    public delegate void SDLAppQuitFunc(void* appState, SDL_AppResult appResult);

    extension(SDL.SDL3)
    {
        public static SDLBool SDL_CreateWindowAndRenderer(string title, int width, int height,
            SDL_WindowFlags windowFlags, out SDL_Window* window, out SDL_Renderer* renderer)
        {
            fixed (SDL_Window** pWindow = &window)
            fixed (SDL_Renderer** pRenderer = &renderer)
            fixed (byte* pTitle = Encoding.UTF8.GetBytes(title))
                return SDL.SDL3.SDL_CreateWindowAndRenderer(pTitle, width, height, windowFlags, pWindow, pRenderer);
        }

        public static SDLBool SDL_UpdateTexture(SDL_Texture* texture, in SDL_Rect rect,
            ReadOnlySpan<byte> pixels, int pitch)
        {
            fixed (byte* pPixels = pixels)
            fixed (SDL_Rect* pRect = &rect)
                return SDL.SDL3.SDL_UpdateTexture(texture, pRect, (IntPtr)pPixels, pitch);
        }

        public static SDLBool SDL_RenderTexture(SDL_Renderer* renderer, SDL_Texture* texture,
            SDL_FRect* srcRect, in SDL_FRect dstRect)
        {
            fixed (SDL_FRect* pDstRect = &dstRect)
                return SDL.SDL3.SDL_RenderTexture(renderer, texture, srcRect, pDstRect);
        }

        public static int SDL_EnterAppMainCallbacks(int argc, string[] argv,
            SDLAppInitFunc appInitFunc, SDLAppIterateFunc appIterateFunc, SDLAppEventFunc appEventFunc,
            SDLAppQuitFunc appQuitFunc)
        {
            byte** argvPtrs = stackalloc byte*[argv.Length];
            for (int i = 0; i < argv.Length; i++)
            {
                var argBytes = Encoding.UTF8.GetBytes(argv[i] + '\0');
                var pArg = (byte*)NativeMemory.Alloc((nuint)argBytes.Length);
                Marshal.Copy(argBytes, 0, (IntPtr)pArg, argBytes.Length);
                argvPtrs[i] = pArg;
            }

            var appInitFuncDelegate = (delegate* unmanaged[Cdecl]<nint*, int, byte**, SDL_AppResult>)
                Marshal.GetFunctionPointerForDelegate(appInitFunc);
            var appIterateFuncDelegate = (delegate* unmanaged[Cdecl]<nint, SDL_AppResult>)
                Marshal.GetFunctionPointerForDelegate(appIterateFunc);
            var appEventFuncDelegate = (delegate* unmanaged[Cdecl]<nint, SDL_Event*, SDL_AppResult>)
                Marshal.GetFunctionPointerForDelegate(appEventFunc);
            var appQuitFuncDelegate = (delegate* unmanaged[Cdecl]<nint, SDL_AppResult, void>)
                Marshal.GetFunctionPointerForDelegate(appQuitFunc);

            try
            {
                var result = SDL.SDL3.SDL_EnterAppMainCallbacks(argc, argvPtrs,
                    appInitFuncDelegate, appIterateFuncDelegate, appEventFuncDelegate, appQuitFuncDelegate);

                GC.KeepAlive(appInitFunc);
                GC.KeepAlive(appIterateFunc);
                GC.KeepAlive(appEventFunc);
                GC.KeepAlive(appQuitFunc);

                return result;
            }
            finally
            {
                for (byte* argvPtr = *argvPtrs; argvPtr < argvPtrs + argv.Length; ++argvPtr)
                    NativeMemory.Free(argvPtr);
            }
        }

        public static SDLBool SDL_PushEvent(in SDL_Event sdlEvent)
        {
            fixed (SDL_Event* pEvent = &sdlEvent)
                return SDL.SDL3.SDL_PushEvent(pEvent);
        }

        public static SDL_IOStream* SDL_OpenIO(in SDL_IOStreamInterface ioInterface, nint userdata)
        {
            fixed (SDL_IOStreamInterface* pInterface = &ioInterface)
                return SDL.SDL3.SDL_OpenIO(pInterface, userdata);
        }
    }
}
