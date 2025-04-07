using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using UnityEngine;

namespace LaunchpadReloaded.Options.Modifiers;

public class CrewmateModifierOptions : AbstractOptionGroup
{
    public override string GroupName => "Crewmate Modifiers";
    public override bool ShowInModifiersMenu => true;
    public override Color GroupColor => Palette.CrewmateRoleHeaderBlue;

    [ModdedNumberOption("Mayor Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float MayorChance { get; set; } = 0f;

    [ModdedNumberOption("Torch Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float TorchChance { get; set; } = 0f;

    [ModdedNumberOption("Vendetta Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float VendettaChance { get; set; } = 0f;
}