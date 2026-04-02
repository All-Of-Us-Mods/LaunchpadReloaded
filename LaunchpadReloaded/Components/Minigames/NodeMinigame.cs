using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using MiraAPI.Modifiers;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RegisterInIl2Cpp]
// ReSharper disable once CheckNamespace
public class NodeMinigame(nint ptr) : Minigame(ptr)
{
    public FloatRange sliderX = new(-0.65f, 1.85f);
    public Collider2D[] sliders = null!;
    public TextMeshPro statusText = null!;
    public TextMeshPro nodeIdText = null!;
    private HackNodeComponent _node = null!;
    private readonly Controller _myController = new();
    private int _sliderId;
    private bool _initialized;

    public void Open(HackNodeComponent node)
    {
        EnsureInitialized();
        _node = node;
        nodeIdText.text = $"node_{node.id}";

        statusText.text = "disabled";
        statusText.color = Color.red;
        Begin(null);
    }

    private void Awake()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        var miniGame = GetComponent<DivertPowerMinigame>() ?? GetComponentInChildren<DivertPowerMinigame>(true);
        if (miniGame != null)
        {
            sliders = miniGame.Sliders ?? Array.Empty<Collider2D>();
            OpenSound = miniGame.OpenSound;
            CloseSound = miniGame.CloseSound;
            miniGame.Destroy();
        }
        else
        {
            sliders = Array.Empty<Collider2D>();
        }

        _sliderId = 0;

        var outsideBtn = transform.FindChild("BackgroundCloseButton/OutsideCloseButton").GetComponent<PassiveButton>();
        var closeBtn = transform.FindChild("CloseButton").GetComponent<ButtonBehavior>();
        statusText = transform.FindChild("StatusText").GetComponent<TextMeshPro>();
        nodeIdText = transform.FindChild("NodeId").GetComponent<TextMeshPro>();

        closeBtn.OnClick.AddListener((UnityAction)(() =>
        {
            Close();
        }));

        outsideBtn.OnClick.AddListener((UnityAction)(() =>
        {
            Close();
        }));

        for (var i = 0; i < sliders.Length; i++)
        {
            if (i != _sliderId)
            {
                sliders[i].GetComponent<SpriteRenderer>().color = new Color(0, 0.5188679f, 0.1322604f);
            }
        }

        _initialized = true;
    }

    private void FixedUpdate()
    {
        _myController.Update();

        if (!_node.isActive && amClosing == CloseState.None)
        {
            statusText.text = "enabled";
            statusText.color = Color.green;

            StartCoroutine(CoStartClose(0.6f));
            return;
        }

        if (amClosing != CloseState.None)
        {
            return;
        }


        if (sliders.Length == 0 || _sliderId == sliders.Length)
        {
            return;
        }

        var collider2D2 = sliders[_sliderId];
        Vector2 vector2 = collider2D2.transform.localPosition;
        var dragState = _myController.CheckDrag(collider2D2);
        if (dragState == DragState.Dragging)
        {
            var vector3 = _myController.DragPosition - (Vector2)collider2D2.transform.parent.position;
            vector3.x = sliderX.Clamp(vector3.x);
            vector2.x = vector3.x;
            collider2D2.transform.localPosition = vector2;
            return;
        }

        if (dragState != DragState.Released)
        {
            return;
        }

        if (sliderX.max - vector2.x < 0.05f)
        {
            _sliderId += 1;
            collider2D2.GetComponent<SpriteRenderer>().color = new Color(0, 0.5188679f, 0.1322604f);

            SoundManager.Instance.PlaySoundImmediate(LaunchpadAssets.BeepSound.LoadAsset(), false, 0.8f);

            if (_sliderId != sliders.Length)
            {
                sliders[_sliderId].GetComponent<SpriteRenderer>().color = new Color(0, 1, 0.2549479f);
            }
            else
            {
                statusText.text = "enabled";
                statusText.color = Color.green;

                StartCoroutine(CoStartClose(1f));
                if (TutorialManager.InstanceExists)
                {
                    foreach (var plr in PlayerControl.AllPlayerControls)
                    {
                        if (plr.AmOwner)
                        {
                            continue;
                        }
                        plr.RpcRemoveModifier<HackedModifier>();
                    }
                }
                PlayerControl.LocalPlayer.RpcRemoveModifier<HackedModifier>();
            }
        }
    }
}
