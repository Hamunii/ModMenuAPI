using BepInEx;
using BepInEx.Logging;
using System.Reflection;
using MonoMod.RuntimeDetour.HookGen;
using UnityModMenuAPI.CorePatches.CW;
using UnityModMenuAPI.MenuGUI;
using UnityEngine;
using MonoMod.RuntimeDetour;
using HarmonyLib;
using System;

namespace UnityModMenuAPI;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class UnityModMenuAPI : BaseUnityPlugin
{
    public static UnityModMenuAPI Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static GameObject myGUIObject = null!;
    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        var x = new IsEditorItem();
        InitializeGUI();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    static void InitializeGUI(){
        ModMenu.canOpenDevToolsMenu = true;
        if(!ModMenu.menuExists){
            myGUIObject = new GameObject("DevToolsGUI");
            UnityEngine.Object.DontDestroyOnLoad(myGUIObject);
            myGUIObject.hideFlags = HideFlags.HideAndDontSave;
            myGUIObject.AddComponent<ModMenu>();
            ModMenu.menuExists = true;
        }
    }
}
