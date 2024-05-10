using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace UnityModMenuAPI.NetRxLoader;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");

        HotLoadManager.Init();
    }

}
