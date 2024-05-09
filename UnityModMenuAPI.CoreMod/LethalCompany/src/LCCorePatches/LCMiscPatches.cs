using System;
using System.Collections;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using UnityEngine;
using UnityModMenuAPI.ModMenuItems;

namespace CoreModLC.CorePatches;

class LCMiscPatches
{
    const string menuMisc = "Misc";
    internal static void Init()
    {
        ModMenu.RegisterModMenuItem(new IsEditorPatch(), menuMisc);
        ModMenu.RegisterModMenuItem(new InfiniteCreditsPatch(), menuMisc);
        
    }
}

class IsEditorPatch : ModMenuButtonToggle
{
    readonly ModMenuItemMetadata meta = new("IsEditor"){ InvokeOnInit = true };
    public override ModMenuItemMetadata Metadata => meta;
    private static Hook isEditorHook = new Hook(AccessTools.DeclaredPropertyGetter(typeof(Application), nameof(Application.isEditor)), Override_Application_isEditor, new HookConfig(){ ManualApply = true });

    protected override void OnEnable()
    {
        if(isEditorHook == null){
            Plugin.Logger.LogInfo("isEditorHook is null!");
            return;
        }
        isEditorHook.Apply();
    }
    protected override void OnDisable()
    {
        isEditorHook?.Undo();
    }
    private static bool Override_Application_isEditor(Func<bool> orig){
        orig();
        return true;
    }
}


internal class InfiniteCreditsPatch : ModMenuButtonToggle
{
    readonly ModMenuItemMetadata meta = new("Infinite Credits"){ InvokeOnInit = true };
    public override ModMenuItemMetadata Metadata => meta;
    protected override void OnEnable(){ On.Terminal.RunTerminalEvents += InfiniteCredits_Terminal_RunTerminalEvents; }
    protected override void OnDisable(){ On.Terminal.RunTerminalEvents -= InfiniteCredits_Terminal_RunTerminalEvents; }

    private static void InfiniteCredits_Terminal_RunTerminalEvents(On.Terminal.orig_RunTerminalEvents orig, Terminal self, TerminalNode node)
    {
        self.groupCredits = 100000000;
        orig(self, node);
    }
}
