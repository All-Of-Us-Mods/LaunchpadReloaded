using LaunchpadReloaded.Modifiers;
using System.Collections;
using System.Globalization;
using MiraAPI.Modifiers;
using UnityEngine;
using Random = System.Random;

namespace LaunchpadReloaded.Utilities;

public static class Helpers
{
    public static readonly Random Random = new();

    public static bool ShouldCancelClick()
    {
        return PlayerControl.LocalPlayer.HasModifier<DragBodyModifier>() || PlayerControl.LocalPlayer.GetModifier<HackedModifier>() is { DeActivating: false };
    }

    public static PlayerControl? GetPlayerToPoint(Vector3 position)
    {
        Collider2D[] hitColliders = Physics2D.OverlapPointAll(position);

        foreach (var hitCollider in hitColliders)
        {
            var playerControl = hitCollider.GetComponent<PlayerControl>();
            if (!playerControl) continue;
            return playerControl;
        }

        return null;
    }

    public static string FirstLetterToUpper(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }

    public static IEnumerator FadeOut(SpriteRenderer? rend, float delay = 0.01f, float decrease = 0.01f)
    {
        if (rend == null)
        {
            yield break;
        }

        var alphaVal = rend.color.a;
        var tmp = rend.color;

        while (alphaVal > 0)
        {
            alphaVal -= decrease;
            tmp.a = alphaVal;
            rend.color = tmp;

            yield return new WaitForSeconds(delay);
        }
    }
}