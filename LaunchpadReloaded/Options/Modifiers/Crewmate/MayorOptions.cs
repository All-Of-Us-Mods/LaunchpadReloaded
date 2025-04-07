using System;
using LaunchpadReloaded.Modifiers.Game.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;

namespace LaunchpadReloaded.Options.Modifiers.Crewmate;

public class MayorOptions : AbstractOptionGroup<MayorModifier>
{
    public override string GroupName => "Mayor";

    public override Func<bool> GroupVisible =>
        () => OptionGroupSingleton<CrewmateModifierOptions>.Instance.MayorChance > 0;

    [ModdedNumberOption("Extra Votes", 1, 3)]
    public float ExtraVotes { get; set; } = 1;

    [ModdedToggleOption("Allow Multiple Votes on Same Player")]
    public bool AllowVotingTwice { get; set; } = true;
}