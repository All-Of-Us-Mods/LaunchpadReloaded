using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using UnityEngine;

namespace LaunchpadReloaded.Utilities;

public static class HackerUtilities
{
    public static readonly Dictionary<ShipStatus.MapType, Vector3[]> MapNodePositions = new()
    {
        [ShipStatus.MapType.Ship] = [
            new(-3.9285f, 5.6983f, 0.0057f),
            new(12.1729f, -6.5887f, -0.0066f),
            new Vector3(-19.7123f, -6.8006f, -0.0068f),
            new Vector3(-12.3633f, -14.6075f, -0.0146f) ],
        [ShipStatus.MapType.Pb] = [
            new Vector3(3.5599f, -7.584f, -0.0076f),
            new Vector3(22.1169f, -25.0981f, -0.0251f),
            new Vector3(37.3687f, -21.9697f, -0.022f),
            new Vector3(40.6573f, -7.9562f, -0.008f) ],
        [ShipStatus.MapType.Hq] = [
            new Vector3(11.5355f, 10.3573f, 0.0104f),
            new Vector3(-3.063f, 3.8147f, 0.0038f),
            new Vector3(16.6542f, 25.3223f, 0.0253f),
            new Vector3(19.5728f, 17.4778f, 0.0175f) ],
        [ShipStatus.MapType.Fungle] = [
            new Vector3(-22.4063f, -1.6647f, -0.0017f),
            new Vector3(-11.0019f, 12.6502f, 0.0127f),
            new Vector3(24.3133f, 14.628f, 0.0146f),
            new Vector3(7.6678f, -9.9008f, -0.0099f)
        ],

        // Submerged compatibility soon
        [(ShipStatus.MapType)6] = []
    };

    public static readonly Vector3[] AirshipPositions = [
        new(-5.0792f, 10.9539f, 0.011f),
        new(16.856f, 14.7769f, 0.0148f),
        new(37.3283f, -3.7612f, -0.0038f),
        new(19.8862f, -3.9247f, -0.0039f),
        new(-13.1688f, -14.4867f, -0.0145f),
        new(-14.2747f, -4.8171f, -0.0048f),
        new(1.4743f, -2.5041f, -0.0025f),
    ];

    private static readonly Func<PlayerControl?, bool> PlayerHacked = player => player?.GetModifier<HackedModifier>() is { DeActivating: false };

    public static bool AnyPlayerHacked()
    {
        return PlayerControl.AllPlayerControls.ToArray().Any(PlayerHacked);
    }

    public static bool IsHacked(this NetworkedPlayerInfo playerInfo)
    {
        return AmongUsClient.Instance.IsGameStarted && PlayerHacked(playerInfo.Object);
    }

    public static void ForceEndHack()
    {
        if (!PlayerControl.LocalPlayer.IsHost()) return;
        foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(plr => plr.HasModifier<HackedModifier>()))
        {
            player.RpcRemoveModifier<HackedModifier>();
        }
    }

    public static bool IsSabotageConsole(this IUsable? usable)
    {
        if (usable != null && usable.TryCast<Console>())
        {
            var task = usable.Cast<Console>().FindTask(PlayerControl.LocalPlayer);
            return task != null && task.TryCast<SabotageTask>();
        }

        return false;
    }

    public static void CreateNode(this ShipStatus shipStatus, int id, Transform parent, Vector3 position)
    {
        var node = new GameObject("Node");
        node.transform.SetParent(parent, false);
        node.transform.localPosition = position;

        var sprite = node.AddComponent<SpriteRenderer>();
        sprite.sprite = LaunchpadAssets.NodeSprite.LoadAsset();
        node.layer = LayerMask.NameToLayer("ShortObjects");
        sprite.transform.localScale = new Vector3(1, 1, 1);

        sprite.material = shipStatus.AllConsoles[0].gameObject.GetComponent<SpriteRenderer>().material;

        var collider = node.AddComponent<CircleCollider2D>();
        collider.radius = 0.1082f;
        collider.offset = new Vector2(-0.01f, -0.3049f);

        var nodeComponent = node.AddComponent<HackNodeComponent>();
        nodeComponent.image = sprite;
        nodeComponent.id = id;

        node.SetActive(true);
    }
}