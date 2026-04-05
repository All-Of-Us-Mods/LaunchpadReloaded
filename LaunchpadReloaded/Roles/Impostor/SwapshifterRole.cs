using LaunchpadReloaded.Buttons.Impostor;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modules.Localization;
using MiraAPI.Hud;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Impostor;

public class SwapshifterRole(IntPtr ptr) : ImpostorRole(ptr), ICustomRole
{
    public string LocaleKey => "Swapshifter";
    public string RoleName => LaunchpadLocale.GetParsed($"Role{LocaleKey}Name");
    public string RoleDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}Description");
    public string RoleLongDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}LongDescription");
    public Color RoleColor => LaunchpadPalette.SwapperColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.SwapButton,
        OptionsScreenshot = LaunchpadAssets.HackerBanner,
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

        if (CustomButtonSingleton<SwapButton>.Instance.EffectActive)
        {
            CustomButtonSingleton<SwapButton>.Instance.OnEffectEnd();
        }
    }
}
