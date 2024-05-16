using System.Collections.Generic;
using ModMenuAPI.ModMenuItems.BaseItems;

namespace ModMenuAPI.MenuGUI;

internal class ModMenuMenuItem
{
    internal ModMenuMenuItem(string menuTitle)
    {
        MenuTitle = menuTitle;
    }
    internal readonly string MenuTitle = "Unnamed Menu";
    internal readonly List<MMItemBase> MenuItems = new();
}