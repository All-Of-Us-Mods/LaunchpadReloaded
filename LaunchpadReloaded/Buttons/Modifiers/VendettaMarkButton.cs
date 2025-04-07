using Il2CppSystem;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Options.Modifiers.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.PluginLoading;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace LaunchpadReloaded.Buttons.Modifiers;

[MiraIgnore]
public class VendettaMarkButton : BaseLaunchpadButton<PlayerControl>
{
    public override string Name => "Mark";
    public override float Cooldown => OptionGroupSingleton<VendettaOptions>.Instance.MarkCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<VendettaOptions>.Instance.MarkUses;
    
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.InjectButton; // needs a proper sprite
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => true;

    protected override void OnClick()
    {
        Target!.RpcAddModifier<VendettaMarkModifier>(PlayerControl.LocalPlayer.PlayerId);
        ResetTarget();
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return Button && Button!.gameObject.activeSelf;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, 1.1f);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return target && !target!.HasModifier<VendettaMarkModifier>();
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Nullable<Color>(Palette.Purple));
    }
}