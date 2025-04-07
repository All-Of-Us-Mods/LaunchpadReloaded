using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;

namespace LaunchpadReloaded.Options.Modifiers;

public class UniversalModifierOptions : AbstractOptionGroup
{
    public override string GroupName => "Universal Modifiers";
    public override bool ShowInModifiersMenu => true;
    public override uint GroupPriority => 1;

    [ModdedNumberOption("Giant Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float GiantChance { get; set; } = 0f;

    [ModdedNumberOption("Smol Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float SmolChance { get; set; } = 0f;

    [ModdedNumberOption("Gravity Field Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float GravityChance { get; set; } = 0f;
}