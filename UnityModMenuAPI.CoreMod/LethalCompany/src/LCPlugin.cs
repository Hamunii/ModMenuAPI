using BepInEx;
using BepInEx.Logging;
using CoreModLC.CorePatches;
using MonoMod.RuntimeDetour.HookGen;
using System.Reflection;
using UnityModMenuAPI.ModMenuItems;

namespace CoreModLC;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class PluginLC : BaseUnityPlugin
{
    public static PluginLC Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        LCPlayerPatches.Init();
        LCActionPatches.Init();
        
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    private void OnDestroy()
    {
        HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
        ModMenu.RemoveAll();
    }
}
