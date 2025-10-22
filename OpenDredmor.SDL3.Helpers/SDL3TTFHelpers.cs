using SDL;

namespace OpenDredmor.SDL3.Helpers;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "SDL Naming")]
public static unsafe class SDL3TTFHelpers
{
    extension(SDL3_ttf)
    {
        public static unsafe SDLBool TTF_GetTextSize(TTF_Text* text, out int width, out int height)
        {
            fixed (int* pWidth = &width)
            fixed (int* pHeight = &height)
                return SDL3_ttf.TTF_GetTextSize(text, pWidth, pHeight);
        }
    }
}
