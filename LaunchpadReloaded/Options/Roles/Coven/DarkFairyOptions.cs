using LaunchpadReloaded.Roles.Coven;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;

namespace LaunchpadReloaded.Options.Roles.Coven;

public class DarkFairyOptions : AbstractOptionGroup<DarkFairyRole>
{
    public override string GroupName => "Dark Fairy";

    [ModdedNumberOption("Darken Cooldown", 0, 120, 5, MiraNumberSuffixes.Seconds)]
    public float CharmCooldown { get; set; } = 45;

    [ModdedNumberOption("Max Darken Uses", 1, 5)]
    public float CharmCount { get; set; } = 3;
}