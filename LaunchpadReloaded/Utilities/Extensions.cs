using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Options;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using PowerTools;
using Reactor.Utilities.Extensions;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LaunchpadReloaded.Utilities;

public static class Extensions
{
    public static PlayerTagManager? GetTagManager(this PlayerControl player)
    {
        return player.GetComponent<PlayerTagManager>();
    }

    public static DeadBodyCacheComponent GetCacheComponent(this DeadBody body)
    {
        return body.gameObject.GetComponent<DeadBodyCacheComponent>();
    }

    public static void SetBodyType(this PlayerControl player, int bodyType)
    {
        if (bodyType == 6)
        {
            player.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
            if (!OptionGroupSingleton<BattleRoyaleOptions>.Instance.ShowKnife.Value)
            {
                return;
            }

            var seekerHand = player.transform.FindChild("BodyForms/Seeker/SeekerHand").gameObject;
            var hand = Object.Instantiate(seekerHand).gameObject;
            hand.transform.SetParent(seekerHand.transform.parent);
            hand.transform.localScale = new Vector3(2, 2, 2);
            hand.name = "KnifeHand";
            hand.layer = LayerMask.NameToLayer("Players");

            var transform = player.transform;

            hand.transform.localPosition = transform.localPosition;
            hand.transform.position = transform.position;

            var nodeSync = hand.GetComponent<SpriteAnimNodeSync>();
            nodeSync.flipOffset = new Vector3(-1.5f, 0.5f, 0);
            nodeSync.normalOffset = new Vector3(1.5f, 0.5f, 0);

            var rend = hand.GetComponent<SpriteRenderer>();
            rend.sprite = LaunchpadAssets.KnifeHandSprite.LoadAsset();

            hand.SetActive(true);
            return;
        }

        player.MyPhysics.SetBodyType((PlayerBodyTypes)bodyType);

        if (bodyType == (int)PlayerBodyTypes.Normal)
        {
            player.cosmetics.currentBodySprite.BodySprite.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            player.cosmetics.SetNamePosition(new Vector3(0, 1, -.5f));
        }
    }

    public static void SetGradientData(this GameObject gameObject, byte playerId)
    {
        var data = gameObject.GetComponent<PlayerGradientData>();
        if (!data)
        {
            data = gameObject.AddComponent<PlayerGradientData>();
        }
        data.playerId = playerId;
    }

    public static bool ButtonTimerEnabled(this PlayerControl playerControl)
    {
        return (playerControl.moveable || playerControl.petting) && playerControl is { inVent: false, shapeshifting: false } && (!DestroyableSingleton<HudManager>.InstanceExists || !DestroyableSingleton<HudManager>.Instance.IsIntroDisplayed) && !MeetingHud.Instance && !PlayerCustomizationMenu.Instance && !ExileController.Instance && !IntroCutscene.Instance;
    }

    public static bool IsSealed(this Vent vent)
    {
        return vent.gameObject.TryGetComponent<SealedVentComponent>(out _);
    }

    public static void Revive(this DeadBody body, PlayerControl sender)
    {
        var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(player => player.PlayerId == body.ParentId);
        if (player == null)
        {
            return;
        }

        player.NetTransform.SnapTo(body.transform.position);

        if (PhysicsHelpers.AnythingBetween(player.Collider, player.Collider.bounds.center, player.transform.position, Constants.ShipAndAllObjectsMask, false))
        {
            player.NetTransform.SnapTo(sender.transform.position);
        }

        body.gameObject.Destroy();
        player.GetModifierComponent().AddModifier<RevivedModifier>();
    }

    public static void HideBody(this DeadBody body)
    {
        body.GetComponent<DeadBodyCacheComponent>().SetVisibility(false);
        body.Reported = true;
    }

    public static void ShowBody(this DeadBody body, bool reported)
    {
        body.GetComponent<DeadBodyCacheComponent>().SetVisibility(true);
        body.Reported = reported;
    }
}