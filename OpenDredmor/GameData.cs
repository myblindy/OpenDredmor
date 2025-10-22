using Microsoft.Extensions.Logging;
using OpenDredmor.CommonInterfaces;
using System.Collections.Frozen;
using System.Xml.Linq;

namespace OpenDredmor;

sealed class GameData(BaseVFS vfs, ILogger<GameData> logger) : BaseGameData(vfs)
{
    static IList<Skill> ParseSkills(Stream stream) => [..
        XDocument.Load(stream).Root!
            .Elements("skill")
            .Where(x => x.Attribute("deprecated") is null)
            .Select(x => new Skill
            {
                Name = (string)x.Attribute("name")!,
                Description = (string)x.Attribute("description")!,
                AchievementTag = (string)x.Attribute("achievement")!,
                Id = (int)x.Attribute("id")!,
                Type = (string)x.Attribute("type")! switch
                {
                    "warrior" => SkillType.Warrior,
                    "rogue" => SkillType.Rogue,
                    "wizard" => SkillType.Wizard,
                    _ => throw new InvalidDataException($"Unknown skill type: {x.Attribute("type")!}"),
                },
                SpritePath = (string)x.Element("art")!.Attribute("icon")!,
            })];

    protected override async Task LoadAllDataAsync()
    {
        var skillDbStreams = VFS.OpenAllExpansionStreams("game/skillDB.xml", reverse: false);
        var skillExpansionSets = await Task.WhenAll(skillDbStreams
            .Select(async stream => await Task.Run(() => ParseSkills(stream))));
        SortedSkills = [.. skillExpansionSets
            .SelectMany(set => set)
            .DistinctBy(skill => skill.Name, StringComparer.OrdinalIgnoreCase)];
        Skills = SortedSkills.ToFrozenDictionary(skill => skill.Name, StringComparer.OrdinalIgnoreCase);
        foreach (var stream in skillDbStreams)
            stream.Dispose();
        logger.LogInformation("Loaded {SkillCount} skills.", Skills.Count);
    }
}
