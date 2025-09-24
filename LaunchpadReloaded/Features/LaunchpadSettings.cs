using Il2CppSystem;
using System.Linq;
using BepInEx.Configuration;
using LaunchpadReloaded.Components;
using MiraAPI.Hud;
using MiraAPI.LocalSettings;
using MiraAPI.LocalSettings.Attributes;
using Object = Il2CppSystem.Object;

namespace LaunchpadReloaded.Features;

public class LaunchpadSettings : LocalSettingsTab
{
    public override string TabName => "Launchpad";

    public override LocalSettingTabAppearance TabAppearance { get; } = new()
    {
        TabIcon = LaunchpadAssets.HackButton
    };

    [LocalToggleSetting]
    public ConfigEntry<bool> Bloom { get; }

    [LocalToggleSetting]
    public ConfigEntry<bool> CustomBloomSettings { get; }

    [LocalSliderSetting(formatString: "0.0", min: 0.5f, max: 5f)]
    public ConfigEntry<float> BloomSlider { get; }

    [LocalToggleSetting]
    public ConfigEntry<bool> LockedCamera { get; private set; }

    [LocalToggleSetting]
    public ConfigEntry<bool> UniqueDummies { get; }

    [LocalEnumSetting(names:["Bottom Left", "Bottom Right"])]
    public ConfigEntry<ButtonLocation> ButtonLocation { get; }

    public LaunchpadSettings(ConfigFile config) : base(config)
    {
        Bloom = config.Bind("General", "Enable Bloom", true);
        Bloom.SettingChanged += (_, _) => { SetBloom(Bloom.Value); };

        CustomBloomSettings = config.Bind("General", "Custom Bloom Settings", false);
        CustomBloomSettings.SettingChanged += (_, _) => { SetBloom(Bloom.Value); };

        BloomSlider = config.Bind("General", "Bloom Threshold", 1.2f);
        BloomSlider.SettingChanged += (_, _) => { SetBloom(Bloom.Value); };

        LockedCamera = config.Bind("General", "Locked Camera", false);

        ButtonLocation = config.Bind("General", "Button Location", MiraAPI.Hud.ButtonLocation.BottomRight);
        ButtonLocation.SettingChanged += (_, _) =>
        {
            foreach (var button in MiraAPI.PluginLoading.MiraPluginManager.GetPluginByGuid(LaunchpadReloadedPlugin.Id)!.Buttons)
            {
                button.SetButtonLocation(ButtonLocation.Value);
            }
        };

        UniqueDummies = config.Bind("General", "Unique Freeplay Dummies", false);
        UniqueDummies.SettingChanged += (_, _) =>
        {
            if (!TutorialManager.InstanceExists || !AccountManager.InstanceExists)
            {
                return;
            }

            var dummies = UnityEngine.Object.FindObjectsOfType<DummyBehaviour>().ToArray().Reverse().ToList();

            for (var i = 0; i < dummies.Count; i++)
            {
                var dummy = dummies[i];
                if (!dummy.myPlayer)
                {
                    continue;
                }

                dummy.myPlayer.SetName(UniqueDummies.Value
                    ? AccountManager.Instance.GetRandomName()
                    : DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Dummy,
                        Array.Empty<Object>()) + " " + i);
            }
        };
    }


    public static void SetBloom(bool enabled)
    {
        if (!HudManager.InstanceExists)
        {
            return;
        }
        var bloom = HudManager.Instance.PlayerCam.GetComponent<Bloom>();
        if (bloom == null)
        {
            bloom = HudManager.Instance.PlayerCam.gameObject.AddComponent<Bloom>();
        }
        bloom.enabled = enabled;
        bloom.SetBloomByMap();
    }
}
