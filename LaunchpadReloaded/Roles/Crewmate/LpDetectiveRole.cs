using LaunchpadReloaded.Buttons.Crewmate;
using LaunchpadReloaded.Features;
using MiraAPI.Hud;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Crewmate;

public class LpDetectiveRole(IntPtr ptr) : CrewmateRole(ptr), ICustomRole
{
    public string RoleName => "Detective";
    public string RoleDescription => "Investigate and find clues on murders.";
    public string RoleLongDescription => "Investigate bodies to get clues and use your instinct ability\nto see recent footsteps around you!";
    public Color RoleColor => LaunchpadPalette.DetectiveColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.InvestigateButton,
        OptionsScreenshot = LaunchpadAssets.DetectiveBanner,
    };

    public override void OnDeath(DeathReason reason)
    {
        Deinitialize(Player);
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        if (!targetPlayer.AmOwner)
        {
            return;
        }

        if (CustomButtonSingleton<InstinctButton>.Instance.EffectActive)
        {
            CustomButtonSingleton<InstinctButton>.Instance.OnEffectEnd();
        }
    }
}
