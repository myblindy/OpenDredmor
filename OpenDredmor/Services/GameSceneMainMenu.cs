using OpenDredmor.CommonInterfaces.Models;
using OpenDredmor.CommonInterfaces.Services.Interfaces;

namespace OpenDredmor.Services;

sealed class GameSceneMainMenu(GameModel gameModel, BaseAudio audio, BaseRenderer renderer,
    BaseUI ui, BaseVFS vfs) : IGameScene
{
    public void RenderScene()
    {
        audio.PlayBGM("tunes/finaltitle.ogg");

        // background
        renderer.RenderSprites(
            new Sprite(@"ui/dredmor_main_big.png", new(0, 0, 1920, 1080),
                Expansion: gameModel.MainMenuBackgroundExpansion),
            new Sprite(@"ui/main_titletext.png", new(110, 20, 800, 600)));

        if (gameModel.MainMenuBackgroundExpansion == 3)
            renderer.RenderSprites(new Sprite(@"ui/cotw_title.png", new(75, 110, 820, 0)));
        else if (gameModel.MainMenuBackgroundExpansion == 1)
            renderer.RenderSprites(new Sprite(@"ui/rotdg_title_small2.png", new(75, 110, 820, 0)));

        renderer.RenderSprites(new Sprite(@"ui/menus/main_menubg.png", new(1200, 270, 304, 440)));

        // menu buttons
        if (ui.ImageButton("ui/menus/main_newgame0.png", "ui/menus/main_newgame1.png", "ui/menus/main_newgame2.png",
            new(1256, 334, 190, 32)))
        {
            gameModel.CurrentScene = GameScene.NewGameChooseDifficultyMenu;
        }
        if (ui.ImageButton("ui/menus/main_loadgame0.png", "ui/menus/main_loadgame1.png", "ui/menus/main_loadgame2.png",
            new(1256, 378, 190, 32)))
        {
        }
        if (ui.ImageButton("ui/menus/main_highscores0.png", "ui/menus/main_highscores1.png", "ui/menus/main_highscores2.png",
            new(1256, 422, 190, 32)))
        {
        }
        if (ui.ImageButton("menus/main_collections0.png", "menus/main_collections1.png", "menus/main_collections2.png",
            new(1256, 466, 190, 32)))
        {
        }
        if (ui.ImageButton("ui/menus/main_credits0.png", "ui/menus/main_credits1.png", "ui/menus/main_credits2.png",
            new(1256, 510, 190, 32)))
        {
        }
        if (ui.ImageButton("ui/menus/main_tutorial0.png", "ui/menus/main_tutorial1.png", "ui/menus/main_tutorial2.png",
            new(1256, 554, 190, 32)))
        {
        }
        if (ui.ImageButton("ui/menus/main_quit0.png", "ui/menus/main_quit1.png", "ui/menus/main_quit2.png",
            new(1256, 598, 190, 32)))
        {
        }
        if (ui.ImageButton("ui/menus/main_bg_button0.png", "ui/menus/main_bg_button1.png", "ui/menus/main_bg_button2.png",
            new(1336, 646, 32, 32)))
        {
            gameModel.MainMenuBackgroundExpansion = (gameModel.MainMenuBackgroundExpansion - 1 + vfs.ExpansionDirectoryNames.Length) % vfs.ExpansionDirectoryNames.Length;
        }
    }
}
