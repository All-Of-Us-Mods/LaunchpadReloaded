using Il2CppSystem;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Networking;
using UnityEngine;

namespace LaunchpadReloaded.Modifiers;

public class BaitModifier : GameModifier
{
    public override string ModifierName => "Bait";
    public override int GetAmountPerGame()
    {
        return 15; //I was too lazy to add an option group with values for these
    }
    public override int GetAssignmentChance()
    {
        return 100; //I was too lazy to add an option group with values for these
    }

    [RegisterEvent]
    public static void OnKill(AfterMurderEvent @event)
    {
        if (@event.Target.HasModifier<BaitModifier>()) @event.Source.CmdReportDeadBody(@event.Target.Data);
    }
}
