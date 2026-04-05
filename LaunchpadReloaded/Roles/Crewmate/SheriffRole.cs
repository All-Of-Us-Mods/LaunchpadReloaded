using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modules.Localization;
using MiraAPI.Roles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Crewmate;

public class SheriffRole(IntPtr ptr) : CrewmateRole(ptr), ICustomRole
{
    public string LocaleKey => "Sheriff";
    public string RoleName => LaunchpadLocale.GetParsed($"Role{LocaleKey}Name");
    public string RoleDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}Description");
    public string RoleLongDescription => LaunchpadLocale.GetParsed(
        $"Role{LocaleKey}LongDescription",
        null,
        new Dictionary<string, string>
        {
            { "<impostor>", $"{Palette.ImpostorRed.ToTextColor()}Impostor</color>" },
            { "<crewmate>", $"{Palette.CrewmateBlue.ToTextColor()}Crewmate</color>" },
        });
    public Color RoleColor => LaunchpadPalette.SheriffColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.ShootButton,
        OptionsScreenshot = LaunchpadAssets.SheriffBanner,
    };
}
