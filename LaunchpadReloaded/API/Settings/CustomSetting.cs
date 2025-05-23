using Reactor.Localization.Utilities;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace LaunchpadReloaded.API.Settings;

public class CustomSetting
{
    public string Text { get; }
    public StringNames StringName { get; }
    public bool Enabled;
    public Action<bool>? ChangedEvent = null;

    private GameObject? _buttonObject;

    public CustomSetting(string name, bool value)
    {
        Text = name;
        StringName = CustomStringName.CreateAndRegister(name);
        Enabled = value;

        CustomSettingsManager.CustomSettings.Add(this);
    }

    public void CreateButton(OptionsMenuBehaviour optionsMenu, Transform parent)
    {
        var generalTab = optionsMenu.transform.FindChild("GeneralTab");
        var button = generalTab.FindChild("ChatGroup").FindChild("CensorChatButton");
        _buttonObject = UnityEngine.Object.Instantiate(button, parent).gameObject;
        _buttonObject.name = Text;

        var tb = _buttonObject.GetComponent<ToggleButtonBehaviour>();
        tb.BaseText = StringName;
        tb.UpdateText(Enabled);

        var pb = _buttonObject.GetComponent<PassiveButton>();
        pb.OnClick.RemoveAllListeners();
        pb.OnClick.AddListener((UnityAction)ClickHandler);
    }

    private void ClickHandler()
    {
        Enabled = !Enabled;
        _buttonObject?.GetComponent<ToggleButtonBehaviour>().UpdateText(Enabled);

        ChangedEvent?.Invoke(Enabled);
    }
}