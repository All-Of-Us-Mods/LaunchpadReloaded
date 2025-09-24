using AmongUs.Data;
using HarmonyLib;
using LaunchpadReloaded.Features;
using MiraAPI.LocalSettings;
using UnityEngine;

namespace LaunchpadReloaded.Patches.Generic;

[HarmonyPatch(typeof(FollowerCamera), nameof(FollowerCamera.Update))]
public static class FollowerCameraUpdatePatch
{
    public static void Postfix(FollowerCamera __instance)
    {
        if (!__instance.Target || __instance.Locked || !LocalSettingsTabSingleton<LaunchpadSettings>.Instance.LockedCamera.Value) return;
        __instance.centerPosition = __instance.Target.transform.position + (Vector3) __instance.Offset;

        var v = __instance.centerPosition;
        if (__instance.shakeAmount > 0f && DataManager.Settings.Gameplay.ScreenShake && __instance.OverrideScreenShakeEnabled)
        {
            var num = Time.fixedTime * __instance.shakePeriod;
            var num2 = Mathf.PerlinNoise(0.5f, num) * 2f - 1f;
            var num3 = Mathf.PerlinNoise(num, 0.5f) * 2f - 1f;
            v.x += num2 * __instance.shakeAmount;
            v.y += num3 * __instance.shakeAmount;
        }
        __instance.transform.position = v;
    }
}