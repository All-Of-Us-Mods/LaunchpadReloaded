using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LaunchpadReloaded.Patches.Compat;

[HarmonyPatch]
public static class MiraApiHudManagerPatch
{
    public static MethodBase? TargetMethod()
    {
        return AccessTools.Method("MiraAPI.Patches.HudManagerPatches:StartPostfix");
    }

    public static Exception? Finalizer(Exception __exception)
    {
        if (__exception is KeyNotFoundException knf && knf.Message.Contains("ActionButton"))
        {
            return null;
        }

        return __exception;
    }
}
