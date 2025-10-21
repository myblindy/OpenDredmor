using SDL;

namespace OpenDredmor.SDL3.Helpers;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "SDL Naming")]
public static unsafe class SDL3MixerHelpers
{
    extension(SDL3_mixer)
    {
        public static MIX_Mixer* MIX_CreateMixerDevice(in SDL_AudioSpec spec)
        {
            fixed (SDL_AudioSpec* pSpec = &spec)
                return SDL3_mixer.MIX_CreateMixerDevice((SDL_AudioDeviceID)uint.MaxValue, pSpec);
        }

        public static MIX_Audio* MIX_LoadAudio_IO(MIX_Mixer* mixer, in SDL_IOStream stream, 
            SDLBool preDecode, SDLBool closeIO)
        {
            fixed(SDL_IOStream* pStream = &stream)
                return SDL3_mixer.MIX_LoadAudio_IO(mixer, pStream, preDecode, closeIO);
        }
    }
}
