using LaunchpadReloaded.Modules.Localization;
using LaunchpadReloaded.Roles.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;

namespace LaunchpadReloaded.Options.Roles.Crewmate;

public class CaptainOptions : AbstractOptionGroup<CaptainRole>
{
    public override string GroupName => LaunchpadLocale.GetParsed("RoleCaptainName");

    [ModdedNumberOption("Meeting Cooldown", 0, 120, 5, MiraNumberSuffixes.Seconds)]
    public float CaptainMeetingCooldown { get; set; } = 45;

    [ModdedNumberOption("Meeting Uses", 1, 5)]
    public float CaptainMeetingCount { get; set; } = 3;

    [ModdedNumberOption("Zoom Cooldown", 5, 60, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ZoomCooldown { get; set; } = 30;

    [ModdedNumberOption("Zoom Duration", 5, 25, 1, MiraNumberSuffixes.Seconds)]
    public float ZoomDuration { get; set; } = 10;

    [ModdedNumberOption("Zoom Distance", 4, 15)]
    public float ZoomDistance { get; set; } = 6;
}