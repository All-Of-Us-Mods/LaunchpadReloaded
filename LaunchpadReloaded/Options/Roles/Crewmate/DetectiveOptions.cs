using LaunchpadReloaded.Roles.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;

namespace LaunchpadReloaded.Options.Roles.Crewmate;

public class DetectiveOptions : AbstractOptionGroup<LpDetectiveRole>
{
    public override string GroupName => "Detective";

    [ModdedToggleOption("Hide Suspects")]
    public bool HideSuspects { get; set; } = false;

    [ModdedNumberOption("Suspect Count", 2, 8, 1, MiraNumberSuffixes.None)]
    public float SuspectCount { get; set; } = 4;

    [ModdedNumberOption("Footsteps Duration", 1, 10, 1, MiraNumberSuffixes.Seconds)]
    public float FootstepsDuration { get; set; } = 3;

    [ModdedNumberOption("Instinct Duration", 3, 76, 3, MiraNumberSuffixes.Seconds)]
    public float InstinctDuration { get; set; } = 18;

    [ModdedNumberOption("Instinct Uses", 0, 10, zeroInfinity: true)]
    public float InstinctUses { get; set; } = 3;

    [ModdedNumberOption("Instinct Cooldown", 0, 45, 1, MiraNumberSuffixes.Seconds)]
    public float InstinctCooldown { get; set; } = 15;

}