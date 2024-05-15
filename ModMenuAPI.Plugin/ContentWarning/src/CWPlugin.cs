using BepInEx;
using BepInEx.Logging;
using PluginCW.CorePatches;
using MonoMod.RuntimeDetour.HookGen;
using System.Reflection;
using ModMenuAPI.ModMenuItems;

namespace PluginCW;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class BepPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        Plugin.Logger = base.Logger;
#if DEBUG
        ModMenuAPI.HotLoadPlugin.OnLoad();
#endif
        Plugin.Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");

        Plugin.Init();
    }

    private void OnDestroy()
    {
        HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
        ModMenu.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
#if DEBUG
        ModMenuAPI.HotLoadPlugin.Dispose();
#endif
    }
}

internal class Plugin
{
    internal static ManualLogSource Logger { get; set; } = null!;
    internal static void Init()
    {
        CWPlayerPatches.Init();
        CWStatsPatches.Init();
        // CWSpawnActions.Init();
    }
}