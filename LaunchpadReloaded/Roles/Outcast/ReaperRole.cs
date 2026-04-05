using System.Text;
using System.Collections.Generic;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.GameOver;
using LaunchpadReloaded.Modules.Localization;
using LaunchpadReloaded.Options.Roles.Neutral;
using MiraAPI.GameEnd;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Outcast;

public class ReaperRole(System.IntPtr ptr) : RoleBehaviour(ptr), IOutcastRole
{
    public string LocaleKey => "Reaper";
    public string RoleName => LaunchpadLocale.GetParsed($"Role{LocaleKey}Name");
    public string RoleDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}Description");
    public string RoleLongDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}LongDescription");
    public Color RoleColor => LaunchpadPalette.ReaperColor;
    public override bool IsDead => false;

    public int collectedSouls;

    public CustomRoleConfiguration Configuration => new(this)
    {
        TasksCountForProgress = false,
        CanUseVent = false,
        GhostRole = (RoleTypes)RoleId.Get<OutcastGhostRole>(),
        Icon = LaunchpadAssets.SoulButton,
        OptionsScreenshot = LaunchpadAssets.JesterBanner,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var sb = CustomRoleUtils.CreateForRole(this);
        sb.Append("\n<b>" + LaunchpadLocale.GetParsed(
            $"Role{LocaleKey}TabSouls",
            null,
            new Dictionary<string, string>
            {
                { "<count>", collectedSouls.ToString() },
                { "<total>", OptionGroupSingleton<ReaperOptions>.Instance.SoulCollections.ToString() },
            }));
        return sb;
    }

    public override void AppendTaskHint(Il2CppSystem.Text.StringBuilder taskStringBuilder)
    {
        // remove default task hint
    }

    public override bool DidWin(GameOverReason reason)
    {
        return reason == CustomGameOver.GameOverReason<ReaperGameOver>();
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
}
