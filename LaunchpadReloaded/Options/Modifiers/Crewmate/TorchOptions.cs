using System;
using LaunchpadReloaded.Modifiers.Game.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

namespace LaunchpadReloaded.Options.Modifiers.Crewmate;

public class TorchOptions : AbstractOptionGroup<TorchModifier>
{
    public override string GroupName => "Torch";

    public override Func<bool> GroupVisible =>
        () => OptionGroupSingleton<CrewmateModifierOptions>.Instance.TorchChance > 0;

    [ModdedToggleOption("Use Hide N Seek Flashlight")]
    public bool UseFlashlight { get; set; } = true;

    public ModdedNumberOption TorchFlashlightSize { get; } = new("Flashlight Size", .25f, 0.1f, .5f, 0.05f, MiraNumberSuffixes.Multiplier)
    {
        Visible = () => OptionGroupSingleton<TorchOptions>.Instance.UseFlashlight,
    };
}