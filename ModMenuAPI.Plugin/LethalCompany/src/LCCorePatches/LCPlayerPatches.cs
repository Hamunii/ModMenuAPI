using System;
using System.Collections;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using UnityEngine;
using ModMenuAPI.ModMenuItems;

namespace PluginLC.CorePatches;

class LCPlayerPatches
{
    const string menuPlayer = "Player";
    const string menuMisc = "Misc";
    internal static void Init()
    {
        ModMenu.RegisterItem(new InfiniteSprintPatch(), menuPlayer);
        ModMenu.RegisterItem(new MovementCheatPatch(), menuPlayer);
        ModMenu.RegisterItem(new OnDeathHealPatch(), menuPlayer);
        ModMenu.RegisterItem(new InfiniteShotgunAmmoPatch(), menuPlayer);
    }
}
class InfiniteSprintPatch : ModMenuButtonToggleBase
{
    readonly ModMenuItemMetadata meta = new("Infinite Sprint"){ InvokeOnInit = true };
    public override ModMenuItemMetadata Metadata => meta;

    protected override void OnEnable() => On.GameNetcodeStuff.PlayerControllerB.Update += InfiniteSprint_PlayerControllerB_Update;
    protected override void OnDisable() => On.GameNetcodeStuff.PlayerControllerB.Update -= InfiniteSprint_PlayerControllerB_Update;

    private static void InfiniteSprint_PlayerControllerB_Update(On.GameNetcodeStuff.PlayerControllerB.orig_Update orig, GameNetcodeStuff.PlayerControllerB self)
    {
        orig(self);
        self.sprintMeter = 1;
    }
}

internal class MovementCheatPatch : ModMenuButtonToggleBase
{
    readonly ModMenuItemMetadata meta = new("Movement Cheat"){ InvokeOnInit = true };
    public override ModMenuItemMetadata Metadata => meta;

    protected override void OnEnable(){
        On.GameNetcodeStuff.PlayerControllerB.Jump_performed += MovementCheat_PlayerControllerB_Jump_performed;
        On.GameNetcodeStuff.PlayerControllerB.Update += MovementCheat_PlayerControllerB_Update;
    }
    protected override void OnDisable(){
        On.GameNetcodeStuff.PlayerControllerB.Jump_performed -= MovementCheat_PlayerControllerB_Jump_performed;
        On.GameNetcodeStuff.PlayerControllerB.Update -= MovementCheat_PlayerControllerB_Update;
    }

    private static void MovementCheat_PlayerControllerB_Update(On.GameNetcodeStuff.PlayerControllerB.orig_Update orig, GameNetcodeStuff.PlayerControllerB self) {
        if (self.isSpeedCheating){
            self.walkForce *= 0.8f;
            self.walkForce = Vector3.ClampMagnitude(self.walkForce, 0.1f);
            if(self.isSprinting){
                self.fallValue -= 130 * Time.deltaTime;
                self.thisController.Move(self.walkForce * 700 * Time.deltaTime);
            }
            else{
                self.thisController.Move(self.walkForce * 200 * Time.deltaTime);
            }
        }
        orig(self);
    }
    private static void MovementCheat_PlayerControllerB_Jump_performed(On.GameNetcodeStuff.PlayerControllerB.orig_Jump_performed orig, GameNetcodeStuff.PlayerControllerB self, UnityEngine.InputSystem.InputAction.CallbackContext context) {
        self.playerSlidingTimer = 0f;
        self.isJumping = true;
        self.sprintMeter = Mathf.Clamp(self.sprintMeter - 0.08f, 0f, 1f);
        self.movementAudio.PlayOneShot(StartOfRound.Instance.playerJumpSFX);
        if(self.jumpCoroutine != null){
            self.StopCoroutine(self.jumpCoroutine);
            // Cheat stuff
            self.isSpeedCheating = true;
            if(self.isSprinting){
                self.jumpForce = 50f;
            }
            else{
                self.jumpForce = 13f;
            }
            self.jumpCoroutine = self.StartCoroutine(CustomPlayerJump(self));
            return;
        }
        self.jumpCoroutine = self.StartCoroutine(self.PlayerJump());
    }
    // It turns out, using a transpiler on an IEnumerator is not as easy.
    private static IEnumerator CustomPlayerJump(GameNetcodeStuff.PlayerControllerB self) {
        self.playerBodyAnimator.SetBool("Jumping", value: true);
        self.fallValue = self.jumpForce;
        self.fallValueUncapped = self.jumpForce;
        yield return new WaitForSeconds(0.1f);
        self.isJumping = false;
        self.isFallingFromJump = true;
        yield return new WaitUntil(() => self.thisController.isGrounded);
        self.playerBodyAnimator.SetBool("Jumping", value: false);
        self.isFallingFromJump = false;
        self.PlayerHitGroundEffects();
        self.jumpCoroutine = null;
        // Cheat stuff
        self.isSpeedCheating = false;
        self.jumpForce = 13f;
    }
}

internal class OnDeathHealPatch : ModMenuButtonToggleBase
{
    readonly ModMenuItemMetadata meta = new("On Death: Heal"){ InvokeOnInit = true };
    public override ModMenuItemMetadata Metadata => meta;

    protected override void OnEnable() => On.GameNetcodeStuff.PlayerControllerB.KillPlayer += OnDeathHeal_PlayerControllerB_KillPlayer;
    protected override void OnDisable() => On.GameNetcodeStuff.PlayerControllerB.KillPlayer -= OnDeathHeal_PlayerControllerB_KillPlayer;

    private static void OnDeathHeal_PlayerControllerB_KillPlayer(On.GameNetcodeStuff.PlayerControllerB.orig_KillPlayer orig, GameNetcodeStuff.PlayerControllerB self, Vector3 bodyVelocity, bool spawnBody, CauseOfDeath causeOfDeath, int deathAnimation)
    {   
        self.health = 100;
        self.MakeCriticallyInjured(enable: false);
        HUDManager.Instance.UpdateHealthUI(self.health, hurtPlayer: false);
        // Lazy hacky method to get rid of the broken glass effect.
        self.DamagePlayer(damageNumber: 0, hasDamageSFX: false);
    }
}

internal class InfiniteShotgunAmmoPatch : ModMenuButtonToggleBase
{
    readonly ModMenuItemMetadata meta = new("Infinite Shotgun Ammo"){ InvokeOnInit = true };
    public override ModMenuItemMetadata Metadata => meta;
    protected override void OnEnable() => IL.ShotgunItem.ItemActivate += InfiniteShotgunAmmo_ShotgunItem_ItemActivate;
    protected override void OnDisable() => IL.ShotgunItem.ItemActivate -= InfiniteShotgunAmmo_ShotgunItem_ItemActivate;

    private static void InfiniteShotgunAmmo_ShotgunItem_ItemActivate(ILContext il)
    {
        /*
        // Find this:

        if (this.shellsLoaded == 0)
        {
            this.StartReloadGun();
            return;
        }

        // And replace: brtrue.s (branch if shellsLoaded is not zero)
        //        with: br       (branch unconditionally)
        //
        // But because brtrue.s pops a value from the stack
        // and br doesn't, we add a Pop before it.
        */
        ILCursor c = new(il);
        c.GotoNext(
            x => x.MatchLdfld<ShotgunItem>("shellsLoaded")
        );

        // ldfld int32 ShotgunItem::shellsLoaded => pop
        c.Remove();
        c.Emit(OpCodes.Pop);

        // brtrue.s IL_0020                      => br IL_0020
        c.Next.OpCode = OpCodes.Br;
    }
}

