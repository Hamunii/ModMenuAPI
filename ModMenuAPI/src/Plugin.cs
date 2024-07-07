using System.Reflection;
using BepInEx;
using ModMenuAPI.MenuGUI;
using ModMenuAPI.Meta; // Don't remove, used by non-debug version
using ModMenuAPI.ModMenuItems;
using MonoMod.RuntimeDetour.HookGen;
using UnityEngine;

namespace ModMenuAPI;

#if !DEBUG
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
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
/// <summary>
/// This is managed by the other mod that is used in combination with this API.<br/>
/// This is a way to get around load order issue and having 2 plugins in one assembly (hot loading issues).
/// </summary>
public static class HotLoadPlugin
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static void OnLoad()
    {
        MMLog.Logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
        Plugin.OnLoad();
    }

    public static void Dispose() => Plugin.Dispose();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
#endif

internal static class Plugin
{
    internal static GameObject myGUIObject = null!;

    internal static void OnLoad()
    {
        MMLog.Log($"{PluginInfo.PLUGIN_GUID} v{PluginInfo.PLUGIN_VERSION} has loaded!");

        InitializeGUI();

        ConfigMenu.Init();
    }

    internal static void InitializeGUI()
    {
        ModMenuGUI.canOpenDevToolsMenu = true;
        if (!ModMenuGUI.menuExists)
        {
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