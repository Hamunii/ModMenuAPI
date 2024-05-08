using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityModMenuAPI.ModMenuItems;

namespace UnityModMenuAPI.CorePatches.CW;

class CWPlayerPatches
{
    const string menuTitle = "Player";
    internal static void Init()
    {
        ModMenu.RegisterModMenuItem(new InfiniteJumpPatch(), menuTitle);
        ModMenu.RegisterModMenuItem(new FastMovementPatch(), menuTitle);
    }
}

class InfiniteJumpPatch : ModMenuButtonToggle
{
    readonly ModMenuItemMetadata meta = new("Infinite Jump", "Removes check for touching ground when jumping.");
    public override ModMenuItemMetadata Metadata => meta;
    protected override void OnEnable() { IL.PlayerController.TryJump += PlayerController_TryJump; }
    protected override void OnDisable() { IL.PlayerController.TryJump -= PlayerController_TryJump; }
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

class FastMovementPatch : ModMenuButtonToggle
{
    readonly ModMenuItemMetadata meta = new("Fast Movement");
    public override ModMenuItemMetadata Metadata => meta;
    PlayerController? self = null;
    protected override void OnEnable() {
        if (Player.localPlayer is null)
        {
            On.PlayerController.Start += PlayerController_Start;
            return;
        }

        self = Player.localPlayer.refs.controller;

        origMovForce = self.movementForce;
        origStaminaReg = self.staminaRegRate;
        origJumpForceDur = self.jumpForceDuration;
        origJumpForceOverTime = self.jumpForceOverTime;

        self.movementForce = 150;
        self.staminaRegRate = 10000;
        self.jumpForceDuration = 0.1f;
        self.jumpForceOverTime = 2f;
    }
    protected override void OnDisable()
    {
        if(self is null)
            return;

        self.movementForce = origMovForce;
        self.staminaRegRate = origStaminaReg;
        self.jumpForceDuration = origJumpForceDur;
        self.jumpForceOverTime = origJumpForceOverTime;
    }
    static float origMovForce;
    static float origStaminaReg;
    static float origJumpForceDur;
    static float origJumpForceOverTime;
    private void PlayerController_Start(On.PlayerController.orig_Start orig, PlayerController self)
    {
        orig(self);
        if(self == Player.localPlayer.refs.controller)
        {
            OnEnable();
            On.PlayerController.Start -= PlayerController_Start;
        }
    }
}