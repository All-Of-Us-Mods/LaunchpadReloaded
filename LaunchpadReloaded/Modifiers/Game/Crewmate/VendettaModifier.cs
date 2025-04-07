using LaunchpadReloaded.Buttons.Modifiers;
using LaunchpadReloaded.Options.Modifiers;
using LaunchpadReloaded.Options.Modifiers.Crewmate;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;

namespace LaunchpadReloaded.Modifiers.Game.Crewmate;

public sealed class VendettaModifier : LPModifier
{
    public override string ModifierName => "Vendetta";
    public override string Description =>
        $"You can mark " +
        $"{OptionGroupSingleton<VendettaOptions>.Instance.MarkUses} player{(OptionGroupSingleton<VendettaOptions>.Instance.MarkUses > 1 ? "s" : "")} per round\n" +
        $"If they vote you in the next meeting,\nthey will die in the next round.";

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.VendettaChance;
    public override int GetAmountPerGame() => 1;

    public override void OnActivate()
    {
        if (!Player.AmOwner) return;

        CustomButtonSingleton<VendettaMarkButton>.Instance.Button?.Show();
    }

    public override void OnDeactivate()
    {
        if (!Player.AmOwner) return;

        CustomButtonSingleton<VendettaMarkButton>.Instance.Button?.Hide();
    }

    [RegisterEvent]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro) return;

        if (PlayerControl.LocalPlayer.HasModifier<VendettaModifier>())
        {
            CustomButtonSingleton<VendettaMarkButton>.Instance.SetUses(CustomButtonSingleton<VendettaMarkButton>.Instance.MaxUses);
        }

        if (!PlayerControl.LocalPlayer.IsHost()) return;

        var victims = ModifierUtils.GetActiveModifiers<VendettaMarkModifier>();
        foreach (var mod in victims)
        {
            mod.Player.RpcRemoveModifier<VendettaMarkModifier>();
            var voteData = mod.Player.GetVoteData();
            if (voteData && voteData.VotedFor(mod.Vendetta.PlayerId))
            {
                mod.Vendetta.RpcCustomMurder(mod.Player, teleportMurderer: false);
            }
        }
    }
}