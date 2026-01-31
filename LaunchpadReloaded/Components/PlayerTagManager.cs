using LaunchpadReloaded.Features;
using MiraAPI.Utilities;
using MonoMod.Utils;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LaunchpadReloaded.Components;

[RegisterInIl2Cpp]
public class PlayerTagManager(IntPtr ptr) : MonoBehaviour(ptr)
{
    public PlayerControl player = null!;
    public PlayerVoteArea? voteArea;

    public Transform tagHolder = null!;
    public GameObject tagTemplate = null!;
    public GridLayoutGroup gridLayout = null!;

    public Transform meetingTagHolder = null!;

    public readonly Dictionary<PlayerTag, GameObject> Tags = [];
    private readonly Dictionary<PlayerTag, GameObject> _oldTags = [];

    private bool _inMeeting;

    public void Awake()
    {
        player = GetComponent<PlayerControl>();

        tagHolder = Instantiate(LaunchpadAssets.PlayerTags.LoadAsset(), player.transform).transform;
        gridLayout = tagHolder.GetComponent<GridLayoutGroup>();

        tagTemplate = tagHolder.transform.GetChild(0).gameObject;
        tagTemplate.SetActive(false);
        tagTemplate.transform.SetParent(player.transform); // Just to store it as a template

        UpdatePosition();
    }

    public void MeetingStart()
    {
        var meeting = MeetingHud.Instance;
        if (meeting != null)
        {
            _inMeeting = true;

            voteArea = meeting.playerStates.FirstOrDefault(plr => plr.TargetPlayerId == player.PlayerId);

            if (voteArea != null)
            {
                meetingTagHolder = Instantiate(LaunchpadAssets.PlayerTags.LoadAsset(), voteArea.transform).transform;
                meetingTagHolder.transform.GetChild(0).gameObject.DestroyImmediate();
                meetingTagHolder.gameObject.layer = voteArea.gameObject.layer;

                Dictionary<PlayerTag, GameObject> toAdd = new();

                foreach (var tagPair in Tags)
                {
                    _oldTags.Add(tagPair.Key, tagPair.Value);

                    var cloneTag = Instantiate(tagPair.Value, meetingTagHolder, false);
                    cloneTag.layer = meetingTagHolder.gameObject.layer;
                    cloneTag.transform.GetChild(0).gameObject.layer = cloneTag.layer;

                    toAdd.Add(tagPair.Key, cloneTag);
                }

                Tags.Clear();
                Tags.AddRange(toAdd);

                meetingTagHolder.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

                UpdatePosition();
            }
        }
    }

    public void MeetingEnd()
    {
        _inMeeting = false;

        Tags.Clear();

        foreach (var tagPair in _oldTags)
        {
            Tags[tagPair.Key] = tagPair.Value;
        }

        _oldTags.Clear();

        UpdatePosition();
    }

    public void Update()
    {
        foreach (var tagPair in Tags)
        {
            if (!tagPair.Value)
            {
                continue;
            }

            var visible = tagPair.Key.IsLocallyVisible(player)
                          && (_inMeeting ? voteArea!.NameText.gameObject.active : player.cosmetics.nameText.gameObject.active);

            if (!tagPair.Value.active && visible)
            {
                UpdatePosition();
            }

            tagPair.Value.SetActive(visible);
        }
    }

    public int GetActiveCount()
    {
        return Tags.Keys.Count(obj => obj.IsLocallyVisible(player));
    }

    public void UpdatePosition()
    {
        var columnCount = Mathf.CeilToInt((float)GetActiveCount() / gridLayout.constraintCount);

        if (columnCount <= 0)
        {
            return;
        }

        var colorblind = player.cosmetics.colorBlindText.gameObject.active;
        var nameTextY = (colorblind ? 0.85f : 0.69f) + columnCount * 0.27f;
        var holderY = 0.53f + columnCount * 0.15f;

        if (_inMeeting)
        {
            var nameTextPos = voteArea!.NameText.transform.localPosition;

            holderY = -0.07f + columnCount * -0.05f;
            nameTextY = 0.025f + columnCount * 0.1f;

            voteArea!.ColorBlindName.transform.localPosition = new Vector3(-0.9058f, -0.1666f, -0.01f);
            voteArea!.NameText.transform.localPosition = new Vector3(nameTextPos.x, nameTextY, nameTextPos.z);
            meetingTagHolder.transform.localPosition = new Vector3(nameTextPos.x, holderY, 0);
        }
        else
        {
            tagHolder.transform.localPosition = new Vector3(0, holderY, -0.35f);
            player.cosmetics.nameTextContainer.transform.localPosition = new Vector3(0, nameTextY, -0.5f);
        }
    }

    [HideFromIl2Cpp]
    public PlayerTag? GetTagByName(string tagName)
    {
        return Tags.Keys.FirstOrDefault(playerTag => playerTag.Name == tagName);
    }

    [HideFromIl2Cpp]
    public void RemoveTag(PlayerTag tagStruct)
    {
        if (!Tags.TryGetValue(tagStruct, out var playerTag))
        {
            return;
        }

        playerTag.gameObject.DestroyImmediate();
        Tags.Remove(tagStruct);

        UpdatePosition();
    }

    public void ClearTags()
    {
        foreach (var plrTag in Tags)
        {
            RemoveTag(plrTag.Key);
        }
    }

    [HideFromIl2Cpp]
    public void AddTag(PlayerTag plrTag)
    {
        if (Tags.ContainsKey(plrTag))
        {
            return;
        }

        var newTag = Instantiate(tagTemplate, tagHolder.transform);
        var bgRend = newTag.GetComponent<SpriteRenderer>();
        var tagText = newTag.transform.GetChild(0).GetComponent<TextMeshPro>();
        tagText.text = plrTag.Text;
        newTag.name = plrTag.Name;

        if (plrTag.Color != Color.clear)
        {
            tagText.color = plrTag.Color.LightenColor(0.25f);
            bgRend.color = plrTag.Color;
        }

        Tags.Add(plrTag, newTag);

        UpdatePosition();
    }
}

public record struct PlayerTag(string Name, string Text, Color Color)
{
    public Func<PlayerControl, bool> IsLocallyVisible { get; set; } = _ => false;
}
