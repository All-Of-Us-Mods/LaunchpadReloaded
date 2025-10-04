using LaunchpadReloaded.Features;
using LaunchpadReloaded.Networking.Roles;
using LaunchpadReloaded.Options.Roles.Crewmate;
using LaunchpadReloaded.Roles.Crewmate;
using LaunchpadReloaded.Utilities;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Rewired;
using UnityEngine;
using Helpers = LaunchpadReloaded.Utilities.Helpers;

namespace LaunchpadReloaded.Buttons.Crewmate;

public class SealButton : BaseLaunchpadButton<Vent>
{
    public override string Name => "Seal";
    public override float Cooldown => OptionGroupSingleton<SealerOptions>.Instance.SealVentCooldown;
    public override float EffectDuration => 0;
    public override int MaxUses => (int)OptionGroupSingleton<SealerOptions>.Instance.SealVentUses;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.SealButton;
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => false;
    public override float Distance => 1f;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is SealerRole;
    }

    public override Vent? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance, Helpers.BodyFilter2D);
    }

    public override bool IsTargetValid(Vent? target)
    {
        return target != null && target.enabled &&
            !PlayerControl.LocalPlayer.MustCleanVent(target.Id)
            && !target.IsSealed();
    }

    public override void SetOutline(bool active)
    {
        Target?.myRend.UpdateOutline(active ? PlayerControl.LocalPlayer.Data.Role.NameColor : null);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        SoundManager.Instance.PlaySound(LaunchpadAssets.TapeSound.LoadAsset(), false, volume: 2);
        PlayerControl.LocalPlayer.RpcSealVent(Target.Id);

        ResetTarget();
    }
}