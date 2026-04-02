using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LaunchpadReloaded.Utilities;

namespace LaunchpadReloaded.Patches.Generic;

/// <summary>
/// Cancel clicking of any action buttons in Among Us (if should cancel click)
/// </summary>
[HarmonyPatch]
public class ButtonPatches
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        return AccessTools.GetTypesFromAssembly(typeof(ActionButton).Assembly)
            .Where(type => type.IsAssignableTo(typeof(ActionButton)))
            .SelectMany(type => type.GetMethods())
            .Where(method => method.Name == "DoClick");
    }

    public static bool Prefix(ActionButton __instance)
    {
        if (__instance is UseButton)
        {
            return true;
        }

        return !Helpers.ShouldCancelClick();
    }
}
