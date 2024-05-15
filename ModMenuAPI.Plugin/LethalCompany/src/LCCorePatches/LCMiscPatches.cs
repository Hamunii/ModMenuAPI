using System;
using System.Collections;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using UnityEngine;
using ModMenuAPI.ModMenuItems;
using System.Collections.Generic;
using System.Linq;

namespace PluginLC.CorePatches;

class LCMiscPatches
{
    const string menuMisc = "Misc";
    internal static ModMenuButtonContextMenuInstantiable weatherOverridesMenu = new("Weather Override >");
    internal static void Init()
    {
        ModMenu.RegisterItem(new IsEditorPatch(), menuMisc);
        ModMenu.RegisterItem(new InfiniteCreditsPatch(), menuMisc);
        ModMenu.RegisterItem(new PullLeverAction(), menuMisc);
        ModMenu.RegisterItem(new MeetQuotaPatch(), menuMisc);
        ModMenu.RegisterItem(weatherOverridesMenu, menuMisc);

        if(StartOfRound.Instance is not null)
        {
            PopulateWeatherOverrides();
        }
        On.StartOfRound.Start += StartOfRound_Start;
    }

    private static void StartOfRound_Start(On.StartOfRound.orig_Start orig, StartOfRound self)
    {
        orig(self);
        PopulateWeatherOverrides();
    }

    internal static void SetWeatherOverride()
    {
        if(WeatherOverride.currentOverride is null || StartOfRound.Instance is null)
            return;

        List<SelectableLevel> levels = StartOfRound.Instance.levels.ToList();
        for(var i = 0; i < levels.Count; i++ )
        {
            var level = levels[i];
            level.currentWeather = WeatherOverride.currentOverride.WeatherType;
        }
    }

    private static void PopulateWeatherOverrides()
    {
        List<LevelWeatherType> weathers = new(); 
        foreach(var level in StartOfRound.Instance.levels)
        {
            foreach(var weather in level.randomWeathers)
                weathers.Add(weather.weatherType);
        }
        weatherOverridesMenu.MenuItems.Clear();
        foreach(var weather in weathers.Distinct().ToList())
        {
            weatherOverridesMenu.MenuItems.Add(new WeatherOverride(weather.ToString(), weather));
        }
    }
}

class IsEditorPatch : ModMenuButtonToggleBase
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


internal class InfiniteCreditsPatch : ModMenuButtonToggleBase
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

internal class PullLeverAction : ModMenuButtonActionBase
{
    readonly ModMenuItemMetadata meta = new("Pull Lever");
    public override ModMenuItemMetadata Metadata => meta;

    internal PullLeverAction()
    {
        On.StartMatchLever.Start += StartMatchLever_Start;
    }
    private static void StartMatchLever_Start(On.StartMatchLever.orig_Start orig, StartMatchLever self)
    {
        Plugin.Logger.LogInfo("Got Lever instance!");
        lever = self;
        orig(self);
    }

    static StartMatchLever? lever = null;
    public override void OnClick()
    {
        if(lever is null)
        {
            Plugin.Logger.LogInfo("Lever was null!");
            On.StartMatchLever.Update += StartMatchLever_Update;
        }
        else
        {
            lever.PullLeverAnim(StartOfRound.Instance.inShipPhase);
            lever.PullLever();
        }
    }

    private void StartMatchLever_Update(On.StartMatchLever.orig_Update orig, StartMatchLever self)
    {
        lever = self;
        orig(self);
        On.StartMatchLever.Update -= StartMatchLever_Update;
        CommonInvoke();
    }
}

internal class MeetQuotaPatch : ModMenuButtonToggleBase
{
    readonly ModMenuItemMetadata meta = new("Force Meet Quota");
    public override ModMenuItemMetadata Metadata => meta;

    protected override void OnEnable() => On.StartOfRound.EndOfGame += StartOfRound_EndOfGame;
    protected override void OnDisable() => On.StartOfRound.EndOfGame -= StartOfRound_EndOfGame;

    private static IEnumerator StartOfRound_EndOfGame(On.StartOfRound.orig_EndOfGame orig, StartOfRound self, int bodiesInsured, int connectedPlayersOnServer, int scrapCollected)
    {
        TimeOfDay.Instance.quotaFulfilled = TimeOfDay.Instance.profitQuota;

        var origIE = orig(self,bodiesInsured,connectedPlayersOnServer,scrapCollected);
        while(origIE.MoveNext())
            yield return origIE.Current;
    }
}

class WeatherOverride : ModMenuButtonToggleBase
{
    internal static WeatherOverride? currentOverride;
    internal readonly LevelWeatherType WeatherType;
    internal WeatherOverride(string weatherName, LevelWeatherType weatherType)
    {
        meta = new(weatherName);
        WeatherType = weatherType;
    }
    readonly ModMenuItemMetadata meta = new("Override <weather>");
    public override ModMenuItemMetadata Metadata => meta;

    protected override void OnEnable()
    {
        currentOverride?.CommonInvoke();
        currentOverride = this;
        LCMiscPatches.SetWeatherOverride();
    }
    protected override void OnDisable()
    {
        if(currentOverride == this)
            currentOverride = null;
    }
}