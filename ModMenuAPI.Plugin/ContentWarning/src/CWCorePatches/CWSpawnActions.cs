using System;
using System.Collections;
using HarmonyLib;
using MonoMod.Cil;
using ModMenuAPI.ModMenuItems;

namespace PluginCW.CorePatches;

class CWSpawnActions
{
    static RoundSpawner roundSpawner = null!;
    const string menuTitle = "Monsters";
    internal static void Init()
    {
        On.RoundSpawner.ctor += RoundSpawner_Ctor;
        On.RoundSpawner.Start += RoundSpawner_Start;
    }

    private static void RoundSpawner_Ctor(On.RoundSpawner.orig_ctor orig, RoundSpawner self)
    {
        Plugin.Logger.LogInfo("Got RoundSpawner instance!");
        roundSpawner = self;
        foreach(var item in roundSpawner.possibleSpawns)
        {
            Plugin.Logger.LogInfo($"(ctor) item: {item.gameObject.name}");
        }
        orig(self);
    }

    private static IEnumerator RoundSpawner_Start(On.RoundSpawner.orig_Start orig, RoundSpawner self)
    {
        foreach(var item in roundSpawner.possibleSpawns)
        {
            Plugin.Logger.LogInfo($"(Start) item: {item.gameObject.name}");
        }

        var origIEnum = orig(self);
        while(origIEnum.MoveNext())
            yield return origIEnum.Current;
    }
}
