using LaunchpadReloaded.Options;
using MiraAPI.GameOptions;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using MiraAPI.Voting;
using TMPro;
using UnityEngine;
using Helpers = MiraAPI.Utilities.Helpers;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace LaunchpadReloaded.Features.Voting;
public static class VotingTypesManager
{
    public static VotingTypes SelectedType => OptionGroupSingleton<VotingOptions>.Instance.VotingType;

    private static readonly byte[] RecommendedVotes =
    [
        1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5
    ];

    public static int GetDynamicVotes() =>
        (int)Math.Min(
            RecommendedVotes[
                Math.Min(
                    Math.Clamp(
                        Helpers.GetAlivePlayers().Count,
                        0,
                        15),
                    RecommendedVotes.Length)],
            OptionGroupSingleton<VotingOptions>.Instance.MaxVotes.Value);

    private static Dictionary<byte, float> GetChancePercents(List<CustomVote> votes)
    {
        var dict = new Dictionary<byte, float>();

        foreach (var pair in VotingUtils.CalculateNumVotes(votes))
        {
            dict[pair.Key] = pair.Value / votes.Count * 100;
        }

        return dict;
    }

    public static byte GetVotedPlayerByChance(List<CustomVote> votes)
    {
        if (!votes.Any())
        {
            return (byte)SpecialVotes.Skip;
        }

        var rand = new Random();
        List<byte> plrs = [.. votes.Select(vote => vote.Suspect)];
        return plrs[rand.Next(plrs.Count)];
    }

    public static void HandlePopulateResults(List<CustomVote> votes)
    {
        if (!UseChance() && !OptionGroupSingleton<VotingOptions>.Instance.ShowPercentages.Value)
        {
            return;
        }

        var chances = GetChancePercents(votes);

        var skipText = MeetingHud.Instance.SkippedVoting;
        skipText.GetComponentInChildren<TextTranslatorTMP>().Destroy();

        chances.TryGetValue((byte)SpecialVotes.Skip, out var skips);
        skipText.GetComponentInChildren<TextMeshPro>().text = "Skipped\nVoting\n<size=110%>" + Math.Round(skips, 0) + "%</size>";

        foreach (var voteArea in MeetingHud.Instance.playerStates)
        {
            chances.TryGetValue(voteArea.TargetPlayerId, out var val);
            if (voteArea.AmDead || val < 1)
            {
                continue;
            }

            var text = $"{Math.Round(val, 0)}%";
            var chanceThing = Object.Instantiate(voteArea.LevelNumberText.transform.parent, voteArea.transform).gameObject;
            chanceThing.gameObject.name = "ChanceCircle";
            chanceThing.transform.localPosition = new Vector3(1.2801f, -0.2431f, -2.5401f);
            chanceThing.transform.localScale = new Vector3(0.35f, 0.35f, 1);
            chanceThing.transform.GetChild(0).gameObject.SetActive(false);
            chanceThing.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);

            var tmp = chanceThing.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>();
            tmp.fontSize = 3f;
            tmp.text = text;
            tmp.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public static bool UseChance() => SelectedType is VotingTypes.Chance or VotingTypes.Combined;
    public static bool CanVoteMultiple() => SelectedType is VotingTypes.Multiple or VotingTypes.Combined;
}