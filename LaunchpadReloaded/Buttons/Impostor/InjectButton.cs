using Il2CppSystem;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Options;
using LaunchpadReloaded.Options.Roles.Impostor;
using LaunchpadReloaded.Roles.Impostor;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace LaunchpadReloaded.Buttons.Impostor;

public class InjectButton : BaseLaunchpadButton<PlayerControl>
{
    public override string Name => "Inject";
    public override float Cooldown => OptionGroupSingleton<SurgeonOptions>.Instance.InjectCooldown;
    public override float EffectDuration => OptionGroupSingleton<SurgeonOptions>.Instance.PoisonDelay;
    public override int MaxUses => (int)OptionGroupSingleton<SurgeonOptions>.Instance.InjectUses;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.InjectButton;
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => false;

    public override bool Enabled(RoleBehaviour? role) => role is SurgeonRole;

    private PlayerControl? _injectedPlayer;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(OptionGroupSingleton<FunOptions>.Instance.FriendlyFire, 1.1f);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return target != null && !target.HasModifier<PoisonModifier>();
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Nullable<Color>(LaunchpadPalette.SurgeonColor));
    }

    public override bool CanUse()
    {
        return base.CanUse() && _injectedPlayer == null;
    }

    public override void OnEffectEnd()
    {
        if (_injectedPlayer is null
            || _injectedPlayer.Data.IsDead || PlayerControl.LocalPlayer.Data.IsDead
            || (_injectedPlayer.Data.Role.IsImpostor && !OptionGroupSingleton<FunOptions>.Instance.FriendlyFire) || !_injectedPlayer.HasModifier<PoisonModifier>()
            || MeetingHud.Instance != null)
        {
            _injectedPlayer = null;
            return;
        }

        _injectedPlayer.RpcRemoveModifier<PoisonModifier>();
        PlayerControl.LocalPlayer.RpcCustomMurder(_injectedPlayer, resetKillTimer: false, createDeadBody: true, teleportMurderer: false, showKillAnim: false);
        _injectedPlayer = null;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        _injectedPlayer = Target;
        _injectedPlayer.RpcAddModifier<PoisonModifier>();
        SoundManager.Instance.PlaySound(LaunchpadAssets.InjectSound.LoadAsset(), false, volume: 3);

        ResetTarget();
    }
}