using LaunchpadReloaded.Buttons.Crewmate;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modules.Localization;
using MiraAPI.Hud;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Crewmate;

public class LpDetectiveRole(IntPtr ptr) : CrewmateRole(ptr), ICustomRole
{
    public string LocaleKey => "Detective";
    public string RoleName => LaunchpadLocale.GetParsed($"Role{LocaleKey}Name");
    public string RoleDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}Description");
    public string RoleLongDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}LongDescription");
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
