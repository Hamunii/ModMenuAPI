using BepInEx;
using UnityEngine;
using MonoMod.RuntimeDetour.HookGen;
using System.Reflection;
using ModMenuAPI.MenuGUI;
using System;
using ModMenuAPI.ModMenuItems;

namespace ModMenuAPI;

#if !DEBUG
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class BepPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        MMLog.Logger = base.Logger;
        Plugin.OnLoad();
    }
    private void OnDestroy() => Plugin.Dispose();
}
#else
public static class HotLoadPlugin
{
    public static void OnLoad()
    {
        MMLog.Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
        Plugin.OnLoad();
    }
    public static void Dispose() => Plugin.Dispose();
}
#endif

internal static class Plugin
{
    internal static GameObject myGUIObject = null!;
    internal static void OnLoad()
    {
        MMLog.Log($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");

        InitializeGUI();
    }

    internal static void InitializeGUI(){
        ModMenuGUI.canOpenDevToolsMenu = true;
        if(!ModMenuGUI.menuExists){
            myGUIObject = new GameObject("ModMenuAPI_GUI");
            UnityEngine.Object.DontDestroyOnLoad(myGUIObject);
            myGUIObject.hideFlags = HideFlags.HideAndDontSave;
            myGUIObject.AddComponent<ModMenuGUI>();
            ModMenuGUI.menuExists = true;

            ModMenuGUI.DevToolsMenuOpen = true;
        }
    }

    internal static void Dispose()
    {
        HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
        ModMenu.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
        UnityEngine.Object.Destroy(myGUIObject);
    }
}