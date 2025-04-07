using LaunchpadReloaded.Options.Modifiers;
using LaunchpadReloaded.Options.Modifiers.Crewmate;
using MiraAPI.GameOptions;

namespace LaunchpadReloaded.Modifiers.Game.Crewmate;

public sealed class TorchModifier : LPModifier
{
    public override string ModifierName => "Torch";
    public override string Description => 
        OptionGroupSingleton<TorchOptions>.Instance.UseFlashlight
        ? "You will have a flashlight\nif lights are sabotaged."
        : "You have max vision\nif lights are sabotaged.";

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.TorchChance;
    public override int GetAmountPerGame() => 1;
}