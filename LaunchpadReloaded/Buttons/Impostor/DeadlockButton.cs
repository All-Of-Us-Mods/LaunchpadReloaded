using AmongUs.Data;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Options.Roles.Impostor;
using LaunchpadReloaded.Roles.Impostor;
using LaunchpadReloaded.Utilities;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using Rewired;
using UnityEngine;

namespace LaunchpadReloaded.Buttons.Impostor;

public class DeadlockButton : BaseLaunchpadButton
{
    public override string Name => "Deadlock";
    public override float Cooldown => (int)OptionGroupSingleton<HitmanOptions>.Instance.DeadlockCooldown;
    public override float EffectDuration => OptionGroupSingleton<HitmanOptions>.Instance.DeadlockDuration;
    public override int MaxUses => (int)OptionGroupSingleton<HitmanOptions>.Instance.DeadlockUses;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.DeadlockButton;
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => false;
    
    public override bool Enabled(RoleBehaviour? role) => role is HitmanRole;

    protected override void OnClick()
    {
        var hitman = PlayerControl.LocalPlayer.Data.Role.TryCast<HitmanRole>()!;
        if (hitman == null)
        {
            return;
        }

        HitmanUtilities.Initialize();

        hitman.inDeadlockMode = true;
        hitman.overlay?.gameObject.SetActive(true);
        hitman.StartTransition(hitman.OverlayColor);

        DataManager.Settings.Gameplay.ScreenShake = true;
        HudManager.Instance?.AlertFlash?.SetOverlay(true);

        SoundManager.Instance.StopSound(LaunchpadAssets.DeadlockFadeIn.LoadAsset());
        SoundManager.Instance.PlaySound(LaunchpadAssets.DeadlockFadeIn.LoadAsset(), false);

        SoundManager.instance.StopSound(LaunchpadAssets.DeadlockAmbience.LoadAsset());
        SoundManager.Instance.PlaySound(LaunchpadAssets.DeadlockAmbience.LoadAsset(), true, 1f, SoundManager.Instance.sfxMixer);
    }

    public override void OnEffectEnd()
    {
        if (HitmanUtilities.ClockTick != null)
        {
            Coroutines.Stop(HitmanUtilities.ClockTick);
            HitmanUtilities.ClockTick = null;
        }

        if (HitmanUtilities.OverlayTick != null)
        {
            Coroutines.Stop(HitmanUtilities.OverlayTick);
            HitmanUtilities.OverlayTick = null;
        }

        if (!HudManager.InstanceExists)
        {
            return;
        }

        var hitman = PlayerControl.LocalPlayer.Data.Role.TryCast<HitmanRole>()!;
        if (hitman == null)
        {
            return;
        }

        HitmanUtilities.Deinitialize();

        hitman.inDeadlockMode = false;
        hitman.StartTransition(new Color(0f, 0f, 0f, 0f), () => hitman.overlay?.gameObject.SetActive(false));

        HudManager.Instance?.AlertFlash?.SetOverlay(false);
        SoundManager.Instance.PlaySound(LaunchpadAssets.DeadlockFadeOut.LoadAsset(), false);
        SoundManager.Instance.StopSound(LaunchpadAssets.DeadlockAmbience.LoadAsset());

        if (HitmanUtilities.MarkedPlayers != null && HitmanUtilities.MarkedPlayers.Count > 0)
        {
            Coroutines.Start(HitmanUtilities.KillMarkedPlayers());
        }
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (!playerControl.AmOwner || playerControl.Data.Role is not HitmanRole || !EffectActive || !HudManager.InstanceExists)
        {
            return;
        }

        HitmanUtilities.ClockTick ??= Coroutines.Start(HitmanUtilities.ClockTickRoutine());

        HitmanUtilities.OverlayTick ??= Coroutines.Start(HitmanUtilities.OverlayColorRoutine());

        if (HitmanUtilities.MarkedPlayers?.Count >= OptionGroupSingleton<HitmanOptions>.Instance.MarkLimit)
        {
            return;
        }

        HitmanUtilities.PlayerMarkCheck();
    }
}