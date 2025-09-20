using AmongUs.Data;
using HarmonyLib;
using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Networking.Color;
using LaunchpadReloaded.Options;
using MiraAPI.GameOptions;
using Reactor.Networking.Rpc;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace LaunchpadReloaded.Patches.Colors.Gradients;

[HarmonyPatch(typeof(PlayerTab))]
public static class PlayerTabPatches
{
    public static bool SelectGradient;
    private static ColorChip? _switchButton;
    private static TextMeshPro? _buttonText;
    private static TextMeshPro? _titleText;

    private static void SwitchSelector(PlayerTab instance)
    {
        SelectGradient = !SelectGradient;
        instance.currentColor = SelectGradient ? GradientManager.LocalGradientId : DataManager.Player.Customization.Color;
    }

    [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.GetDisplayColor))]
    public static class InventoryTabPatch
    {
        public static bool Prefix(InventoryTab __instance, ref int __result)
        {
            if (SelectGradient)
            {
                __result = GradientManager.LocalGradientId;
                return false;
            }

            return true;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerTab.OnEnable))]
    public static void OnEnablePostfix(PlayerTab __instance)
    {
        if (!_switchButton)
        {
            _titleText = __instance.transform.FindChild("Text").GetComponent<TextMeshPro>();
            _switchButton = Object.Instantiate(__instance.ColorTabPrefab, __instance.ColorTabArea);

            var spriteRenderer = _switchButton.GetComponent<SpriteRenderer>();
            var sprite = spriteRenderer.sprite = LaunchpadAssets.BlankButton.LoadAsset();

            _switchButton.GetComponent<BoxCollider2D>().size = sprite.rect.size / sprite.pixelsPerUnit;
            _switchButton.transform.localScale = new Vector3(1, 1, 1);
            _switchButton.transform.localPosition = new Vector3(2, 1.5f, -2);

            var buttonText = Object.Instantiate(__instance.transform.Find("Text").gameObject, _switchButton.transform);
            buttonText.transform.localPosition = new Vector3(0, 0, 0);
            buttonText.GetComponent<TextTranslatorTMP>().Destroy();

            _buttonText = buttonText.GetComponent<TextMeshPro>();
            _buttonText.alignment = TextAlignmentOptions.Center;
            _buttonText.richText = true;
            _buttonText.fontSize = _buttonText.fontSizeMax = 3.5f;

            _switchButton.Button.OnClick.RemoveAllListeners();
            _switchButton.Button.OnMouseOut.RemoveAllListeners();
            _switchButton.Button.OnMouseOver.RemoveAllListeners();
            _switchButton.Button.OnClick.AddListener((UnityAction)(() => { SwitchSelector(__instance); }));
        }

        foreach (var colorChip in __instance.ColorChips)
        {
            colorChip.Button.OnMouseOut.RemoveAllListeners();
            colorChip.Button.OnMouseOut.AddListener((UnityAction)(() =>
            {
                __instance.SelectColor(SelectGradient ? GradientManager.LocalGradientId : DataManager.Player.Customization.Color);
            }));
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerTab.ClickEquip))]
    public static bool ClickPrefix(PlayerTab __instance)
    {
        if (SelectGradient && __instance.AvailableColors.Remove(__instance.currentColor))
        {
            GradientManager.LocalGradientId = __instance.currentColor;
            __instance.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);
            if (__instance.HasLocalPlayer())
            {
                Rpc<CustomCmdCheckColor>.Instance.SendTo(AmongUsClient.Instance.HostId,
                    new CustomColorData(
                        (byte)PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId,
                        (byte)__instance.currentColor));
            }
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerTab.SelectColor))]
    public static bool SelectPrefix(PlayerTab __instance, [HarmonyArgument(0)] int colorId)
    {
        if (SelectGradient)
        {
            __instance.UpdateAvailableColors();
            __instance.currentColor = colorId;
            var colorName = Palette.GetColorName(colorId);
            PlayerCustomizationMenu.Instance.SetItemName(colorName);
            __instance.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);

            var playerGradientData = __instance.PlayerPreview.GetComponent<PlayerGradientData>();
            if (playerGradientData)
            {
                playerGradientData.Destroy();
            }
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerTab.Update))]
    public static void UpdatePostfix(PlayerTab __instance)
    {
        if (_buttonText != null && _titleText != null)
        {
            _buttonText.text = SelectGradient ? "Main Color" : "Secondary\nColor";
            _titleText.text = SelectGradient ? "Secondary Color: " : "Main Color: ";
        }

        if (SelectGradient)
        {
            __instance.currentColorIsEquipped = __instance.currentColor == GradientManager.LocalGradientId;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerTab.UpdateAvailableColors))]
    public static bool UpdateColorsPrefix(PlayerTab __instance)
    {
        for (var i = 0; i < Palette.PlayerColors.Length; i++)
        {
            __instance.AvailableColors.Add(i);
        }
        if (!OptionGroupSingleton<FunOptions>.Instance.UniqueColors.Value)
        {
            return false;
        }

        if (!GameData.Instance)
        {
            return false;
        }

        var allPlayers = GameData.Instance.AllPlayers;
        var localGradId = GradientManager.LocalGradientId;
        var localColorId = PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId;

        foreach (var data in allPlayers)
        {
            if (!GradientManager.TryGetColor(data.PlayerId, out var gradColor))
            {
                continue;
            }

            var primaryColor = SelectGradient ? data.DefaultOutfit.ColorId : gradColor;
            var secondaryColor = SelectGradient ? gradColor : data.DefaultOutfit.ColorId;

            var isSame = primaryColor == (SelectGradient ? localColorId : localGradId);
            var isOpposite = secondaryColor == (SelectGradient ? localColorId : localGradId);

            if (isSame)
            {
                __instance.AvailableColors.Remove(secondaryColor);
            }

            if (isOpposite)
            {
                __instance.AvailableColors.Remove(primaryColor);
            }
        }

        return false;
    }
}