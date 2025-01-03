﻿using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using LaunchpadReloaded.Features;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using BepInEx.Configuration;
using MiraAPI;
using MiraAPI.PluginLoading;
using MiraAPI.Utilities;
using Reactor.Utilities;

namespace LaunchpadReloaded;

[BepInAutoPlugin("dev.xtracube.launchpad", "LaunchpadReloaded")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class LaunchpadReloadedPlugin : BasePlugin, IMiraPlugin
{
    public Harmony Harmony { get; } = new(Id);
    public ConfigFile GetConfigFile()
    {
        return Config;
    }

    public string OptionsTitleText => "Launchpad";

    public override void Load()
    {
        LaunchpadEventListeners.Initialize();

        Harmony.PatchAll();

        ReactorCredits.Register("Launchpad", Version.Truncate(11, "") ?? Version, true, ReactorCredits.AlwaysShow);
        
        LaunchpadSettings.Initialize();

        Config.Save();
    }
}