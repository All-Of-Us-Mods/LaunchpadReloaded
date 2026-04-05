using Il2CppInterop.Runtime.Attributes;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modules.Localization;
using MiraAPI.Roles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Impostor;

public class BurrowerRole(IntPtr ptr) : ImpostorRole(ptr), ICustomRole
{
    public string LocaleKey => "Burrower";
    public string RoleName => LaunchpadLocale.GetParsed($"Role{LocaleKey}Name");
    public string RoleDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}Description");
    public string RoleLongDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}LongDescription");
    public Color RoleColor => LaunchpadPalette.BurrowerColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.DigVentButton,
        OptionsScreenshot = LaunchpadAssets.HackerBanner,
    };

    [HideFromIl2Cpp]
    public List<Vent> DugVents { get; } = [];
}
