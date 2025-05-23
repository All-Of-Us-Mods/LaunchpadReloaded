using LaunchpadReloaded.Features;
using MiraAPI.Utilities;
using Reactor.Utilities.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaunchpadReloaded.Components;

[RegisterInIl2Cpp(typeof(IUsable))]
public class HackNodeComponent(IntPtr ptr) : MonoBehaviour(ptr), IBaseUsable
{
    public static List<HackNodeComponent> AllNodes { get; } = [];

    public bool isActive;
    public int id;
    public SpriteRenderer image = null!;
    public ArrowBehaviour? arrow;

    public float UsableDistance => 0.8f;
    public float PercentCool => 0;
    public ImageNames UseIcon => ImageNames.UseButton;

    public void Awake()
    {
        AllNodes.Add(this);
    }

    public void SetArrowActive(bool value)
    {
        if (arrow == null)
        {
            arrow = Helpers.CreateArrow(transform, LaunchpadPalette.HackerColor);
            arrow.target = transform.position;
        }

        arrow.gameObject.SetActive(value);
    }
    public void OnDestroy()
    {
        AllNodes.Remove(this);
    }

    public void SetOutline(bool on, bool mainTarget)
    {
        image.material.SetFloat(ShaderID.Outline, on ? 1 : 0);
        image.material.SetColor(ShaderID.OutlineColor, Color.green);
        image.material.SetColor(ShaderID.AddColor, mainTarget ? Color.green : Color.clear);
    }

    public void Use()
    {
        var nodeGame = Instantiate(LaunchpadAssets.NodeGame.LoadAsset(), HudManager.Instance.transform);
        var miniGame = nodeGame.AddComponent<NodeMinigame>();
        miniGame.Open(this);
    }

    public float CanUse(NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
    {
        var num = float.MaxValue;
        var @object = pc.Object;
        couldUse = !pc.IsDead && @object.CanMove && isActive;
        canUse = couldUse;
        if (canUse)
        {
            var truePosition = @object.GetTruePosition();
            var position = transform.position;
            num = Vector2.Distance(truePosition, position);
            canUse &= num <= UsableDistance;
        }
        return num;
    }
}
