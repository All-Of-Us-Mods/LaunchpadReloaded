using AmongUs.GameOptions;
using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Utilities;
using MiraAPI.Modifiers;
using System.Linq;
using UnityEngine;

namespace LaunchpadReloaded.Modifiers;

public class RevivedModifier : BaseModifier
{
    public override string ModifierName => "Revived";
    private readonly int _visorColor = Shader.PropertyToID("_VisorColor");
    private Color _ogVisorColor;

    private readonly PlayerTag _revivedTag = new()
    {
        Name = "RevivedTag",
        Text = "Revived",
        Color = LaunchpadPalette.MedicColor,
        IsLocallyVisible = _ => true,
    };

    public override void OnActivate()
    {
        var tagManager = Player.GetTagManager();

        if (tagManager != null)
        {
            var existingTag = tagManager.GetTagByName(_revivedTag.Name);
            if (existingTag.HasValue)
            {
                tagManager.RemoveTag(existingTag.Value);
            }

            tagManager.AddTag(_revivedTag);
        }

        _ogVisorColor = Player.cosmetics.currentBodySprite.BodySprite.material.GetColor(_visorColor);
        Player.cosmetics.currentBodySprite.BodySprite.material.SetColor(_visorColor, LaunchpadPalette.MedicColor);

        if (!Player.Data.IsDead)
        {
            return;
        }

        Player.Revive();

        Player.RemainingEmergencies = GameManager.Instance.LogicOptions.GetNumEmergencyMeetings();
        RoleManager.Instance.SetRole(Player, RoleTypes.Crewmate);
        Player.Data.Role.SpawnTaskHeader(Player);
        Player.MyPhysics.SetBodyType(Player.BodyType);

        if (Player.AmOwner)
        {
            HudManager.Instance.MapButton.gameObject.SetActive(true);
            HudManager.Instance.ReportButton.gameObject.SetActive(true);
            HudManager.Instance.UseButton.gameObject.SetActive(true);
            Player.myTasks.RemoveAt(0);
        }
    }

    public override void OnDeactivate()
    {
        var tagManager = Player?.GetTagManager();

        if (tagManager != null)
        {
            tagManager.RemoveTag(_revivedTag);
        }

        Player!.cosmetics.currentBodySprite.BodySprite.material.SetColor(_visorColor, _ogVisorColor);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        if (MeetingHud.Instance)
        {
            var playerState = MeetingHud.Instance.playerStates.First(plr => plr.TargetPlayerId == Player.PlayerId);
            if (playerState is null)
            {
                return;
            }

            playerState.PlayerIcon.cosmetics.currentBodySprite.BodySprite.material.SetColor(_visorColor, LaunchpadPalette.MedicColor);
        }
    }
}