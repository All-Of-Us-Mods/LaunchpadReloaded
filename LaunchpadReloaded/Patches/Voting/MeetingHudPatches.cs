using HarmonyLib;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Voting;
using LaunchpadReloaded.Options;
using LaunchpadReloaded.Utilities;
using MiraAPI.GameOptions;
using Reactor.Utilities.Extensions;
using System.Linq;
using MiraAPI.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace LaunchpadReloaded.Patches.Voting;

[HarmonyPatch(typeof(MeetingHud))]
public static class MeetingHudPatches
{
    private static GameObject? _typeText;
    private static PlayerVoteArea? _confirmVotes;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.Start))]
    public static void AwakePostfix(MeetingHud __instance)
    {
        if (NotepadHud.Instance != null)
        {
            NotepadHud.Instance.UpdateAspectPos();
        }

        if (_typeText == null)
        {
            _typeText = Object.Instantiate(__instance.TimerText, __instance.TimerText.transform.parent).gameObject;
            _typeText.GetComponent<TextTranslatorTMP>().Destroy();
            _typeText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Left;
            _typeText.transform.localPosition = new Vector3(-1.4327f, -2.1964f, 0);
            _typeText.name = "VoteTypeText";
            _typeText.gameObject.SetActive(false);
        }

        if (_confirmVotes == null && (VotingTypesManager.CanVoteMultiple() ||
                                      OptionGroupSingleton<VotingOptions>.Instance.AllowConfirmingVotes.Value))
        {
            _confirmVotes = Object.Instantiate(__instance.SkipVoteButton, __instance.SkipVoteButton.transform.parent);
            _confirmVotes.gameObject.name = "ConfirmVotesBtn";
            _confirmVotes.SetTargetPlayerId((byte)SpecialVotes.Confirm);
            _confirmVotes.Parent = __instance;

            var confirmText = _confirmVotes.gameObject.GetComponentInChildren<TextMeshPro>();
            _confirmVotes.gameObject.GetComponentInChildren<TextTranslatorTMP>().Destroy();
            confirmText.text = "CONFIRM VOTES";
            __instance.SkipVoteButton.transform.position += new Vector3(0, 0.18f, 0);
            _confirmVotes.transform.position -= new Vector3(0, 0.1f, 0);
            foreach (var plr in __instance.playerStates.AddItem(__instance.SkipVoteButton))
            {
                plr.gameObject.GetComponentInChildren<PassiveButton>().OnClick
                    .AddListener((UnityAction)(() => _confirmVotes.ClearButtons()));
            }
        }
        else if (_confirmVotes != null && !VotingTypesManager.CanVoteMultiple() &&
                 !OptionGroupSingleton<VotingOptions>.Instance.AllowConfirmingVotes.Value)
        {
            _confirmVotes.gameObject.Destroy();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.Update))]
    public static void UpdatePatch(MeetingHud __instance)
    {
        if (HackerUtilities.AnyPlayerHacked())
        {
            HackerUtilities.ForceEndHack();
        }

        var voteData = PlayerControl.LocalPlayer.GetVoteData();
        if (voteData == null || _typeText == null)
        {
            return;
        }

        var tmp = _typeText.GetComponent<TextMeshPro>();
        tmp.text = VotingTypesManager.SelectedType != VotingTypes.Classic
            ? $"<size=160%>{voteData.VotesRemaining} votes left</size>\nVoting Type: {VotingTypesManager.SelectedType}"
            : $"<size=160%>{voteData.VotesRemaining} votes left</size>";

        var logicOptionsNormal = GameManager.Instance.LogicOptions.TryCast<LogicOptionsNormal>();

        if (logicOptionsNormal is not null)
        {
            var votingTime = logicOptionsNormal.GetVotingTime();
            if (votingTime > 0)
            {
                var num2 = __instance.discussionTimer - logicOptionsNormal.GetDiscussionTime();
                if (AmongUsClient.Instance.AmHost && num2 >= votingTime)
                {
                    foreach (var player in MiraAPI.Utilities.Helpers.GetAlivePlayers().Where(x => x.GetVoteData().VotesRemaining > 0))
                    {
                        __instance.CastVote(player.PlayerId, (byte)SpecialVotes.Confirm);
                    }
                }
            }
        }

        if (PlayerControl.LocalPlayer.Data.IsDead)
        {
            if (_confirmVotes != null)
            {
                _confirmVotes.SetDisabled();
            }

            _typeText.gameObject.SetActive(false);
            if (__instance.state != MeetingHud.VoteStates.Results)
            {
                return;
            }

            foreach (var voteArea in __instance.playerStates.Where(state => !state.resultsShowing))
            {
                voteArea.ClearForResults();
            }

            return;
        }

        switch (__instance.state)
        {
            case MeetingHud.VoteStates.Voted:
            case MeetingHud.VoteStates.NotVoted:
                if (PlayerControl.LocalPlayer.GetVoteData().VotesRemaining == 0)
                {
                    _typeText.gameObject.SetActive(false);
                    if (_confirmVotes != null)
                    {
                        _confirmVotes.SetDisabled();
                    }
                }
                else
                {
                    _typeText.gameObject.SetActive(true);
                    if (_confirmVotes != null)
                    {
                        _confirmVotes.SetEnabled();
                    }
                }

                break;

            case MeetingHud.VoteStates.Results:
                if (_confirmVotes != null)
                {
                    _confirmVotes.SetDisabled();
                }

                _typeText.gameObject.SetActive(false);
                foreach (var voteArea in __instance.playerStates.Where(state => !state.resultsShowing))
                    voteArea.ClearForResults();
                break;

            default:
                if (_confirmVotes != null)
                {
                    _confirmVotes.SetDisabled();
                }

                _typeText.gameObject.SetActive(false);
                break;
        }
    }
}