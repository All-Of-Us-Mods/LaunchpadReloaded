using LaunchpadReloaded.Features;
using LaunchpadReloaded.Options.Roles.Crewmate;
using LaunchpadReloaded.Utilities;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaunchpadReloaded.Modifiers;

public class FootstepsModifier : BaseModifier
{
    public override string ModifierName => "Footsteps";

    public override bool HideOnUi => true;

    private Vector3 _lastPos;

    private Dictionary<GameObject, SpriteRenderer> _currentSteps = null!;

    public override void OnActivate()
    {
        _currentSteps = new Dictionary<GameObject, SpriteRenderer>();
        _lastPos = Player.transform.position;
    }

    public override void OnDeactivate()
    {
        foreach (var obj in _currentSteps)
        {
            Coroutines.Start(FootstepFadeout(obj.Key, obj.Value));
        }
    }

    private static IEnumerator FootstepFadeout(GameObject obj, SpriteRenderer rend)
    {
        yield return Helpers.FadeOut(rend, 0.0001f, 0.05f);
        obj.DestroyImmediate();
    }

    private static IEnumerator FootstepDisappear(GameObject obj, SpriteRenderer rend)
    {
        yield return new WaitForSeconds(OptionGroupSingleton<DetectiveOptions>.Instance.FootstepsDuration);
        yield return FootstepFadeout(obj, rend);
    }

    private bool _lastFlip;

    public override void FixedUpdate()
    {
        if (Vector3.Distance(_lastPos, Player.transform.position) < 1)
        {
            return;
        }

        var angle = Mathf.Atan2(Player.MyPhysics.Velocity.y, Player.MyPhysics.Velocity.x) * Mathf.Rad2Deg;

        var footstep = new GameObject("Footstep")
        {
            transform =
            {
                parent = ShipStatus.Instance.transform,
                position = new Vector3(Player.transform.position.x, Player.transform.position.y, -2f),
                rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward)
            }
        };

        var sprite = footstep.AddComponent<SpriteRenderer>();
        sprite.sprite = LaunchpadAssets.Footstep.LoadAsset();
        sprite.material = LaunchpadAssets.GradientMaterial.LoadAsset();
        footstep.layer = LayerMask.NameToLayer("Players");

        if (_lastFlip == false)
        {
            _lastFlip = true;
            sprite.flipX = true;
        }
        else
        {
            _lastFlip = false;
            sprite.flipX = false;
        }

        sprite.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
        Player.SetPlayerMaterialColors(sprite);

        _currentSteps.Add(footstep, sprite);
        _lastPos = Player.transform.position;
        Coroutines.Start(FootstepDisappear(footstep, sprite));
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}