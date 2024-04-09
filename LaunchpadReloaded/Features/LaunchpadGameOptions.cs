using LaunchpadReloaded.API.GameModes;
using LaunchpadReloaded.API.GameOptions;
using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Networking.Color;
using LaunchpadReloaded.Utilities;
using Reactor.Networking.Rpc;

namespace LaunchpadReloaded.Features;

public class LaunchpadGameOptions
{
    public static LaunchpadGameOptions Instance { get; private set; }

    public readonly CustomStringOption GameModes;
    public readonly CustomToggleOption BanCheaters;

    // Voting Types
    public readonly CustomStringOption VotingType;
    public readonly CustomNumberOption MaxVotes;
    public readonly CustomToggleOption AllowVotingForSamePerson;
    public readonly CustomToggleOption AllowConfirmingVotes;
    public readonly CustomToggleOption HideVotingIcons;
    public readonly CustomToggleOption ShowPercentages;
    public readonly CustomOptionGroup VotingGroup;

    // General Options
    public readonly CustomToggleOption OnlyShowRoleColor;
    public readonly CustomToggleOption DisableMeetingTeleport;
    public readonly CustomToggleOption GhostsSeeRoles;
    public readonly CustomOptionGroup GeneralGroup;

    // Battle Royale
    public readonly CustomToggleOption SeekerCharacter;
    public readonly CustomToggleOption ShowKnife;
    public readonly CustomOptionGroup BattleRoyaleGroup;

    // Fun Options
    public readonly CustomToggleOption FriendlyFire;
    public readonly CustomToggleOption UniqueColors;
    public readonly CustomStringOption Character;
    public readonly CustomOptionGroup FunGroup;

    private LaunchpadGameOptions()
    {
        GameModes = new CustomStringOption("Gamemode", 0, ["Default", "Battle Royale"])
        {
            ChangedEvent = CustomGameModeManager.SetGameMode
        };

        VotingType = new CustomStringOption("Voting Type", 0, ["Classic", "Multiple", "Chance", "Combined"]);

        MaxVotes = new CustomNumberOption("Max Votes", 3, 2, 5, 1, NumberSuffixes.None)
        {
            Hidden = () => !VotingTypesManager.CanVoteMultiple()
        };

        ShowPercentages = new CustomToggleOption("Show Percentages", false)
        {
            Hidden = VotingTypesManager.UseChance
        };

        HideVotingIcons = new CustomToggleOption("Hide Voting Icons", false)
        {
            Hidden = () => !VotingTypesManager.UseChance() && !ShowPercentages.Value
        };

        AllowConfirmingVotes = new CustomToggleOption("Allow Confirming Votes", false)
        {
            Hidden = VotingTypesManager.CanVoteMultiple
        };

        AllowVotingForSamePerson = new CustomToggleOption("Allow Voting Same Person Again", true)
        {
            Hidden = () => !VotingTypesManager.CanVoteMultiple()
        };


        VotingGroup = new CustomOptionGroup("Voting Type",
            toggleOpt: [AllowVotingForSamePerson, ShowPercentages, AllowConfirmingVotes, HideVotingIcons],
            stringOpt: [],
            numberOpt: [MaxVotes]);

        BanCheaters = new CustomToggleOption("Ban Cheaters", true)
        {
            ShowInHideNSeek = true
        };

        DisableMeetingTeleport = new CustomToggleOption("Disable Meeting Teleport", false);

        OnlyShowRoleColor = new CustomToggleOption("Reveal Crewmate Roles", false);

        GhostsSeeRoles = new CustomToggleOption("Ghosts See Roles", true);
        GeneralGroup = new CustomOptionGroup("General Options",
            toggleOpt: [BanCheaters, OnlyShowRoleColor, DisableMeetingTeleport, GhostsSeeRoles],
            stringOpt: [],
            numberOpt: []);

        FriendlyFire = new CustomToggleOption("Friendly Fire", false);

        UniqueColors = new CustomToggleOption("Unique Colors", true)
        {
            ShowInHideNSeek = true,
            ChangedEvent = value =>
            {
                if (!AmongUsClient.Instance.AmHost || !value)
                {
                    return;
                }

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (GradientManager.TryGetColor(player.PlayerId, out var grad) && !player.AmOwner)
                    {
                        Rpc<CustomCmdCheckColor>.Instance.Handle(player, new CustomColorData((byte)player.Data.DefaultOutfit.ColorId, grad));
                    }
                }
            }
        };

        Character = new CustomStringOption("Character", 0, ["Default", "Horse", "Long"])
        {
            ChangedEvent = i =>
            {
                foreach (var plr in PlayerControl.AllPlayerControls)
                {
                    plr.SetBodyType(i);
                }
            }
        };

        FunGroup = new CustomOptionGroup("Fun Options",
            toggleOpt: [FriendlyFire, UniqueColors],
            stringOpt: [Character],
            numberOpt: []);

        SeekerCharacter = new CustomToggleOption("Use Seeker Character", true);
        ShowKnife = new CustomToggleOption("Show Knife", true)
        {
            Hidden = () => SeekerCharacter.Value == false
        };

        BattleRoyaleGroup = new CustomOptionGroup("Battle Royale Options",
            toggleOpt: [SeekerCharacter, ShowKnife],
            stringOpt: [],
            numberOpt: [])
        {
            Hidden = () => GameModes.IndexValue != (int)LaunchpadGamemodes.BattleRoyale
        };

        GeneralGroup.Hidden = FunGroup.Hidden = VotingType.Hidden = VotingGroup.Hidden = () => !CustomGameModeManager.IsDefault();

        foreach (var role in CustomRoleManager.CustomRoles)
        {
            if (role.Value is ICustomRole customRole)
            {
                customRole.CreateOptions();
            }
        }
    }

    public static void Initialize()
    {
        Instance = new LaunchpadGameOptions();
    }
}