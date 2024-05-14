# ModMenuAPI

> [!NOTE]  
> This API is not quite yet released.

[![GitHub Build Status](https://img.shields.io/github/actions/workflow/status/Hamunii/ModMenuAPI/build.yml?style=for-the-badge&logo=github)](https://github.com/Hamunii/ModMenuAPI/blob/main/.github/workflows/build.yml)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/Hamunii/ModMenuAPI?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/lethal-company/p/Hamunii/ModMenuAPI/)
[![NuGet Version](https://img.shields.io/nuget/v/Hamunii.ModMenuAPI?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/Hamunii.ModMenuAPI)

An API to add your stuff as buttons on a menu that is accessible during gameplay.

### Support

| Engine       | Supported? |
|--------------|----|
| Unity Mono   | ✅ |
| Unity IL2CPP | ❌ |
| *Other*      | ❌ |

| Modloader    | Supported? |
|--------------|----|
| BepInEx 5    | ✅ |
| *Other*      | ❌ |

#### Games with mods using this API

| Game | Mods |
|------|------|
| Lethal Company | [ModMenuAPI.CoreMod.LC](/ModMenuAPI.CoreMod/LethalCompany/) |
| Content Warning | [ModMenuAPI.CoreMod.CW](/ModMenuAPI.CoreMod/ContentWarning/) |

### Usage

```cs
using ModMenuAPI.ModMenuItems;

class CWPatches
{
    internal static void Init()
    {
        // menuTitle: The name of the menu this item will be listed under.
        ModMenu.RegisterItem(new InfiniteJumpPatch(), menuTitle: "Player");
        ModMenu.RegisterItem(new SetMoneyPatch(), menuTitle: "Stats");
    }
}

// This is an example of a toggle button
class InfiniteJumpPatch : ModMenuButtonToggle
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

### ModMenuAPI Architecture

ModMenuAPI is designed to be modular and lightweight. This is because the goal of this project is to be a generic mod menu API for any game.

| Namespace | Purpose |
|-|-|
| `ModMenuAPI` | The entry point of the software; handles loading and unloading itself so it plays well with hot loading. *This is the only modloader-specific part, and adding support for another modloader should be incredibly easy.* |
| `ModMenuAPI.ModMenuItems` & `ModMenuAPI.ModMenuItems.BaseItems` | Contains the mod menu button components that are used, including the API methods used for registering buttons on the menu. |
| `ModMenuAPI.MenuGUI` | Contains the graphical menu implementation, which currently is Unity's OnGUI. *A new implementation should be easy, as the most significant thing this does (other than the graphics) is invoke button presses using `ModMenuBaseItemBase`'s `CommonInvoke()`.* |