using UnityModMenuAPI.MenuGUI;
using UnityModMenuAPI.ModMenuItems.BaseItems;

namespace UnityModMenuAPI.ModMenuItems;

public static class ModMenu
{
    /// <summary>
    /// Register a menu item under a menu with the specified name.<br/>
    /// If a menu with the specified title doesn't exist yet, it will be created.
    /// </summary>
    /// <param name="menuItem">The menu item to register.</param>
    /// <param name="menuTitle">The name of the menu this item will be listed under.</param>
    public static void RegisterModMenuItem(ModMenuBaseItem menuItem, string menuTitle)
    {
        ModMenuMenuItem? menu = ModMenuGUI.ModMenus.Find(x => x.MenuTitle.ToLower().Equals(menuTitle.ToLower()));
        if(menu is not null)
            menu.MenuItems.Add(menuItem);
        else
        {
            var newMenu = new ModMenuMenuItem(menuTitle);
            newMenu.MenuItems.Add(menuItem);
            ModMenuGUI.ModMenus.Add(newMenu);
        }
    }
    /// <summary>
    /// Remove all menu items.
    /// </summary>
    public static void RemoveAll()
    {
        ModMenuGUI.ModMenus.Clear();
    }
}