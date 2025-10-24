using OpenDredmor.CommonInterfaces.Models;
using OpenDredmor.CommonInterfaces.Services.Interfaces;
using OpenDredmor.CommonInterfaces.Support;

namespace OpenDredmor.Services;

sealed class GameSceneNewGameNameMenu(GameModel gameModel, BaseRenderer renderer, BaseUI ui) : IGameScene
{
    public void RenderScene()
    {
        Rect2 portrait1Rect = new(847, 650, 0, 0),
            portrait2Rect = new(994, 650, 0, 0);

        renderer.RenderSprites(
            new Sprite(@"ui/menus/bg_stonetile.png",
                new(0, 0, BaseRenderer.VirtualWidth, BaseRenderer.VirtualHeight), Tile: true),
            new Sprite(@"ui/menus/bg_paintingframe.png", new(544, 224, 0, 0)),
            new Sprite(@"ui/menus/menubg_sepia.png", new(561, 241, 0, 0)),
            new Sprite(@"ui/menus/panel_choosename.png", new(720, 360, 0, 0)),
            new Sprite(@"ui/menus/panel_choosehero.png", new(720, 560, 0, 0)),
            new Sprite(@"ui/portrait/portrait_100_stare.png", portrait1Rect,
                Alpha: gameModel.MainMenuNewGameSelectedCharacterIndex == 0 ? 1f : 0.5f),
            new Sprite(@"ui/portrait/portraitf_100_stare.png", portrait2Rect,
                Alpha: gameModel.MainMenuNewGameSelectedCharacterIndex == 1 ? 1f : 0.5f),

            new Sprite(@"ui/menus/topbar_horz_tile_bg.png",
                new(0, 0, BaseRenderer.VirtualWidth, 64), Tile: true),
            new Sprite(@"ui/menus/title_choosename.png",
                new(BaseRenderer.VirtualWidth / 2, 0, 0, 0), Anchor: SpriteAnchor.TopCenter));
        renderer.RenderText(gameModel.MainMenuNewGameSelectedCharacterName + "_", 35, new(804, 421, 308, 81), SpriteAnchor.LeftCenter);

        if (ui.InvisibleButton(portrait1Rect, referenceImage: @"ui/portrait/portrait_100_stare.png"))
        {
            gameModel.MainMenuNewGameSelectedCharacterIndex = 0;
        }

        if (ui.InvisibleButton(portrait2Rect, referenceImage: @"ui/portrait/portraitf_100_stare.png"))
        {
            gameModel.MainMenuNewGameSelectedCharacterIndex = 1;
        }

        if (ui.ImageButton(@"ui/skillselect_back0.png", @"ui/skillselect_back1.png", @"ui/skillselect_back2.png",
                default))
        {
            gameModel.CurrentScene = GameScene.NewGameSkillSelectionMenu;
        }

        if (ui.ImageButton(@"ui/skillselect_done0.png", @"ui/skillselect_done1.png", @"ui/skillselect_done2.png",
            new(BaseRenderer.VirtualWidth, 0, 0, 0), SpriteAnchor.TopRight))
        {
        }
    }
}
