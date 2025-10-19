using System;

namespace OpenDredmor.CommonInterfaces;

public abstract class BaseGame
{
    protected BaseVFS VFS { get; }
    protected TimeProvider TimeProvider { get; }
    protected BaseRenderer Renderer { get; }

    public BaseGame(BaseVFS vfs, BaseRenderer renderer, TimeProvider timeProvider)
    {
        VFS = vfs;
        Renderer = renderer;
        TimeProvider = timeProvider;

        Renderer.OnMouseClicked += (s, e) => OnMouseClicked(e.X, e.Y);
    }

    protected abstract void OnMouseClicked(float x, float y);

    public GameScene CurrentScene { get; set; }

    public void Run() => Renderer.Run();
}

public enum GameScene
{
    Intro,
    MainMenu,
    InGame,
}