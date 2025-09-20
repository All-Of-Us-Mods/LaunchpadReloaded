using Il2CppInterop.Runtime.Attributes;
using LaunchpadReloaded.Features;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Attributes;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LaunchpadReloaded.Components;

[RegisterInIl2Cpp]
public sealed class GuessRoleMinigame(IntPtr ptr) : Minigame(ptr)
{
    private ShapeshifterPanel _panelPrefab = null!;
    private Action<RoleBehaviour> _onClick = null!;
    private Scroller _scroller = null!;

    private PassiveButton _closeButton = null!;
    private PassiveButton _outsideCloseButton = null!;

    public void Awake()
    {
        _outsideCloseButton = transform.FindChild("Background/OutsideCloseButton").GetComponent<PassiveButton>();
        _closeButton = transform.FindChild("CloseButton").GetComponent<PassiveButton>();

        _closeButton.OnClick.AddListener((UnityAction)(() => Close()));
        _outsideCloseButton.OnClick.AddListener((UnityAction)(() => Close()));

        _panelPrefab = transform.FindChild("Panel").gameObject.GetComponent<ShapeshifterPanel>();
        _scroller = transform.FindChild("Scroller").gameObject.GetComponent<Scroller>();

        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    public static GuessRoleMinigame Create()
    {
        var gameObject = Instantiate(LaunchpadAssets.RoleMinigame.LoadAsset(), HudManager.Instance.transform);
        var minigame = gameObject.AddComponent<GuessRoleMinigame>();
        return minigame;
    }

    [HideFromIl2Cpp]
    public static void SetRole(ShapeshifterPanel panel, RoleBehaviour roleBehaviour, Action onClick)
    {
        panel.shapeshift = onClick;
        panel.Button.ClickSound = HudManager.Instance.MapButton.ClickSound;

        var color = roleBehaviour.NameColor == Color.white ? roleBehaviour.TeamColor : roleBehaviour.NameColor;

        panel.NameText.text = "<font=\"LiberationSans SDF\" material=\"LiberationSans SDF - Chat Message Masked\">" + roleBehaviour.NiceName + "</font>";
        panel.NameText.color = Color.white;

        panel.Background.color = color.DarkenColor(0.3f);
        panel.gameObject.SetActive(true);

        var roleIcon = panel.transform.FindChild("RoleIcon").gameObject.GetComponent<SpriteRenderer>();
        Sprite? sprite = null;

        if (CustomRoleManager.GetCustomRoleBehaviour(roleBehaviour.Role, out var customRole))
        {
            var icon = customRole?.Configuration.Icon;
            if (icon == MiraAssets.Empty || icon == null)
            {
                sprite = null;
            }
            else
            {
                sprite = icon.LoadAsset();
            }
        }
        else
        {
            if (roleBehaviour.RoleIconSolid != null)
            {
                sprite = roleBehaviour.RoleIconSolid;
            }
        }

        if (sprite == null)
        {
            roleIcon.gameObject.SetActive(false);

            var rectTransform = panel.NameText.gameObject.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(0, 0, -15);
            rectTransform.sizeDelta = new Vector2(2.5f, 0.3726f);
        }
        else
        {
            roleIcon.sprite = sprite;
        }
    }

    [HideFromIl2Cpp]
    public void Open(Func<RoleBehaviour, bool> roleMatch, Action<RoleBehaviour?> clickHandler)
    {
        _onClick = clickHandler;

        _closeButton.OnClick.AddListener((UnityAction)(() =>
        {
            clickHandler(null);
        }));

        _outsideCloseButton.OnClick.AddListener((UnityAction)(() =>
        {
            clickHandler(null);
        }));

        var roles = RoleManager.Instance.AllRoles.ToArray().Where(roleMatch).ToArray();

        foreach (var role in roles)
        {
            var shapeshifterPanel = Instantiate(_panelPrefab, _scroller.Inner);
            SetRole(shapeshifterPanel, role, () => { _onClick(role); });
        }

        _scroller.SetBounds(new FloatRange(0, roles.Length * 0.5f - 2), new FloatRange(0, 0));
        Begin(null);
    }
}