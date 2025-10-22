using OpenDredmor.CommonInterfaces;
using OpenDredmor.CommonInterfaces.Support;

namespace OpenDredmor;

partial class Game
{
    int newGameSkillGridRowOffset;
    readonly List<Skill> newGameSelectedSkills = [];
    readonly int newGameMaxSelectableSkills = 7;

    void RenderSceneNewGameSkillSelectionMenu()
    {
        Renderer.RenderSprites(
            new Sprite(@"ui/menus/bg_stonetile.png",
                new(0, 0, BaseRenderer.VirtualWidth, BaseRenderer.VirtualHeight), Tile: true),
            new Sprite(@"ui/menus/bg_paintingframe.png", new(544, 224, 832, 632)),
            new Sprite(@"ui/menus/skillchoose_skillinfo_bg.png", new(564, 721, 0, 0)),

            new Sprite(@"ui/menus/topbar_horz_tile_bg.png",
                new(0, 0, BaseRenderer.VirtualWidth, 64), Tile: true),
            new Sprite(@"ui/menus/title_chooseskills.png",
                new(BaseRenderer.VirtualWidth / 2, 0, 0, 0), Anchor: SpriteAnchor.TopCenter),
            new Sprite(@"ui/menus/selected_skills_panel.png", new(560, 620, 0, 0)),
            new Sprite(@"ui/menus/scroll_holder.png", new(1340, 338, 0, 0)));

        if (UI.ImageButton(@"ui/skillselect_back0.png", @"ui/skillselect_back1.png", @"ui/skillselect_back2.png",
            default))
        {
            CurrentScene = GameScene.NewGameChooseDifficultyMenu;
        }

        if (UI.ImageButton(@"ui/skillselect_done0.png", @"ui/skillselect_done1.png", @"ui/skillselect_done2.png",
            new(BaseRenderer.VirtualWidth, 0, 0, 0), SpriteAnchor.TopRight))
        {
            if (newGameSelectedSkills.Count == newGameMaxSelectableSkills)
                CurrentScene = GameScene.NewGameNameMenu;
        }

        const int gridStartX = 564, gridStartY = 242, gridOffsetX = 642 - gridStartX, gridOffsetY = 318 - gridStartY;
        const int gridRows = 5, gridCols = 10;

        if (UI.ImageButton(@"ui/menus/arrowbutton_up0.png", @"ui/menus/arrowbutton_up1.png", @"ui/menus/arrowbutton_up2.png",
            new(1354, 376, 0, 0)))
        {
            if (Math.Ceiling((double)GameData.SortedSkills.Length / gridCols) - gridRows - newGameSkillGridRowOffset <= 0)
                --newGameSkillGridRowOffset;
        }

        if (UI.ImageButton(@"ui/menus/arrowbutton_down0.png", @"ui/menus/arrowbutton_down1.png", @"ui/menus/arrowbutton_down2.png",
            new(1354, 436, 0, 0)))
        {
            if (Math.Ceiling((double)GameData.SortedSkills.Length / gridCols) - gridRows - newGameSkillGridRowOffset > 0)
                ++newGameSkillGridRowOffset;
        }

        // skill grid
        for (var row = 0; row < gridRows; row++)
            for (var col = 0; col < gridCols; col++)
            {
                var skillIndex = (newGameSkillGridRowOffset + row) * gridCols + col;
                if (skillIndex >= GameData.SortedSkills.Length)
                    break;

                var skill = GameData.SortedSkills[skillIndex];
                var isSkillSelected = newGameSelectedSkills.Contains(skill);
                var buttonRect = new Rect2(gridStartX + col * gridOffsetX, gridStartY + row * gridOffsetY, 0, 0);

                var alpha = isSkillSelected ? 1f : 0.5f;
                Renderer.RenderSprites(
                    new Sprite(skill.SpritePath, buttonRect.Inset(4), Alpha: alpha),
                    new Sprite(@"skills/skill_shadow.png", buttonRect, Alpha: alpha));
                if (UI.InvisibleButton(buttonRect, referenceImage: @"skills/skill_shadow.png"))
                {
                    if (isSkillSelected)
                        newGameSelectedSkills.Remove(skill);
                    else if (newGameSelectedSkills.Count < newGameMaxSelectableSkills)
                        newGameSelectedSkills.Add(skill);
                }

                if (UI.IsLastHovered)
                {
                    Renderer.RenderText(skill.Name, 35f, new(577, 736, 341, 90));
                    Renderer.RenderText(skill.Description, 16f, new(917, 736, 425, 90), SpriteAnchor.TopLeft);
                }
            }

        // selected skills
        const int selectedGridStartX = 579, selectedGridStartY = 640, selectedGridOffsetX = 667 - selectedGridStartX;
        List<Skill>? skillsToRemove = default;
        for (var col = 0; col < newGameSelectedSkills.Count; ++col)
            if (UI.ImageButton(newGameSelectedSkills[col].SpritePath, null, null,
                new(selectedGridStartX + col * selectedGridOffsetX, selectedGridStartY, 0, 0)))
            {
                (skillsToRemove ??= []).Add(newGameSelectedSkills[col]);
            }
        if (skillsToRemove is not null)
            foreach (var skillToRemove in skillsToRemove)
                newGameSelectedSkills.Remove(skillToRemove);

        if (UI.ImageButton(@"ui/menus/skills_last0.png", @"ui/menus/skills_last1.png", null,
            new(1278, 645, 0, 0)))
        {
        }

        if (UI.ImageButton(@"ui/menus/skills_random0.png", @"ui/menus/skills_random1.png", null,
            new(1198, 645, 0, 0)))
        {
            newGameSelectedSkills.Clear();
            newGameSelectedSkills.AddRange(GameData.SortedSkills.Shuffle().Take(newGameMaxSelectableSkills));
        }
    }
}
