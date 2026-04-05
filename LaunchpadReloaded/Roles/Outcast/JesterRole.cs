using AmongUs.GameOptions;
using Il2CppSystem.Text;
using System.Collections.Generic;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.GameOver;
using LaunchpadReloaded.Modules.Localization;
using LaunchpadReloaded.Options.Roles.Neutral;
using MiraAPI.GameEnd;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Outcast;

public class JesterRole(System.IntPtr ptr) : RoleBehaviour(ptr), IOutcastRole
{
    public string LocaleKey => "Jester";
    public string RoleName => LaunchpadLocale.GetParsed($"Role{LocaleKey}Name");
    public string RoleDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}Description");
    public string RoleLongDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}LongDescription");
    public Color RoleColor => LaunchpadPalette.JesterColor;
    public override bool IsDead => false;

    public CustomRoleConfiguration Configuration => new(this)
    {
        TasksCountForProgress = false,
        CanUseVent = OptionGroupSingleton<JesterOptions>.Instance.CanUseVents,
        GhostRole = (RoleTypes)RoleId.Get<OutcastGhostRole>(),
        Icon = LaunchpadAssets.JesterIcon,
        OptionsScreenshot = LaunchpadAssets.JesterBanner,
    };

    public override void AppendTaskHint(StringBuilder taskStringBuilder)
    {
        // remove default task hint
    }

    public override bool DidWin(GameOverReason reason)
    {
        return reason == CustomGameOver.GameOverReason<JesterGameOver>();
    }

    public string GetCustomEjectionMessage(NetworkedPlayerInfo exiled)
    {
        return LaunchpadLocale.GetParsed(
            $"Role{LocaleKey}EjectionMessage",
            null,
            new Dictionary<string, string>
            {
                { "<player>", exiled.PlayerName },
            });
    }

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>();
        return !(console != null) || console.AllowImpostor;
    }

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl != PlayerControl.LocalPlayer)
        {
            return;
        }

        var orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl);
        orCreateTask.Text = string.Concat([
            LaunchpadPalette.JesterColor.ToTextColor(),
                DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FakeTasks, Il2CppSystem.Array.Empty<Il2CppSystem.Object>()),
                "</color>"
        ]);
    }
}
