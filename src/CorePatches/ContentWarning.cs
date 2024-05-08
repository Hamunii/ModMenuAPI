using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityModMenuAPI.ModMenuItems;

namespace UnityModMenuAPI.CorePatches.CW;

class CWInit
{
    internal static void Init()
    {
        ModMenu.RegisterModMenuItem(new InfiniteJumpPatch());
    }
}

class InfiniteJumpPatch : ModMenuButtonToggle
{
    ModMenuItemConfig myConf = new()
    {
        Name = "Infinite Jump",
        ToolTip = "Removes check for touching ground when jumping."
    };
    public override ModMenuItemConfig Config => myConf;
    
    protected override void OnEnable() { IL.PlayerController.TryJump += PlayerController_TryJump; }
    protected override void OnDisable() { IL.PlayerController.TryJump -= PlayerController_TryJump; }

    private static void PlayerController_TryJump(ILContext il)
    {
        ILCursor c = new(il);
        // we remove if statement branches and pop the values that would have been popped
        while (c.TryGotoNext(x => x.MatchBgeUn(out _) || x.MatchBleUn(out _)))
        {
            c.Remove();
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Pop);
        }
    }
}