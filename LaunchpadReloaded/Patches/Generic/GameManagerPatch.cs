using HarmonyLib;
using LaunchpadReloaded.Components;
using Reactor.Utilities;

namespace LaunchpadReloaded.Patches.Generic;

[HarmonyPatch(typeof(GameManager), nameof(GameManager.Awake))]
public static class GameManagerPatch
{
    public static void Postfix(GameManager __instance)
    {
        foreach (var deadBody in __instance.deadBodyPrefab)
        {
            deadBody.gameObject.AddComponent<DeadBodyCacheComponent>();
            Logger<LaunchpadReloadedPlugin>.Info("Added DeadBodyCacheComponent to dead body prefab");
        }
    }
}