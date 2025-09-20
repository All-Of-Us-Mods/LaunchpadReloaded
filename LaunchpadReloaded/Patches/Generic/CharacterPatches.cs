using HarmonyLib;
using InnerNet;
using System;
using LaunchpadReloaded.Options;
using MiraAPI.GameOptions;
using Reactor.Utilities;
using static CosmeticsLayer;

namespace LaunchpadReloaded.Patches.Generic;
/// <summary>
/// Credit to Pietro for helping, and some code for this class
/// https://github.com/0xDrMoe/TownofHost-Enhanced/pull/784/files#diff-f347fffd3b0ec57eb94d7c80b0327474b4b2574e31bb09005e4c2407bc7471b7
/// 
/// TODO: FIX NAMETAG POSITION
/// </summary>

[HarmonyPatch]
public static class CharacterPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NormalGameManager), "GetBodyType")]
    public static void GetBodyTypePatches(ref PlayerBodyTypes __result)
    {
        __result = FunOptions.IntToBodyTypes[OptionGroupSingleton<FunOptions>.Instance.Character.Value];
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LongBoiPlayerBody), nameof(LongBoiPlayerBody.Awake))]
    public static bool LongBodyAwakePatch(LongBoiPlayerBody __instance)
    {
        __instance.cosmeticLayer.OnSetBodyAsGhost += (Action)__instance.SetPoolableGhost;
        __instance.cosmeticLayer.OnColorChange += (Action<int>)__instance.SetHeightFromColor;
        __instance.cosmeticLayer.OnCosmeticSet += (Action<string, int, CosmeticKind>)__instance.OnCosmeticSet;
        __instance.gameObject.layer = 8;

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LongBoiPlayerBody), nameof(LongBoiPlayerBody.SetHeightFromColor))]
    public static bool SetHeightColorPatch(LongBoiPlayerBody __instance)
    {
        if (!__instance.isPoolablePlayer)
        {
            if (GameManager.Instance.IsHideAndSeek() && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started && __instance.myPlayerControl.Data.Role != null && __instance.myPlayerControl.Data.Role.TeamType == RoleTeamTypes.Impostor)
            {
                return false;
            }
            __instance.targetHeight = __instance.heightsPerColor[0];
            if (LobbyBehaviour.Instance)
            {
                __instance.SetupNeckGrowth(false, false);
                return false;
            }
            __instance.SetupNeckGrowth(true, false);
        }
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LongBoiPlayerBody), nameof(LongBoiPlayerBody.SetHeighFromDistanceHnS))]
    public static bool NeckSizePatch(LongBoiPlayerBody __instance, ref float distance)
    {
        __instance.targetHeight = distance / 10f + 0.5f;
        __instance.SetupNeckGrowth(true);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LongBoiPlayerBody), nameof(LongBoiPlayerBody.Start))]
    public static bool LongBodyStartPatch(LongBoiPlayerBody __instance)
    {
        Logger<LaunchpadReloadedPlugin>.Info("hello there should longer");
        __instance.ShouldLongAround = true;
        __instance.skipNeckAnim = true;
        if (__instance.hideCosmeticsQC)
        {
            __instance.cosmeticLayer.SetHatVisorVisible(false);
        }
        __instance.SetupNeckGrowth(true);
        if (__instance.isExiledPlayer)
        {
            var instance = ShipStatus.Instance;
            if (instance == null || instance.Type != ShipStatus.MapType.Fungle)
            {
                __instance.cosmeticLayer.AdjustCosmeticRotations(-17.75f);
            }
        }
        if (!__instance.isPoolablePlayer)
        {
            __instance.cosmeticLayer.ValidateCosmetics();
        }
        if (__instance.myPlayerControl)
        {
            __instance.StopAllCoroutines();
            __instance.SetHeightFromColor(__instance.myPlayerControl.Data.DefaultOutfit.ColorId);
        }
        return false;
    }
}