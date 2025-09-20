using MiraAPI.Modifiers;
using System;
using System.Collections.Generic;

namespace LaunchpadReloaded.Modifiers;

public class DeathData(DateTime deathTime, PlayerControl killer, IEnumerable<PlayerControl> suspects)
    : BaseModifier
{
    public override string ModifierName => "DeathData";
    public override bool HideOnUi => true;

    public DateTime DeathTime { get; } = deathTime;
    public PlayerControl Killer { get; } = killer;
    public IEnumerable<PlayerControl> Suspects { get; } = suspects;
}