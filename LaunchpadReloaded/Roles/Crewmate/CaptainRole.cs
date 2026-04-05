using LaunchpadReloaded.Buttons.Crewmate;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modules.Localization;
using MiraAPI.Hud;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Crewmate;

public class CaptainRole(IntPtr ptr) : CrewmateRole(ptr), ICustomRole
{
    public string LocaleKey => "Captain";
    public string RoleName => LaunchpadLocale.GetParsed($"Role{LocaleKey}Name");
    public string RoleDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}Description");
    public string RoleLongDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}LongDescription");
    public Color RoleColor => LaunchpadPalette.CaptainColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.ZoomButton,
        OptionsScreenshot = LaunchpadAssets.CaptainBanner,
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

        if (CustomButtonSingleton<ZoomButton>.Instance.EffectActive)
        {
            CustomButtonSingleton<ZoomButton>.Instance.OnEffectEnd();
        }
    }
}