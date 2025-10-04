using LaunchpadReloaded.Features;
using LaunchpadReloaded.Utilities;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.LocalSettings;
using MiraAPI.PluginLoading;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;

[MiraIgnore]
public abstract class BaseLaunchpadButton : CustomActionButton
{
    public override ButtonLocation Location => LocalSettingsTabSingleton<LaunchpadSettings>.Instance.ButtonLocation.Value;

    public abstract bool TimerAffectedByPlayer { get; }

    public abstract bool AffectedByHack { get; }

    public override BaseKeybind Keybind => MiraGlobalKeybinds.PrimaryAbility;

    public override bool CanUse()
    {
        var buttonTimer = !TimerAffectedByPlayer || PlayerControl.LocalPlayer.ButtonTimerEnabled();
        var hack = !AffectedByHack || !PlayerControl.LocalPlayer.Data.IsHacked();
        return base.CanUse() && PlayerControl.LocalPlayer.CanMove && buttonTimer && hack;
    }
}

[MiraIgnore]
public abstract class BaseLaunchpadButton<T> : CustomActionButton<T> where T : MonoBehaviour
{
    public override ButtonLocation Location => ButtonLocation.BottomRight;

    public abstract bool TimerAffectedByPlayer { get; }

    public abstract bool AffectedByHack { get; }

    public override BaseKeybind Keybind => MiraGlobalKeybinds.PrimaryAbility;

    public override void ResetTarget()
    {
        SetOutline(false);
        Target = null;
    }

    public override bool CanUse()
    {
        var buttonTimer = !TimerAffectedByPlayer || PlayerControl.LocalPlayer.ButtonTimerEnabled();
        var hack = !AffectedByHack || !PlayerControl.LocalPlayer.Data.IsHacked();
        return base.CanUse() && PlayerControl.LocalPlayer.CanMove && buttonTimer && hack;
    }
}