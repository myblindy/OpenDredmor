using OpenDredmor.CommonInterfaces;

namespace OpenDredmor;

sealed partial class Game : BaseGame
{
    public Game(BaseVFS vfs, BaseRenderer renderer, BaseUI ui, BaseAudio audio, BaseGameData gameData, TimeProvider timeProvider)
        : base(vfs, renderer, ui, audio, gameData, timeProvider)
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
        else if (CurrentScene == GameScene.NewGameSkillSelectionMenu)
            RenderSceneNewGameSkillSelectionMenu();
        else if (CurrentScene == GameScene.NewGameNameMenu)
            RenderSceneNewGameNameMenu();
        else
            throw new NotImplementedException();
    }
}
