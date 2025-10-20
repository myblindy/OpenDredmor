using OpenDredmor.CommonInterfaces;

namespace OpenDredmor;

class Game : BaseGame
{
    readonly BaseAudio audio;

    public Game(BaseVFS vfs, BaseRenderer renderer, BaseAudio audio, TimeProvider timeProvider)
        : base(vfs, renderer, timeProvider)
    {
        renderer.OnNewFrame += OnNewFrame;
        this.audio = audio;
    }

    protected override void OnMouseClicked(float x, float y)
    {
        if (CurrentScene == GameScene.MainMenu)
        {
        }

        throw new NotImplementedException();
    }

    private void OnNewFrame()
    {
        if (CurrentScene == GameScene.MainMenu)
        {
            audio.PlayBGM("tunes/finaltitle.ogg");
            Renderer.RenderSprites([
                new(@"ui/dredmor_main_big.png", new(0, 0, Renderer.Width, Renderer.Height))
                ]);
        }
        else
            throw new NotImplementedException();
    }
}
