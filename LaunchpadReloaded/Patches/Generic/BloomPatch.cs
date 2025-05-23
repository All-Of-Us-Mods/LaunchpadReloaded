﻿using HarmonyLib;
using LaunchpadReloaded.Features;

namespace LaunchpadReloaded.Patches.Generic;

[HarmonyPatch(typeof(FollowerCamera), nameof(FollowerCamera.SnapToTarget))]
public static class BloomPatch
{
    public static void Postfix()
    {
        LaunchpadSettings.SetBloom(LaunchpadSettings.Instance?.Bloom.Enabled ?? false);
    }
}