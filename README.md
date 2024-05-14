# ModMenuAPI

An API to add your stuff as buttons on a menu that is accessible during gameplay.

### Usage

```cs
using ModMenuAPI.ModMenuItems;

class CWPatches
{
    internal static void Init()
    {
        // menuTitle: The name of the menu this item will be listed under.
        ModMenu.RegisterModMenuItem(new InfiniteJumpPatch(), menuTitle: "Player");
        ModMenu.RegisterModMenuItem(new SetMoneyPatch(), menuTitle: "Stats");
    }
}

// This is an example of a toggle button
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

// This is an example of an action button - it isn't a toggle
class SetMoneyPatch : ModMenuButtonAction
{
    readonly ModMenuItemMetadata meta = new("Set Money");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 100000000;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
    }
}
```