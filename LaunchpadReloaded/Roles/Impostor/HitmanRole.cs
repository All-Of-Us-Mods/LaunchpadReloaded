using LaunchpadReloaded.Buttons.Impostor;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modules.Localization;
using LaunchpadReloaded.Utilities;
using MiraAPI.Hud;
using MiraAPI.Roles;
using Reactor.Utilities;
using System;
using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Impostor;

public class HitmanRole(IntPtr ptr) : ImpostorRole(ptr), ICustomRole
{
    public string LocaleKey => "Hitman";
    public string RoleName => LaunchpadLocale.GetParsed($"Role{LocaleKey}Name");
    public string RoleDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}Description");
    public string RoleLongDescription => LaunchpadLocale.GetParsed($"Role{LocaleKey}LongDescription");
    public Color RoleColor => LaunchpadPalette.HitmanColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.DeadlockButton,
        OptionsScreenshot = LaunchpadAssets.JanitorBanner,
        UseVanillaKillButton = false,
    };

    public bool inDeadlockMode;
    public GameObject? overlay;
    public readonly Color OverlayColor = new(10f / 255f, 30f / 255f, 10f / 255f, 0.4f);

    public SpriteRenderer? OverlayRend { get; private set; }
    private IEnumerator? _transitionCoroutine;

    public override void OnDeath(DeathReason reason)
    {
        Deinitialize(Player);
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        if (!targetPlayer.AmOwner)
        {
            return;
        }

        if (CustomButtonSingleton<DeadlockButton>.Instance.EffectActive)
        {
            HitmanUtilities.ClearMarks();
            CustomButtonSingleton<DeadlockButton>.Instance.OnEffectEnd();
        }
    }

    [HideFromIl2Cpp]
    public void StartTransition(Color targetColor, Action? action = null)
    {
        if (_transitionCoroutine != null)
        {
            Coroutines.Stop(_transitionCoroutine);
        }

        _transitionCoroutine = Coroutines.Start(HitmanUtilities.TransitionColor(targetColor, OverlayRend!, 1f, action));
    }

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        overlay = GameObject.Find("DeadlockTint");

        if (overlay == null)
        {
            overlay = new GameObject("DeadlockTint");
            overlay.gameObject.SetActive(false);
            overlay.transform.SetParent(HudManager.Instance.transform, true);

            OverlayRend = overlay.AddComponent<SpriteRenderer>();
            OverlayRend.sprite = LaunchpadAssets.DeadlockVignette.LoadAsset();
            OverlayRend.color = OverlayColor;

            var mainCamera = Camera.main!;
            var screenHeight = mainCamera.orthographicSize * 2f;
            var screenWidth = screenHeight * mainCamera.aspect;
            var spriteWidth = OverlayRend.sprite.bounds.size.x;
            var spriteHeight = OverlayRend.sprite.bounds.size.y;

            overlay.transform.localScale = new Vector3(screenWidth / spriteWidth, screenHeight / spriteHeight, 1f);
            var position = HudManager.Instance.transform.position;
            overlay.transform.localPosition = new Vector3(position.x, position.y, 20f);
        }
    }

    public void FixedUpdate()
    {
        if (overlay && Player.AmOwner)
        {
            overlay!.transform.position = HudManager.Instance.FullScreen.transform.position;
        }
    }
}