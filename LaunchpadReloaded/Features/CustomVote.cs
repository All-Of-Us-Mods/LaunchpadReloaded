﻿namespace LaunchpadReloaded.Features;
public struct CustomVote(byte voter, byte suspect)
{
    public readonly byte Voter = voter;
    public readonly byte Suspect = suspect;
}