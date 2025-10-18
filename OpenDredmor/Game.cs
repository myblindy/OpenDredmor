using OpenDredmor.CommonInterfaces;

namespace OpenDredmor;

class Game : BaseGame
{
    public Game(BaseVFS vfs, BaseRenderer renderer, TimeProvider timeProvider)
        : base(vfs, renderer, timeProvider)
    {
        renderer.OnNewFrame += OnNewFrame;
    }

    private void OnNewFrame()
    {
        Renderer.RenderSprites([
            new(@"ui/dredmor_main_big.png", new(0, 0, Renderer.Width, Renderer.Height))
            ]);
    }
}
