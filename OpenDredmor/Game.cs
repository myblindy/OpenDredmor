using OpenDredmor.CommonInterfaces;

namespace OpenDredmor;

sealed partial class Game : BaseGame
{
    int mainMenuBackgroundExpansion;

    public Game(BaseVFS vfs, BaseRenderer renderer, BaseUI ui, BaseAudio audio, TimeProvider timeProvider)
        : base(vfs, renderer, ui, audio, timeProvider)
    {
        renderer.OnNewFrame += OnNewFrame;
        mainMenuBackgroundExpansion = vfs.ExpansionDirectoryNames.Length - 1;
    }

    protected override void OnMouseClicked(float x, float y)
    {
    }

    void OnNewFrame()
    {
        if (CurrentScene == GameScene.MainMenu)
            RenderSceneMainMenu();
        else if (CurrentScene == GameScene.NewGameChooseDifficultyMenu)
            RenderSceneNewGameChooseDifficultyMenu();
        else
            throw new NotImplementedException();
    }
}
