using OpenDredmor.CommonInterfaces.Services.Interfaces;
using OpenDredmor.CommonInterfaces.Support;

namespace OpenDredmor.Services;

partial class Game
{
    int newGameSelectedCharacterIndex;
    string newGameSelectedCharacterName = "Mina";
    void RenderSceneNewGameNameMenu()
    {
        Rect2 portrait1Rect = new(847, 650, 0, 0),
            portrait2Rect = new(994, 650, 0, 0);

        Renderer.RenderSprites(
            new Sprite(@"ui/menus/bg_stonetile.png",
                new(0, 0, BaseRenderer.VirtualWidth, BaseRenderer.VirtualHeight), Tile: true),
            new Sprite(@"ui/menus/bg_paintingframe.png", new(544, 224, 0, 0)),
            new Sprite(@"ui/menus/menubg_sepia.png", new(561, 241, 0, 0)),
            new Sprite(@"ui/menus/panel_choosename.png", new(720, 360, 0, 0)),
            new Sprite(@"ui/menus/panel_choosehero.png", new(720, 560, 0, 0)),
            new Sprite(@"ui/portrait/portrait_100_stare.png", portrait1Rect,
                Alpha: newGameSelectedCharacterIndex == 0 ? 1f : 0.5f),
            new Sprite(@"ui/portrait/portraitf_100_stare.png", portrait2Rect,
                Alpha: newGameSelectedCharacterIndex == 1 ? 1f : 0.5f),

            new Sprite(@"ui/menus/topbar_horz_tile_bg.png",
                new(0, 0, BaseRenderer.VirtualWidth, 64), Tile: true),
            new Sprite(@"ui/menus/title_choosename.png",
                new(BaseRenderer.VirtualWidth / 2, 0, 0, 0), Anchor: SpriteAnchor.TopCenter));
        Renderer.RenderText(newGameSelectedCharacterName + "_", 35, new(804, 421, 308, 81), SpriteAnchor.LeftCenter);

        if (UI.InvisibleButton(portrait1Rect, referenceImage: @"ui/portrait/portrait_100_stare.png"))
        {
            newGameSelectedCharacterIndex = 0;
        }

        if (UI.InvisibleButton(portrait2Rect, referenceImage: @"ui/portrait/portraitf_100_stare.png"))
        {
            newGameSelectedCharacterIndex = 1;
        }

        if (UI.ImageButton(@"ui/skillselect_back0.png", @"ui/skillselect_back1.png", @"ui/skillselect_back2.png",
                default))
        {
            CurrentScene = GameScene.NewGameSkillSelectionMenu;
        }

        if (UI.ImageButton(@"ui/skillselect_done0.png", @"ui/skillselect_done1.png", @"ui/skillselect_done2.png",
            new(BaseRenderer.VirtualWidth, 0, 0, 0), SpriteAnchor.TopRight))
        {
        }

    }
}
