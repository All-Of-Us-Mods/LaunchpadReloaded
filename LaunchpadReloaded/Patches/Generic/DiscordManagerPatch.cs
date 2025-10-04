using System;
using Discord;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LaunchpadReloaded.Patches.Generic;

/// <summary>
/// Custom Discord RPC
/// </summary>
[HarmonyPatch]
public static class DiscordManagerPatch
{
    private const long ClientId = 1217217004474339418;
    private const uint SteamAppId = 945360;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(DiscordManager), nameof(DiscordManager.Start))]
    public static bool DiscordManagerStartPrefix(DiscordManager __instance)
    {
        DiscordManager.ClientId = ClientId;
        if (Application.platform == RuntimePlatform.Android)
        {
            return true;
        }

        InitializeDiscord(__instance);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
    public static void ActivityManagerUpdateActivityPrefix(ActivityManager __instance, [HarmonyArgument(0)] Activity activity)
    {
        activity.Details += " All Of Us: Launchpad";
        activity.State += " | dsc.gg/allofus";
    }

    private static void InitializeDiscord(DiscordManager __instance)
    {
        __instance.presence = new Discord.Discord(ClientId, 1UL);
        var activityManager = __instance.presence.GetActivityManager();

        activityManager.RegisterSteam(SteamAppId);
        activityManager.add_OnActivityJoin((Action<string>)__instance.HandleJoinRequest);
        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) =>
        {
            __instance.OnSceneChange(scene.name);
        }));
        __instance.SetInMenus();
    }
}
