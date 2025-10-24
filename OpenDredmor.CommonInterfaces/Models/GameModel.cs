using OpenDredmor.CommonInterfaces.Services.Interfaces;

namespace OpenDredmor.CommonInterfaces.Models;

public class GameModel
{
    public GameScene CurrentScene { get; set; }
    public int MainMenuBackgroundExpansion { get; set; }
    public int MainMenuNewGameDifficulty { get; set; } = 1;
    public bool MainMenuNewGamePermaDeath { get; set; } = true;
    public bool MainMenuNewGameNoTimeToGrind { get; set; }
    public bool MainMenuNewGameDigDeeper { get; set; } = true;
    public int MainMenuNewGameSelectedCharacterIndex { get; set; }
    public string MainMenuNewGameSelectedCharacterName { get; set; } = "Mina";
    public int MainMenuNewGameSkillGridRowOffset { get; set; }
    public List<Skill> MainMenuNewGameSelectedSkills { get; } = [];
    public int MainMenuNewGameMaxSelectableSkills { get; } = 7;

}

public enum GameScene
{
    MainMenu,
    NewGameChooseDifficultyMenu,
    NewGameSkillSelectionMenu,
    NewGameNameMenu,
    InGame,
}