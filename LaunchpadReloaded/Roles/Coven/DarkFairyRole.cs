using AmongUs.GameOptions;
using Il2CppSystem.Text;
using LaunchpadReloaded.Features;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Coven;

public class DarkFairyRole(IntPtr ptr) : ImpostorRole(ptr), ICovenRole
{
    public string RoleName => "Dark Fairy";
    public string RoleDescription => "Darken Crewmates and win together.";
    public string RoleLongDescription => "You can darken players to make them part of your cult.\nIf you lose, your cult lose as well.";
    public Color RoleColor => LaunchpadPalette.DarkFairyColor;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.DarkFairyButton,
        UseVanillaKillButton = false,
        CanUseVent = false,
        CanUseSabotage = false,
    };
    
    public override void AppendTaskHint(StringBuilder taskStringBuilder)
    {
        // remove default task hint
    }
    
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl != PlayerControl.LocalPlayer)
        {
            return;
        }

        var orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl);
        orCreateTask.Text = string.Concat([
            LaunchpadPalette.DarkFairyColor.ToTextColor(),
            "Work Alone",
            "</color>"
        ]);
    }
}
