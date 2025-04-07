using LaunchpadReloaded.Features;
using LaunchpadReloaded.Roles.Outcast;
using MiraAPI.GameEnd;
using MiraAPI.Utilities;

namespace LaunchpadReloaded.GameOver;

public sealed class ExecutionerGameOver : CustomGameOver
{
    public override bool VerifyCondition(PlayerControl playerControl, NetworkedPlayerInfo[] winners)
    {
        return winners is [{ Role: ExecutionerRole }];
    }

    public override void AfterEndGameSetup(EndGameManager endGameManager)
    {
        endGameManager.WinText.text = "<size=80%>Executioner Wins!</size>";
        endGameManager.WinText.color = LaunchpadPalette.ExecutionerColor;
        endGameManager.BackgroundBar.material.SetColor(ShaderID.Color, LaunchpadPalette.ExecutionerColor);
    }
}