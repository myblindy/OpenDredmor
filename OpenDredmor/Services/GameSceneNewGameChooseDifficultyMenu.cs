using OpenDredmor.CommonInterfaces.Models;
using OpenDredmor.CommonInterfaces.Services.Interfaces;

namespace OpenDredmor.Services;

sealed class GameSceneNewGameChooseDifficultyMenu(GameModel gameModel, BaseRenderer renderer, BaseUI ui) : IGameScene
{
    public void RenderScene()
    {
        renderer.RenderSprites(
            new Sprite(@"ui/menus/bg_stonetile.png",
                new(0, 0, BaseRenderer.VirtualWidth, BaseRenderer.VirtualHeight), Tile: true),
            new Sprite(@"ui/menus/bg_paintingframe.png", new(544, 224, 832, 632)),

            new Sprite(@"ui/menus/topbar_horz_tile_bg.png",
                new(0, 0, BaseRenderer.VirtualWidth, 64), Tile: true),
            new Sprite(@"ui/menus/title_choosedifficulty.png",
                new(BaseRenderer.VirtualWidth / 2, 0, 0, 0), Anchor: SpriteAnchor.TopCenter));

        if (ui.ImageButton(@"ui/skillselect_back0.png", @"ui/skillselect_back1.png", @"ui/skillselect_back2.png",
            default))
        {
            gameModel.CurrentScene = GameScene.MainMenu;
        }

        if (ui.ImageButton(@"ui/skillselect_done0.png", @"ui/skillselect_done1.png", @"ui/skillselect_done2.png",
            new(BaseRenderer.VirtualWidth, 0, 0, 0), SpriteAnchor.TopRight))
        {
            gameModel.CurrentScene = GameScene.NewGameSkillSelectionMenu;
        }

        renderer.RenderSprites(
            new Sprite(@"ui/menus/panel_choosedifficulty107.png",
                new(730, 240, 0, 0)),
            new Sprite(@"ui/menus/panel_choosedifficulty_expansion.png",
                new(730, 240, 0, 0)));

        // 3 difficulty buttons
        if (ui.InvisibleButton(new(810, 302, 305, 68)))
            gameModel.MainMenuNewGameDifficulty = 0;
        if (ui.InvisibleButton(new(810, 373, 305, 68)))
            gameModel.MainMenuNewGameDifficulty = 1;
        if (ui.InvisibleButton(new(810, 442, 305, 68)))
            gameModel.MainMenuNewGameDifficulty = 2;
        renderer.RenderSprites(new Sprite(@"ui/menus/difficulty_x.png",
            new(815, ((ReadOnlySpan<int>)[318, 382, 446])[gameModel.MainMenuNewGameDifficulty], 0, 0)));

        // permadeath
        if (ui.InvisibleButton(new(804, 543, 317, 55)))
            gameModel.MainMenuNewGamePermaDeath = !gameModel.MainMenuNewGamePermaDeath;
        if (gameModel.MainMenuNewGamePermaDeath)
            renderer.RenderSprites(new Sprite(@"ui/menus/difficulty_x_small.png",
                new(812, 553, 0, 0)));

        // no time to grind
        if (ui.InvisibleButton(new(804, 637, 317, 55)))
            gameModel.MainMenuNewGameNoTimeToGrind = !gameModel.MainMenuNewGameNoTimeToGrind;
        if (gameModel.MainMenuNewGameNoTimeToGrind)
            renderer.RenderSprites(new Sprite(@"ui/menus/difficulty_x_small.png",
                new(812, 647, 0, 0)));

        // dig deeper
        if (ui.InvisibleButton(new(785, 729, 349, 55)))
            gameModel.MainMenuNewGameDigDeeper = !gameModel.MainMenuNewGameDigDeeper;
        if (gameModel.MainMenuNewGameDigDeeper)
            renderer.RenderSprites(new Sprite(@"ui/menus/difficulty_x_small.png",
                new(791, 743, 0, 0)));
    }
}
