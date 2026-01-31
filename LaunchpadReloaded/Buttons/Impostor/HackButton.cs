using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Options.Roles.Impostor;
using LaunchpadReloaded.Roles.Impostor;
using LaunchpadReloaded.Utilities;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace LaunchpadReloaded.Buttons.Impostor;

public class HackButton : BaseLaunchpadButton
{
    public override string Name => "HACK";
    public override float Cooldown => (int)OptionGroupSingleton<HackerOptions>.Instance.HackCooldown;
    public override float EffectDuration => OptionGroupSingleton<HackerOptions>.Instance.HackDuration;
    public override int MaxUses => (int)OptionGroupSingleton<HackerOptions>.Instance.HackUses;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.HackButton;
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => false;
    public override bool Enabled(RoleBehaviour? role) => role is HackerRole;
    public override bool CanUse() => base.CanUse() && !HackerUtilities.AnyPlayerHacked();

    protected override void OnClick()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.Data.IsDead || player.Data.Disconnected)
            {
                continue;
            }

            player.RpcAddModifier<HackedModifier>();
        }

        PlayerControl.LocalPlayer.RawSetColor(15);
    }
}