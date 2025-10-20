using OpenDredmor.CommonInterfaces;

namespace OpenDredmor;

sealed class Game : BaseGame
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
            return;
        }

        throw new NotImplementedException();
    }

    void OnNewFrame()
    {
        if (CurrentScene == GameScene.MainMenu)
        {
            audio.PlayBGM("tunes/finaltitle.ogg");
            Renderer.RenderSprites([
                new(@"ui/dredmor_main_big.png", new(0, 0, 1920, 1080)),
                new(@"ui/main_titletext.png", new(110, 20, 800, 600)),
                new(@"ui/cotw_title.png", new(75, 110, 820, 0)),
                new(@"ui/menus/main_menubg.png", new(1200, 270, 304, 440)),

                new(@"ui/menus/main_newgame1.png",new(1256, 334, 190, 32)),
                new(@"ui/menus/main_loadgame1.png",new(1256, 378, 190, 32)),
                new(@"ui/menus/main_highscores1.png",new(1256, 422, 190, 32)),
                new(@"menus/main_collections1.png",new(1256, 466, 190, 32)),
                new(@"ui/menus/main_credits1.png",new(1256, 510, 190, 32)),
                new(@"ui/menus/main_tutorial1.png",new(1256, 554, 190, 32)),
                new(@"ui/menus/main_quit1.png",new(1256, 598, 190, 32)),

                new(@"ui/menus/main_bg_button1.png", new(1336, 646, 32, 32)),
                ]);
        }
        else
            throw new NotImplementedException();
    }
}
