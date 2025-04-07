using System.Linq;
using LaunchpadReloaded.Features.Voting;
using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Modifiers.Game.Crewmate;
using LaunchpadReloaded.Options;
using LaunchpadReloaded.Options.Modifiers.Crewmate;
using LaunchpadReloaded.Utilities;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;

namespace LaunchpadReloaded.Events;

public static class MeetingEvents
{
    [RegisterEvent]
    public static void ProcessVotesEvent(ProcessVotesEvent @event)
    {
        if (VotingTypesManager.SelectedType is VotingTypes.Chance or VotingTypes.Combined)
        {
            @event.ExiledPlayer =
                GameData.Instance.GetPlayerById(VotingTypesManager.GetVotedPlayerByChance(@event.Votes));
        }
    }

    [RegisterEvent]
    public static void MeetingSelectEvent(MeetingSelectEvent @event)
    {
        var voteData = PlayerControl.LocalPlayer.GetVoteData();
        if (!voteData || voteData.VotesRemaining == 0) return;
        
        var hasVotedFor = voteData.VotedFor((byte)@event.TargetId);
        var mayorOverride = PlayerControl.LocalPlayer.HasModifier<MayorModifier>() &&
                            OptionGroupSingleton<MayorOptions>.Instance.AllowVotingTwice;
        var voteMultiple = mayorOverride || OptionGroupSingleton<VotingOptions>.Instance.AllowVotingForSamePerson;

        if (voteMultiple && hasVotedFor)
        {
            @event.AllowSelect = true;
        }
    }

    [RegisterEvent]
    public static void HandleVoteEvent(HandleVoteEvent @event)
    {
        if (@event.TargetId == (byte)SpecialVotes.Confirm)
        {
            @event.VoteData.SetRemainingVotes(0);
            @event.Cancel();
            return;
        }
        
        var mayorOverride = @event.Player.HasModifier<MayorModifier>() &&
                            OptionGroupSingleton<MayorOptions>.Instance.AllowVotingTwice;
        var voteMultiple = mayorOverride || OptionGroupSingleton<VotingOptions>.Instance.AllowVotingForSamePerson;

        if (@event.VoteData.VotesRemaining > 0 && voteMultiple && @event.VoteData.VotedFor(@event.TargetId))
        {
            @event.VoteData.DecreaseRemainingVotes(1);
            @event.VoteData.VoteForPlayer(@event.TargetId);
            @event.Cancel();
        }
    }

    [RegisterEvent]
    public static void StartMeetingEvent(StartMeetingEvent @event)
    {
        foreach (var player in GameData.Instance.AllPlayers)
        {
            var state = @event.MeetingHud.playerStates.FirstOrDefault(state => state.TargetPlayerId == player.PlayerId);
            var tagManager = player.Object.GetTagManager();

            if (state != null && tagManager != null)
            {
                tagManager.MeetingStart();
            }

            player.Object.RemoveModifier<DragBodyModifier>();
            if (player.IsDead || player.Disconnected || !player.Object)
            {
                continue;
            }

            var voteData = player.Object.GetVoteData();
            if (voteData && VotingTypesManager.SelectedType is VotingTypes.Multiple or VotingTypes.Combined)
            {
                voteData.VotesRemaining += VotingTypesManager.GetDynamicVotes() - 1;
            }
        }
    }

    [RegisterEvent]
    public static void PopulateResultsEvent(PopulateResultsEvent @event)
    {
        VotingTypesManager.HandlePopulateResults(@event.Votes);
    }
}