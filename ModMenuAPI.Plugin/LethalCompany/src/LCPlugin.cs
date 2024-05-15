using BepInEx;
using BepInEx.Logging;
using PluginLC.CorePatches;
using MonoMod.RuntimeDetour.HookGen;
using System.Reflection;
using ModMenuAPI.ModMenuItems;

namespace PluginLC;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

#if DEBUG
        ModMenuAPI.HotLoadPlugin.OnLoad();
#endif
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");

        LCMiscPatches.Init();
        LCPlayerPatches.Init();
        LCActionPatches.Init();
        LCActionEnemy.Init();
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
