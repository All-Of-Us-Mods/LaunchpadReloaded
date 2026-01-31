using LaunchpadReloaded.Features;
using LaunchpadReloaded.Options.Roles.Impostor;
using LaunchpadReloaded.Roles.Impostor;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace LaunchpadReloaded.Buttons.Impostor;

public class SwapButton : BaseLaunchpadButton
{
    public override string Name => "Swap";
    public override float Cooldown => OptionGroupSingleton<SwapshifterOptions>.Instance.SwapCooldown;
    public override float EffectDuration => OptionGroupSingleton<SwapshifterOptions>.Instance.SwapDuration;
    public override int MaxUses => (int)OptionGroupSingleton<SwapshifterOptions>.Instance.SwapUses;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.SwapButton;
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => true;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is SwapshifterRole;
    }

    private PlayerControl? _currentTarget;

    public override void OnEffectEnd()
    {
        if (_currentTarget != null)
        {
            if (_currentTarget.Data.IsDead || _currentTarget.Data.Disconnected || PlayerControl.LocalPlayer.Data.IsDead)
            {
                PlayerControl.LocalPlayer.RpcShapeshift(PlayerControl.LocalPlayer, false);
                _currentTarget = null;
                return;
            }

            var currentPos2 = _currentTarget.transform.position;
            _currentTarget.NetTransform.RpcSnapTo(PlayerControl.LocalPlayer.transform.position);
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(currentPos2);
            PlayerControl.LocalPlayer.RpcShapeshift(PlayerControl.LocalPlayer, false);

            _currentTarget = null;
        }
    }

    protected override void OnClick()
    {
        SetTimerPaused(true);

        var playerMenu = CustomPlayerMenu.Create();
        playerMenu.Begin(plr => !plr.AmOwner && !plr.Data.IsDead && !plr.Data.Disconnected &&
        (!plr.Data.Role.IsImpostor || OptionGroupSingleton<SwapshifterOptions>.Instance.CanSwapImpostors), plr =>
        {
            SetTimerPaused(false);

            if (plr == null)
            {
                EffectActive = false;
                SetTimer(0);

                if (LimitedUses)
                {
                    UsesLeft++;
                    Button?.SetUsesRemaining(UsesLeft);
                }
                return;
            }

            _currentTarget = plr;

            var currentPos = _currentTarget.transform.position;

            _currentTarget.NetTransform.RpcSnapTo(PlayerControl.LocalPlayer.transform.position);
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(currentPos);

            PlayerControl.LocalPlayer.RpcShapeshift(_currentTarget, false);

            playerMenu.Close();
        });
    }
}