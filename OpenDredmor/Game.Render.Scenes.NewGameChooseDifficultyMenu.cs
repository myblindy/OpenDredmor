using OpenDredmor.CommonInterfaces;

namespace OpenDredmor;

partial class Game
{
    void RenderSceneNewGameChooseDifficultyMenu()
    {
        Renderer.RenderSprites(
            new Sprite(@"ui/menus/bg_stonetile.png",
                new(0, 0, BaseRenderer.VirtualWidth, BaseRenderer.VirtualHeight), Tile: true),
            new Sprite(@"ui/menus/bg_paintingframe.png", new(544, 224, 832, 632)),

            new Sprite(@"ui/menus/topbar_horz_tile_bg.png",
                new(0, 0, BaseRenderer.VirtualWidth, 64), Tile: true),
            new Sprite(@"ui/menus/title_choosedifficulty.png",
                new(BaseRenderer.VirtualWidth / 2, 0, 0, 0), Anchor: SpriteAnchor.TopCenter));

        if (UI.ImageButton(@"ui/skillselect_back0.png", @"ui/skillselect_back1.png", @"ui/skillselect_back2.png",
            default))
        {
            CurrentScene = GameScene.MainMenu;
        }

        if (UI.ImageButton(@"ui/skillselect_done0.png", @"ui/skillselect_done1.png", @"ui/skillselect_done2.png",
            new(BaseRenderer.VirtualWidth, 0, 0, 0), SpriteAnchor.TopRight))
        {
        }

        Renderer.RenderSprites(
            new Sprite(@"ui/menus/panel_choosedifficulty107.png",
                new(730, 240, 0, 0)),
            new Sprite(@"ui/menus/panel_choosedifficulty_expansion.png",
                new(730, 240, 0, 0)));
    }
}
