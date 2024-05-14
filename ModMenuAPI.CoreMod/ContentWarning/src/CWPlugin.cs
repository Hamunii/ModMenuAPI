using BepInEx;
using BepInEx.Logging;
using CoreModCW.CorePatches;
using MonoMod.RuntimeDetour.HookGen;
using System.Reflection;
using ModMenuAPI.ModMenuItems;
using ModMenuAPI.NetRxLoader;

namespace CoreModCW;

#if !DEBUG
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class BepPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        Plugin.Logger = base.Logger;
        Plugin.Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");

        Plugin.Init();
        
    }

    private void OnDestroy()
    {
        HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
        ModMenu.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
    }
}
#else
internal class NetRxPlugin : IHotLoadManagerID1
{
    public void OnLoad()
    {
        Plugin.Logger = Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
        Plugin.Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded via NetRX!");

        Plugin.Init();

    }

    public void Dispose()
    {
        HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
        Plugin.Logger.LogInfo("Calling Items to be Deleted");
        // ModMenu.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
    }
}
#endif

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