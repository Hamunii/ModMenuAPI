# ModMenuAPI

> [!NOTE]  
> This API is not quite yet released.

[![GitHub Build Status](https://img.shields.io/github/actions/workflow/status/Hamunii/ModMenuAPI/build.yml?style=for-the-badge&logo=github)](https://github.com/Hamunii/ModMenuAPI/blob/main/.github/workflows/build.yml)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/Hamunii/ModMenuAPI?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/lethal-company/p/Hamunii/ModMenuAPI/)
[![NuGet Version](https://img.shields.io/nuget/v/Hamunii.ModMenuAPI?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/Hamunii.ModMenuAPI)

An API to add your stuff as buttons on a menu that is accessible during gameplay.

### Support

> [!TIP]  
> Is your platform not supported? Open an issue, and let's discuss implementing it!
> ModMenuAPI is very lightweight, and it isn't heavily tied to any platform.

| Engine       | Supported? |
|--------------|----|
| Unity Mono   | ✅ |
| Unity IL2CPP | ❌ |
| *Other*      | ❌ |

| Modloader    | Supported? |
|--------------|----|
| BepInEx 5    | ✅ |
| *Other*      | ❌ |

#### Mods using this API

> [!NOTE]  
> First-party mods using this API are found in the [ModMenuAPI.Plugin](https://github.com/Hamunii/ModMenuAPI.Plugin) repository.

| Game | Mod | Readme |
|------|-----|--------|
| Lethal Company | [ModMenuAPI.Plugin.LC](https://github.com/Hamunii/ModMenuAPI.Plugin/releases) | [link](https://github.com/Hamunii/ModMenuAPI.Plugin/tree/main/ModMenuAPI.Plugin/LethalCompany/Thunderstore/LC_README.md) |
| Content Warning | [ModMenuAPI.Plugin.CW](https://github.com/Hamunii/ModMenuAPI.Plugin/releases) | [link](https://github.com/Hamunii/ModMenuAPI.Plugin/tree/main/ModMenuAPI.Plugin/ContentWarning/Thunderstore/CW_README.md) |

### Usage For Developers

ModMenuAPI is used by registering menu items with an instance of `ModMenu(string menuTitle)`:  
`public ModMenu RegisterItem(MMItemBase menuItem);` which this can be chained, or via a static method:  
`public static MMItemBase ModMenu.RegisterItem(MMItemBase menuItem, string menuTitle)`.

The fundamental building block for each mod menu item is `MMItemBase`, but we have specialized buttons for certain behaviors. The menu items are as follows:
- `MMButtonAction`
- `MMButtonToggle`
- `MMButtonToggleInstantiable`
- `MMButtonMenu`
- `MMButtonMenuInstantiable`

The `Instantiable` versions are like dummy buttons that don't do anything special, but we can still access their states (`MMButtonToggle`'s `Enabled` value) or other data, like the `MMButtonMenu`'s `MenuItems` field.

A menu button is just another submenu, but opened by clicking a button. These are rather powerful, since a lot of options can be put under one, and it opens up much more possibilities despite the UI options being otherwise limited, like the current lack of text or number input fields.

#### Getting Started

ModMenuAPI is available on NuGet, and you can add the following package to your `csproj` file:

> [!WARNING]  
> There are no release builds yet! Consider using this only after a release build exists.
```xml
<ItemGroup>
    <PackageReference Include="Hamunii.ModMenuAPI" Version="0.*-*" />
</ItemGroup>
```

#### Usage Example

```cs
using ModMenuAPI.ModMenuItems;

class CWPatches
{
    internal static void Init()
    {
        // This is the primary way of registering items, through an instance of `ModMenu`.
        // We register an item with instance of a class that inherits MMItemBase.
        new ModMenu("Player")
            .RegisterItem(new InfiniteJumpToggle())
            .RegisterItem(new SomeOtherToggle());

        // Or alternatively, we can use the static method for registering items,
        // which returns an instance of the `menuItem` instead of `ModMenu`
        var buttonInstance = ModMenu.RegisterItem(new SetMoneyAction(), "Stats");
    }
}

// We give the button's name and other optional metadata through the constructor
class InfiniteJumpToggle() : MMButtonToggle("Infinite Jump")
{
    protected override void OnEnable() => IL.PlayerController.TryJump += PlayerController_TryJump;
    protected override void OnDisable() => IL.PlayerController.TryJump -= PlayerController_TryJump;

    private static void PlayerController_TryJump(ILContext il)
    {
        // logic here
    }
}

// This is an example of an action button - it isn't a toggle
class SetMoneyAction() : MMButtonAction("Set Money")
{
    protected override void OnClick()
    {
        // logic here
    }
}
```

### TODO

**Priority:** First to last.
- [ ] Macro/preset system (ability to invoke/toggle buttons on certain events)
- [ ] Ability to toggle the UI on/off
- [ ] Permission system management (e.g. host-only buttons)
- [ ] Thunderstore release (must have permission system)
- [ ] More menu item types (e.g. number input)

### ModMenuAPI Architecture

ModMenuAPI is designed to be modular and lightweight. This is because the goal of this project is to be a generic mod menu API for any game.

| Namespace | Purpose |
|-|-|
| `ModMenuAPI` | The entry point of the software; handles loading and unloading itself so it plays well with hot loading. *This is the only modloader-specific part, and adding support for another modloader should be incredibly easy.* |
| `ModMenuAPI.ModMenuItems` & `ModMenuAPI.ModMenuItems.BaseItems` | Contains the mod menu button components that are used, including the API methods used for registering buttons on the menu. |
| `ModMenuAPI.MenuGUI` | Contains the graphical menu implementation, which currently is Unity's OnGUI. *A new implementation should be easy, as the most significant thing this does (other than the graphics) is invoke button presses using `MMItemBase`'s `CommonInvoke()`.* |