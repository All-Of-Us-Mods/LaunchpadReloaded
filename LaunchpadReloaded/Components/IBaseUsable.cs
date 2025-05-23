namespace LaunchpadReloaded.Components;

public interface IBaseUsable
{
    float UsableDistance { get; }

    float PercentCool { get; }

    ImageNames UseIcon { get; }

    void SetOutline(bool on, bool mainTarget);

    float CanUse(NetworkedPlayerInfo pc, out bool canUse, out bool couldUse);

    void Use();
}