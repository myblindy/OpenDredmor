using Microsoft.Extensions.Hosting;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace OpenDredmor.CommonInterfaces;

public abstract class BaseGameData(BaseVFS vfs) : IHostedService
{
    protected BaseVFS VFS { get; } = vfs;

    public FrozenDictionary<string, Skill> Skills { get; protected set; } = null!;
    public ImmutableArray<Skill> SortedSkills { get; protected set; }

    protected abstract Task LoadAllDataAsync();

    public Task StartAsync(CancellationToken cancellationToken) =>
        LoadAllDataAsync();

    public Task StopAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public enum SkillType { Warrior, Rogue, Wizard }

public class Skill
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string AchievementTag { get; init; }
    public required int Id { get; init; }
    public required SkillType Type { get; init; }
    public required string SpritePath { get; init; }
}