using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Patches;
using MiraAPI;
using MiraAPI.PluginLoading;
using MiraAPI.Utilities;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

namespace LaunchpadReloaded;

[BepInAutoPlugin("dev.xtracube.launchpad", "TOR-W Launchpad")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class LaunchpadReloadedPlugin : BasePlugin, IMiraPlugin
{
    private Harmony Harmony { get; } = new(Id);

    public ConfigFile GetConfigFile()
    {
        return Config;
    }

    public string OptionsTitleText => "TOR-W\nLaunchpad";

    public override void Load()
    {
        Harmony.PatchAll();

        ReactorCredits.Register("TOR-W Launchpad", Version.Truncate(11, "") ?? Version, true,
            ReactorCredits.AlwaysShow);
        LaunchpadSettings.Initialize();

        Config.Save();
    }
}