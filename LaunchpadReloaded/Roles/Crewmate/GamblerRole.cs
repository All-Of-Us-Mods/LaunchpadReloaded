using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modules.Localization;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Crewmate;

public class GamblerRole(IntPtr ptr) : CrewmateRole(ptr), ICustomRole
{
    public string LocaleKey => "Gambler";
    public string RoleName => LaunchpadLocale.GetParsed($"Role{LocaleKey}Name");
    public string RoleDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}Description");
    public string RoleLongDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}LongDescription");
    public Color RoleColor => LaunchpadPalette.GamblerColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.GambleButton,
        OptionsScreenshot = LaunchpadAssets.DetectiveBanner,
    };
}
