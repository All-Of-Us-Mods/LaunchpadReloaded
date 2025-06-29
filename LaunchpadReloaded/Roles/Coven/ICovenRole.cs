using MiraAPI.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Coven;

public interface ICovenRole : ICustomRole
{
    ModdedRoleTeams ICustomRole.Team => ModdedRoleTeams.Custom;
    RoleOptionsGroup ICustomRole.RoleOptionsGroup => new("Coven Roles", new Color32(128, 0, 128, 255));
    TeamIntroConfiguration? ICustomRole.IntroConfiguration => new(new Color32(128, 0, 128, 255), "COVEN", "You are a Coven. You work ALONE.");
}