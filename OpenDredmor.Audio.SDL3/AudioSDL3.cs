using OpenDredmor.CommonInterfaces;
using OpenDredmor.SDL3.Helpers;
using SDL;

namespace OpenDredmor.Audio.SDL3;

public class AudioSDL3(IGameLocation gameLocation) : BaseAudio
{
    bool isInitialized;
    unsafe MIX_Mixer* mixer;

    unsafe MIX_Track* bgmTrack;
    string? lastBGMPath;

    unsafe void EnsureInitialized()
    {
        if (isInitialized)
            return;

        SDL3_mixer.MIX_Init();
        mixer = SDL3_mixer.MIX_CreateMixerDevice((SDL_AudioDeviceID)uint.MaxValue, null);
        bgmTrack = SDL3_mixer.MIX_CreateTrack(mixer);

        isInitialized = true;
    }

    public override unsafe void PlayBGM(string? path)
    {
        EnsureInitialized();

        if (path == lastBGMPath)
            return;

        if (path is not null)
        {
            var stream = gameLocation.FileProvider.GetFileInfo(path).CreateReadStream();
            SDL3_mixer.MIX_SetTrackIOStream(bgmTrack, SDL3StreamManager.CreateSDLIOStreamFromStream(stream), true);

            var props = SDL.SDL3.SDL_CreateProperties();
            SDL.SDL3.SDL_SetNumberProperty(props, SDL3_mixer.MIX_PROP_PLAY_LOOPS_NUMBER, -1);
            SDL3_mixer.MIX_PlayTrack(bgmTrack, props);
            SDL.SDL3.SDL_DestroyProperties(props);
        }
        else
        {
            SDL3_mixer.MIX_StopTrack(bgmTrack, 0);
            SDL3_mixer.MIX_SetTrackIOStream(bgmTrack, null, false);
        }
        lastBGMPath = path;
    }
}
