using Il2CppSystem;
using LaunchpadReloaded.API.Settings;
using MiraAPI.PluginLoading;
using System.Linq;
using BepInEx.Configuration;
using LaunchpadReloaded.Components;
using Reactor.Utilities;
using Object = Il2CppSystem.Object;

namespace LaunchpadReloaded.Features;

public class LaunchpadSettings
{
    public static LaunchpadSettings? Instance { get; private set; }

    public CustomSetting Bloom { get; }
    public CustomSetting UseCustomBloomSettings { get; }
    public CustomSetting LockedCamera { get; }
    public CustomSetting UniqueDummies { get; }
    public CustomSetting ButtonLocation { get; }

    public ConfigEntry<float> BloomThreshold { get; }

    private LaunchpadSettings()
    {
        var configFile = PluginSingleton<LaunchpadReloadedPlugin>.Instance.GetConfigFile();
        var buttonConfig = configFile.Bind("LP Settings", "Button Location", true, "Move buttons to the left side of the screen");
        var bloomConfig = configFile.Bind("LP Settings", "Bloom", true, "Enable bloom effect");
        var lockedCameraConfig = configFile.Bind("LP Settings", "Locked Camera", false, "Lock camera to player");
        var uniqueDummiesConfig = configFile.Bind("LP Settings", "Unique Freeplay Dummies", true, "Give each dummy a unique name");

        ButtonLocation = new CustomSetting("Buttons On Left", buttonConfig.Value)
        {
            ChangedEvent = val =>
            {
                var plugin = MiraPluginManager.GetPluginByGuid(LaunchpadReloadedPlugin.Id)!;
                foreach (var button in plugin.GetButtons())
                {
                    button.SetButtonLocation(val ? MiraAPI.Hud.ButtonLocation.BottomLeft : MiraAPI.Hud.ButtonLocation.BottomRight);
                }
            }
        };

        Bloom = new CustomSetting("Bloom", bloomConfig.Value)
        {
            ChangedEvent = SetBloom
        };

        UseCustomBloomSettings = new CustomSetting("Use Custom Bloom Settings", false)
        {
            ChangedEvent = _ => { SetBloom(Bloom.Enabled); }
        };

        BloomThreshold = configFile.Bind("LP Settings", "Custom Bloom Threshold", 1.2f, "Bloom threshold (linear)");
        BloomThreshold.SettingChanged += (_, _) => { SetBloom(Bloom.Enabled); };

        LockedCamera = new CustomSetting("Locked Camera", lockedCameraConfig.Value);
        UniqueDummies = new CustomSetting("Unique Freeplay Dummies", uniqueDummiesConfig.Value)
        {
            ChangedEvent = val =>
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

                    dummy.myPlayer.SetName(val ? AccountManager.Instance.GetRandomName() :
                        DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Dummy, Array.Empty<Object>()) + " " + i);
                }
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

    public static void Initialize()
    {
        Instance = new LaunchpadSettings();
    }
}
