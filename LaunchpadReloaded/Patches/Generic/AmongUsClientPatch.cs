﻿using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime;
using InnerNet;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Networking.Color;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using UnityEngine;

namespace LaunchpadReloaded.Patches.Generic;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CreatePlayer))]
public static class AmongUsClientPatch
{
    public static void Postfix(ClientData clientData)
    {
        if (!AmongUsClient.Instance.AmHost)
        {
            return;
        }

        Coroutines.Start(WaitForPlayer(clientData));
    }

    private static IEnumerator WaitForPlayer(ClientData clientData)
    {
        // Wait until the player is fully loaded
        var del = DelegateSupport.ConvertDelegate<Il2CppSystem.Func<bool>>(
            () => GameData.Instance.GetPlayerByClient(clientData)?.IsIncomplete == false
                  && PlayerControl.LocalPlayer != null);

        yield return new WaitUntil(del);
        
        var colorData = new Dictionary<byte, CustomColorData>();
        
        foreach (var player in GameData.Instance.AllPlayers)
        {
            if (GradientManager.TryGetColor(player.PlayerId, out var color))
            {
                colorData.Add(player.PlayerId, new CustomColorData((byte)player.DefaultOutfit.ColorId, color));
            }
        }

        // typically we would split this into multiple packets
        // however, it wouldnt exceed the limit unless there was over 300 players
        Rpc<RpcSyncAllColors>.Instance.SendTo(clientData.Id, colorData);
    }
}