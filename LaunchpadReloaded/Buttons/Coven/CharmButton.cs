using Il2CppSystem;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Options.Roles.Coven;
using LaunchpadReloaded.Roles.Coven;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace LaunchpadReloaded.Buttons.Coven;

public class CharmButton : BaseLaunchpadButton<PlayerControl>
{
    public override string Name => "DARKEN";
    public override float Cooldown => OptionGroupSingleton<DarkFairyOptions>.Instance.CharmCooldown;
    public override float EffectDuration => 0;
    public override int MaxUses => (int)OptionGroupSingleton<DarkFairyOptions>.Instance.CharmCount;
    public override Color TextOutlineColor => new Color32(31, 0, 110, 255);
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.DarkFairyButton;
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => true;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is DarkFairyRole;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, 1.1f);
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Nullable<Color>(LaunchpadPalette.DarkFairyColor));
    }

    protected override void OnClick()
    {
        
    }
}
