using LaunchpadReloaded.Modifiers;
using Reactor.Utilities.Attributes;
using System;
using System.Linq;
using MiraAPI.Modifiers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace LaunchpadReloaded.Components;

[RegisterInIl2Cpp]
public class JournalMinigame(nint ptr) : Minigame(ptr)
{
    public TextMeshPro deadPlayerInfo = null!;
    public PassiveButton closeButton = null!;
    public PassiveButton outsideButton = null!;
    public SpriteRenderer deadBodyIcon = null!;

    private void Awake()
    {
        outsideButton = transform.FindChild("Background/OutsideCloseButton").GetComponent<PassiveButton>();
        closeButton = transform.FindChild("CloseButton").GetComponent<PassiveButton>();
        deadPlayerInfo = transform.FindChild("BodyInfo/DeadPlayerInfo").GetComponent<TextMeshPro>();
        deadBodyIcon = transform.FindChild("BodyInfo/Icon").GetComponent<SpriteRenderer>();

        closeButton.OnClick.AddListener((UnityAction)(() => Close()));
        outsideButton.OnClick.AddListener((UnityAction)(() => Close()));
    }

    public void Open(PlayerControl deadPlayer)
    {
        var deathData = deadPlayer.GetModifier<DeathData>();

        if (deathData == null)
        {
            return;
        }

        var suspectTemplate = gameObject.transform.FindChild("SuspectTemplate");
        var suspectsHolder = gameObject.transform.FindChild("Suspects");

        var timeSinceDeath = DateTime.UtcNow.Subtract(deathData.DeathTime);
        deadPlayerInfo.text = timeSinceDeath.Minutes < 1 ? $"{deadPlayer.Data.PlayerName}\n<size=70%>Died {timeSinceDeath.Seconds} seconds ago</size>" :
            $"{deadPlayer.Data.PlayerName}\n<size=70%>Died {timeSinceDeath.Minutes} minutes ago</size>";

        deadPlayer.SetPlayerMaterialColors(deadBodyIcon);

        //if (GameManager.Instance.LogicFlow.GetPlayerCounts().Item1 < OptionGroupSingleton<DetectiveOptions>.Instance.SuspectCount - 1 || OptionGroupSingleton<DetectiveOptions>.Instance.HideSuspects)
        //{
        //    suspectsHolder.gameObject.SetActive(false);
        //    gameObject.transform.FindChild("BottomText").gameObject.SetActive(false);
        //    gameObject.transform.FindChild("SuspectsTitle").GetComponent<TextMeshPro>().text = "Suspects cannot be shown.";
        //    Begin(null);
        //    return;
        //}

        var rnd = new System.Random();
        foreach (var suspect in deathData.Suspects.OrderBy(_ => rnd.Next()))
        {
            var newTemplate = Instantiate(suspectTemplate, suspectsHolder);
            suspect.SetPlayerMaterialColors(newTemplate.GetComponent<SpriteRenderer>());
            newTemplate.gameObject.SetActive(true);
            newTemplate.transform.position = new Vector3(0, 0, -120);

            var nameTag = newTemplate.transform.GetChild(0).GetComponent<TextMeshPro>();
            nameTag.text = suspect.Data.PlayerName;
        }

        Begin(null);
    }
}