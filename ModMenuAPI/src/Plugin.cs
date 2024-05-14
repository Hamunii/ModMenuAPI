using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using MonoMod.RuntimeDetour.HookGen;
using System.Reflection;
using ModMenuAPI.MenuGUI;
using ModMenuAPI.ModMenuItems;
using ModMenuAPI.NetRxLoader;

namespace ModMenuAPI;
#if !DEBUG
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class BepPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        Plugin.Logger = base.Logger;
        Plugin.Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");

// #if DEBUG && false // BepInEx ScriptEngine doesn't play well with dependencies that are also supposed to be live reloaded.
//         DebugLCActions.Init();
// #endif
// #if DEBUG && false
//         CoreModCW.CorePatches.DebugCW.Init();
// #endif

        Plugin.InitializeGUI();

    }

    private void OnDestroy()
    {
        HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
        ModMenu.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
        Destroy(Plugin.myGUIObject);
    }

    
}
#else
internal class NetRxPlugin : IHotLoadManagerID0
{
    public void OnLoad()
    {
        Plugin.Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
        Plugin.Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded via NetRx!");

        Plugin.InitializeGUI();

        // DebugCW.Init();
    }

    public void Dispose()
    {
        HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
        // ModMenu.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());

        Object.Destroy(Plugin.myGUIObject);
    }
}
#endif

internal class Plugin
{
    internal static ManualLogSource Logger { get; set; } = null!;
    internal static GameObject myGUIObject = null!;
    internal static void InitializeGUI(){
        ModMenuGUI.canOpenDevToolsMenu = true;
        if(!ModMenuGUI.menuExists){
            myGUIObject = new GameObject("ModMenuAPI_GUI");
            Object.DontDestroyOnLoad(myGUIObject);
            myGUIObject.hideFlags = HideFlags.HideAndDontSave;
            myGUIObject.AddComponent<ModMenuGUI>();
            ModMenuGUI.menuExists = true;

            ModMenuGUI.DevToolsMenuOpen = true;
        }
    }
}