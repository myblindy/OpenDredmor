using OpenDredmor.CommonInterfaces;

namespace OpenDredmor;

class Game : BaseGame
{
    public Game(BaseVFS vfs, BaseRenderer renderer, TimeProvider timeProvider)
        : base(vfs, renderer, timeProvider)
    {
        renderer.OnNewFrame += OnNewFrame;
    }

    protected override void OnMouseClicked(float x, float y)
    {
        if (CurrentScene == GameScene.Intro)
        {
            CurrentScene = GameScene.MainMenu;
            return;
        }

        throw new NotImplementedException();
    }

    private void OnNewFrame()
    {
        if (CurrentScene == GameScene.Intro)
        {
            Renderer.RenderSprites([
                new(@"ui/dredmor_main_big.png", new(0, 0, Renderer.Width, Renderer.Height))
                ]);
        }
        else if (CurrentScene == GameScene.MainMenu)
        {

        }
        else
            throw new NotImplementedException();
    }
}
