using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Options.Roles.Crewmate;
using LaunchpadReloaded.Roles.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using System.Linq;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using Rewired;
using UnityEngine;

namespace LaunchpadReloaded.Buttons.Crewmate;

public class InstinctButton : BaseLaunchpadButton
{
    public override string Name => "INSTINCT";
    public override float Cooldown => OptionGroupSingleton<DetectiveOptions>.Instance.InstinctCooldown;
    public override float EffectDuration => OptionGroupSingleton<DetectiveOptions>.Instance.InstinctDuration;
    public override int MaxUses => (int)OptionGroupSingleton<DetectiveOptions>.Instance.InstinctUses;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.InstinctButton;
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => true;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is LpDetectiveRole;
    }

    public override void OnEffectEnd()
    {
        foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(plr => plr.HasModifier<FootstepsModifier>()))
        {
            player.GetModifierComponent().RemoveModifier<FootstepsModifier>();
        }
    }

    protected override void OnClick()
    {
        foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(plr => !plr.Data.IsDead))
        {
            player.GetModifierComponent().AddModifier<FootstepsModifier>();
        }
    }
}