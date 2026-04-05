using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Modules.Localization;
using MiraAPI.Roles;
using System;
using MiraAPI.Modifiers;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Impostor;

public class JanitorRole(IntPtr ptr) : ImpostorRole(ptr), ICustomRole
{
    public string LocaleKey => "Janitor";
    public string RoleName => LaunchpadLocale.GetParsed($"Role{LocaleKey}Name");
    public string RoleDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}Description");
    public string RoleLongDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}LongDescription");
    public Color RoleColor => LaunchpadPalette.JanitorColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.DragButton,
        OptionsScreenshot = LaunchpadAssets.JanitorBanner,
    };

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }
        var console = usable.TryCast<Console>();
        return !(console != null) || console.AllowImpostor && !PlayerControl.LocalPlayer.HasModifier<DragBodyModifier>();
    }
}