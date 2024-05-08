using BepInEx;
using BepInEx.Logging;
using UnityModMenuAPI.CorePatches.CW;
using UnityModMenuAPI.MenuGUI;
using UnityEngine;

namespace UnityModMenuAPI;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static GameObject myGUIObject = null!;
    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        CWInit.Init();
        
        InitializeGUI();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    static void InitializeGUI(){
        ModMenuGUI.canOpenDevToolsMenu = true;
        if(!ModMenuGUI.menuExists){
            myGUIObject = new GameObject("DevToolsGUI");
            UnityEngine.Object.DontDestroyOnLoad(myGUIObject);
            myGUIObject.hideFlags = HideFlags.HideAndDontSave;
            myGUIObject.AddComponent<ModMenuGUI>();
            ModMenuGUI.menuExists = true;

            ModMenuGUI.DevToolsMenuOpen = true;
        }
    }
}
