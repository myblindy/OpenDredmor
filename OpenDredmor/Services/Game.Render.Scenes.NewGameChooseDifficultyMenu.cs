using OpenDredmor.CommonInterfaces.Services.Interfaces;

namespace OpenDredmor.Services;

partial class Game
{
    int newGameDifficulty = 1;
    bool newGamePermaDeath = true, newGameNoTimeToGrind, newGameDigDeeper = true;
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
            CurrentScene = GameScene.NewGameSkillSelectionMenu;
        }

        Renderer.RenderSprites(
            new Sprite(@"ui/menus/panel_choosedifficulty107.png",
                new(730, 240, 0, 0)),
            new Sprite(@"ui/menus/panel_choosedifficulty_expansion.png",
                new(730, 240, 0, 0)));

        // 3 difficulty buttons
        if (UI.InvisibleButton(new(810, 302, 305, 68)))
            newGameDifficulty = 0;
        if (UI.InvisibleButton(new(810, 373, 305, 68)))
            newGameDifficulty = 1;
        if (UI.InvisibleButton(new(810, 442, 305, 68)))
            newGameDifficulty = 2;
        Renderer.RenderSprites(new Sprite(@"ui/menus/difficulty_x.png",
            new(815, ((ReadOnlySpan<int>)[318, 382, 446])[newGameDifficulty], 0, 0)));

        // permadeath
        if (UI.InvisibleButton(new(804, 543, 317, 55)))
            newGamePermaDeath = !newGamePermaDeath;
        if (newGamePermaDeath)
            Renderer.RenderSprites(new Sprite(@"ui/menus/difficulty_x_small.png",
                new(812, 553, 0, 0)));

        // no time to grind
        if (UI.InvisibleButton(new(804, 637, 317, 55)))
            newGameNoTimeToGrind = !newGameNoTimeToGrind;
        if (newGameNoTimeToGrind)
            Renderer.RenderSprites(new Sprite(@"ui/menus/difficulty_x_small.png",
                new(812, 647, 0, 0)));

        // dig deeper
        if (UI.InvisibleButton(new(785, 729, 349, 55)))
            newGameDigDeeper = !newGameDigDeeper;
        if (newGameDigDeeper)
            Renderer.RenderSprites(new Sprite(@"ui/menus/difficulty_x_small.png",
                new(791, 743, 0, 0)));
    }
}
