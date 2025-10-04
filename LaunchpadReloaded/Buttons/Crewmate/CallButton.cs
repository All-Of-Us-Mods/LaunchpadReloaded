using LaunchpadReloaded.Features;
using LaunchpadReloaded.Options.Roles.Crewmate;
using LaunchpadReloaded.Roles.Crewmate;
using LaunchpadReloaded.Utilities;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Utilities.Assets;
using Rewired;
using UnityEngine;

namespace LaunchpadReloaded.Buttons.Crewmate;

public class CallButton : BaseLaunchpadButton
{
    public override string Name => "CALL";
    public override float Cooldown => OptionGroupSingleton<CaptainOptions>.Instance.CaptainMeetingCooldown;
    public override float EffectDuration => 0;
    public override int MaxUses => (int)OptionGroupSingleton<CaptainOptions>.Instance.CaptainMeetingCount;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.CallButton;
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => true;

    public override BaseKeybind Keybind => MiraGlobalKeybinds.SecondaryAbility;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is CaptainRole;
    }
    public override bool CanUse() =>
        base.CanUse()
        && !CustomButtonSingleton<ZoomButton>.Instance.EffectActive
        && !HackerUtilities.AnyPlayerHacked();

    public override void ClickHandler()
    {
        if (!CanUse())
        {
            return;
        }

        OnClick();
        Button?.SetDisabled();
    }

    protected override void OnClick()
    {
        var bt = ShipStatus.Instance.EmergencyButton;

        PlayerControl.LocalPlayer.NetTransform.Halt();
        var minigame = Object.Instantiate(bt.MinigamePrefab, Camera.main!.transform, false);

        var taskAdderGame = minigame as TaskAdderGame;
        if (taskAdderGame != null)
        {
            taskAdderGame.SafePositionWorld = bt.SafePositionLocal + (Vector2)bt.transform.position;
        }

        minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
        minigame.Begin(null);
    }
}