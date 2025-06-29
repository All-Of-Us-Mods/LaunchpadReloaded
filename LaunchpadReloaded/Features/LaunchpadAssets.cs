using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using UnityEngine;

namespace LaunchpadReloaded.Features;

public static class LaunchpadAssets
{
    public static readonly AssetBundle Bundle = AssetBundleManager.Load("assets");

    // Shaders
    public static readonly LoadableAsset<Shader> BloomShader = new LoadableBundleAsset<Shader>("Bloom.shader", Bundle);

    // Materials
    public static readonly LoadableAsset<Material> GradientMaterial = new LoadableBundleAsset<Material>("GradientPlayerMaterial", Bundle);
    public static readonly LoadableAsset<Material> MaskedGradientMaterial = new LoadableBundleAsset<Material>("MaskedGradientMaterial", Bundle);

    // Sprites
    public static readonly LoadableAsset<Sprite> BlankButton = new LoadableBundleAsset<Sprite>("BlankButton", Bundle);
    public static readonly LoadableAsset<Sprite> CallButton = new LoadableBundleAsset<Sprite>("CallMeeting.png", Bundle);
    public static readonly LoadableAsset<Sprite> DissectButton = new LoadableResourceAsset("TORWLaunchpad.Resources.Dissect.png");
    public static readonly LoadableAsset<Sprite> DragButton = new LoadableBundleAsset<Sprite>("Drag.png", Bundle);
    public static readonly LoadableAsset<Sprite> DropButton = new LoadableBundleAsset<Sprite>("Drop.png", Bundle);
    public static readonly LoadableAsset<Sprite> HackButton = new LoadableBundleAsset<Sprite>("Hack.png", Bundle);
    public static readonly LoadableAsset<Sprite> HideButton = new LoadableBundleAsset<Sprite>("Clean.png", Bundle);
    public static readonly LoadableAsset<Sprite> InjectButton = new LoadableResourceAsset("TORWLaunchpad.Resources.Inject.png");
    public static readonly LoadableAsset<Sprite> InstinctButton = new LoadableResourceAsset("TORWLaunchpad.Resources.Instinct.png");
    public static readonly LoadableAsset<Sprite> InvestigateButton = new LoadableResourceAsset("TORWLaunchpad.Resources.Investigate.png");
    public static readonly LoadableAsset<Sprite> DigVentButton = new LoadableResourceAsset("TORWLaunchpad.Resources.DigVent.png");
    public static readonly LoadableAsset<Sprite> JesterIcon = new LoadableResourceAsset("TORWLaunchpad.Resources.Jester.png");
    public static readonly LoadableAsset<Sprite> MapButton = new LoadableBundleAsset<Sprite>("Map.png", Bundle);
    public static readonly LoadableAsset<Sprite> ReviveButton = new LoadableBundleAsset<Sprite>("Revive.png", Bundle);
    public static readonly LoadableAsset<Sprite> ScannerButton = new LoadableBundleAsset<Sprite>("Place_Scanner.png", Bundle);
    public static readonly LoadableAsset<Sprite> ShootButton = new LoadableBundleAsset<Sprite>("Shoot.png", Bundle);
    public static readonly LoadableAsset<Sprite> TrackButton = new LoadableBundleAsset<Sprite>("Track.png", Bundle);
    public static readonly LoadableAsset<Sprite> ZoomButton = new LoadableBundleAsset<Sprite>("Zoom.png", Bundle);
    public static readonly LoadableAsset<Sprite> SealButton = new LoadableResourceAsset("TORWLaunchpad.Resources.SealVent.png");
    public static readonly LoadableAsset<Sprite> SwapButton = new LoadableResourceAsset("TORWLaunchpad.Resources.Swap.png");
    public static readonly LoadableAsset<Sprite> FreezeButton = new LoadableResourceAsset("TORWLaunchpad.Resources.Freeze.png");
    public static readonly LoadableAsset<Sprite> SoulButton = new LoadableResourceAsset("TORWLaunchpad.Resources.StealSoul.png");
    public static readonly LoadableAsset<Sprite> GambleButton = new LoadableResourceAsset("TORWLaunchpad.Resources.Gamble.png");
    public static readonly LoadableAsset<Sprite> DeadlockButton = new LoadableResourceAsset("TORWLaunchpad.Resources.Deadlock.png");

    public static readonly LoadableAsset<Sprite> NotepadSprite = new LoadableResourceAsset("TORWLaunchpad.Resources.NotepadButton.png");
    public static readonly LoadableAsset<Sprite> NotepadActiveSprite = new LoadableResourceAsset("TORWLaunchpad.Resources.NotepadButtonActive.png");

    // Banner Sprites
    public static readonly LoadableAsset<Sprite> CaptainBanner = new LoadableResourceAsset("TORWLaunchpad.Resources.Banners.Captain.png");
    public static readonly LoadableAsset<Sprite> DetectiveBanner = new LoadableResourceAsset("TORWLaunchpad.Resources.Banners.Detective.png");
    public static readonly LoadableAsset<Sprite> HackerBanner = new LoadableResourceAsset("TORWLaunchpad.Resources.Banners.Hacker.png");
    public static readonly LoadableAsset<Sprite> JanitorBanner = new LoadableResourceAsset("TORWLaunchpad.Resources.Banners.Janitor.png");
    public static readonly LoadableAsset<Sprite> JesterBanner = new LoadableResourceAsset("TORWLaunchpad.Resources.Banners.Jester.png");
    public static readonly LoadableAsset<Sprite> MayorBanner = new LoadableResourceAsset("TORWLaunchpad.Resources.Banners.Mayor.png");
    public static readonly LoadableAsset<Sprite> MedicBanner = new LoadableResourceAsset("TORWLaunchpad.Resources.Banners.Medic.png");
    public static readonly LoadableAsset<Sprite> SheriffBanner = new LoadableResourceAsset("TORWLaunchpad.Resources.Banners.Sheriff.png");
    public static readonly LoadableAsset<Sprite> SurgeonBanner = new LoadableResourceAsset("TORWLaunchpad.Resources.Banners.Surgeon.png");

    // Object Sprites
    public static readonly LoadableAsset<Sprite> Arrow = new LoadableResourceAsset("TORWLaunchpad.Resources.Arrow.png");
    public static readonly LoadableAsset<Sprite> Bone = new LoadableResourceAsset("TORWLaunchpad.Resources.Bone.png");
    public static readonly LoadableAsset<Sprite> Footstep = new LoadableResourceAsset("TORWLaunchpad.Resources.Footstep.png");
    public static readonly LoadableAsset<Sprite> VentTape = new LoadableResourceAsset("TORWLaunchpad.Resources.VentTape.png");
    public static readonly LoadableAsset<Sprite> VentTapePolus = new LoadableResourceAsset("TORWLaunchpad.Resources.VentTapePolus.png");
    public static readonly LoadableAsset<Sprite> KnifeHandSprite = new LoadableBundleAsset<Sprite>("KnifeHand.png", Bundle);
    public static readonly LoadableAsset<Sprite> NodeSprite = new LoadableBundleAsset<Sprite>("Node.png", Bundle);
    public static readonly LoadableAsset<Sprite> ScannerSprite = new LoadableBundleAsset<Sprite>("Scanner.png", Bundle);

    public static readonly LoadableAsset<Sprite> DeadlockHonor = new LoadableResourceAsset("TORWLaunchpad.Resources.DeadlockHonor.png");
    public static readonly LoadableAsset<Sprite> DeadlockTarget = new LoadableResourceAsset("TORWLaunchpad.Resources.DeadlockTarget.png");
    public static readonly LoadableAsset<Sprite> DeadlockVignette = new LoadableResourceAsset("TORWLaunchpad.Resources.DeadlockVignette.png");
    public static readonly LoadableAsset<Sprite> FrozenBodyOverlay = new LoadableResourceAsset("TORWLaunchpad.Resources.BodyFrozenOverlay.png");
    public static readonly LoadableAsset<Sprite> TraitorIcon = new LoadableResourceAsset("TORWLaunchpad.Resources.Traitor.png");
    public static readonly LoadableAsset<Sprite> TeleportButton = new LoadableResourceAsset("TORWLaunchpad.Resources.TeleportButton.png");
    public static readonly LoadableAsset<Sprite> DarkFairyButton = new LoadableResourceAsset("TORWLaunchpad.Resources.Darken.png");

    // Sounds
    public static readonly LoadableAsset<AudioClip> BeepSound = new LoadableBundleAsset<AudioClip>("Beep.wav", Bundle);
    public static readonly LoadableAsset<AudioClip> InjectSound = new LoadableBundleAsset<AudioClip>("Inject.mp3", Bundle);
    public static readonly LoadableAsset<AudioClip> DissectSound = new LoadableBundleAsset<AudioClip>("Dissect.mp3", Bundle);
    public static readonly LoadableAsset<AudioClip> HackingSound = new LoadableBundleAsset<AudioClip>("HackAmbience.mp3", Bundle);
    public static readonly LoadableAsset<AudioClip> PingSound = new LoadableBundleAsset<AudioClip>("Ping.mp3", Bundle);
    public static readonly LoadableAsset<AudioClip> MoneySound = new LoadableBundleAsset<AudioClip>("MoneySound.mp3", Bundle);
    public static readonly LoadableAsset<AudioClip> BuzzerSound = new LoadableBundleAsset<AudioClip>("Buzzer.mp3", Bundle);
    public static readonly LoadableAsset<AudioClip> DigSound = new LoadableBundleAsset<AudioClip>("Dig.mp3", Bundle);
    public static readonly LoadableAsset<AudioClip> TapeSound = new LoadableBundleAsset<AudioClip>("Tape.mp3", Bundle);
    public static readonly LoadableAsset<AudioClip> SwooshSound = new LoadableBundleAsset<AudioClip>("Swoosh.mp3", Bundle);
    public static readonly LoadableAsset<AudioClip> ReaperSound = new LoadableBundleAsset<AudioClip>("CollectSoul.mp3", Bundle);

    // Deadlock Sounds
    public static readonly LoadableAsset<AudioClip> DeadlockAmbience = new LoadableBundleAsset<AudioClip>("DeadlockAmbience.wav", Bundle);
    public static readonly LoadableAsset<AudioClip> DeadlockFadeIn = new LoadableBundleAsset<AudioClip>("DeadlockFadeIn.wav", Bundle);
    public static readonly LoadableAsset<AudioClip> DeadlockFadeOut = new LoadableBundleAsset<AudioClip>("DeadlockFadeOut.wav", Bundle);
    public static readonly LoadableAsset<AudioClip> DeadlockClockLeft = new LoadableBundleAsset<AudioClip>("DeadlockClockLeft.wav", Bundle);
    public static readonly LoadableAsset<AudioClip> DeadlockClockRight = new LoadableBundleAsset<AudioClip>("DeadlockClockRight.wav", Bundle);
    public static readonly LoadableAsset<AudioClip> DeadlockMark = new LoadableBundleAsset<AudioClip>("DeadlockMark.wav", Bundle);
    public static readonly LoadableAsset<AudioClip> DeadlockKillConfirmal = new LoadableBundleAsset<AudioClip>("DeadlockKillConfirmal.wav", Bundle);

    // Other
    public static readonly LoadableAsset<GameObject> DetectiveGame = new LoadableBundleAsset<GameObject>("JournalMinigame", Bundle);
    public static readonly LoadableAsset<GameObject> RoleMinigame = new LoadableBundleAsset<GameObject>("RoleGuessingMinigame", Bundle);
    public static readonly LoadableAsset<GameObject> ReaperDisplay = new LoadableBundleAsset<GameObject>("ReaperSoulDisplay", Bundle);
    public static readonly LoadableAsset<GameObject> Notepad = new LoadableBundleAsset<GameObject>("Notepad", Bundle);
    public static readonly LoadableAsset<GameObject> NodeGame = new LoadableBundleAsset<GameObject>("NodeMinigame", Bundle);

    public static readonly LoadableAsset<GameObject> PlayerTags = new LoadableBundleAsset<GameObject>("PlayerTags", Bundle);
    public static readonly LoadableAsset<Sprite> TagBackground = new LoadableResourceAsset("TORWLaunchpad.Resources.TagBackground.png");
}