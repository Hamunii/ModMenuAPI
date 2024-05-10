#if DEBUG && true
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityModMenuAPI;
using UnityModMenuAPI.ModMenuItems;

namespace CoreModCW.CorePatches;

class DebugCW
{
    static RoundSpawner roundSpawner = null!;
    const string menuTitle = "Monsters";
    internal static void Init()
    {
        ModMenu.RegisterItem(new InfiniteJumpPatch(), "Player");
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

class InfiniteJumpPatch : ModMenuButtonToggleBase
{
    readonly ModMenuItemMetadata meta = new("Infinite Jump", "Removes check for touching ground when jumping.");
    public override ModMenuItemMetadata Metadata => meta;
    protected override void OnEnable() => IL.PlayerController.TryJump += PlayerController_TryJump;
    protected override void OnDisable() => IL.PlayerController.TryJump -= PlayerController_TryJump;
    private static void PlayerController_TryJump(ILContext il)
    {
        ILCursor c = new(il);
        // we remove `if` branches and pop the values that would have been popped
        while (c.TryGotoNext(x => x.MatchBgeUn(out _) || x.MatchBleUn(out _)))
        {
            c.Remove();
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Pop);
        }
    }
}
#endif