using ModMenuAPI.ModMenuItems;

namespace ModMenuAPI.Meta;

static class ConfigMenu
{
    private static readonly MMButtonToggleInstantiable showMenusToggle =
        new(new MMItemMetadata("Show Menus") { InvokeOnInit = true });
    internal static bool ShowMenus
    {
        get => showMenusToggle.Enabled;
    }

    internal static void Init()
    {
        new ModMenu("ModMenuAPI").RegisterItem(showMenusToggle);
    }
}
