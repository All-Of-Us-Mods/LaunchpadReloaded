using LaunchpadReloaded.Options;
using MiraAPI.GameOptions;

namespace LaunchpadReloaded.Networking;

public static class HostAntiCheat
{
    public static void KickForCheating(this PlayerControl hacker)
    {
        if (!AmongUsClient.Instance.AmHost)
        {
            return;
        }

        var id = AmongUsClient.Instance.GetClientIdFromCharacter(hacker);
        AmongUsClient.Instance.KickPlayer(id, OptionGroupSingleton<GeneralOptions>.Instance.BanCheaters);
        Info($"[HostAntiCheat] Kicked player {hacker.Data.PlayerName} for cheating.");

        var lastMethod = new System.Diagnostics.StackTrace().GetFrame(1)?.GetMethod();
        var methodName = lastMethod != null ? $"{lastMethod.DeclaringType?.FullName}.{lastMethod.Name}" : "UnknownMethod";
        Info($"[HostAntiCheat] Cheating detected in method: {methodName}");
    }
}