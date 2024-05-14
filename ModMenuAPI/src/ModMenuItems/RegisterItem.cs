using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ModMenuAPI.MenuGUI;
using ModMenuAPI.ModMenuItems.BaseItems;

namespace ModMenuAPI.ModMenuItems;

public static class ModMenu
{
    /// <summary>
    /// Register a menu item under a menu with the specified name.<br/>
    /// If a menu with the specified title doesn't exist yet, it will be created.
    /// </summary>
    /// <param name="menuItem">The menu item to register.</param>
    /// <param name="menuTitle">The name of the menu this item will be listed under.</param>
    public static void RegisterItem(ModMenuBaseItemBase menuItem, string menuTitle)
    {
        menuItem.ParentAssembly = Assembly.GetCallingAssembly();
        Plugin.Logger.LogInfo($"V6 Adding item \"{menuItem.Metadata.Name}\" from assembly: " + menuItem.ParentAssembly.FullName);

        ModMenuMenuItem? menu = ModMenuGUI.ModMenus.Find(x => x.MenuTitle.ToLower().Equals(menuTitle.ToLower()));
        if(menu is not null)
        {
            menu.MenuItems.Add(menuItem);
        }
        else
        {
            var newMenu = new ModMenuMenuItem(menuTitle);
            newMenu.MenuItems.Add(menuItem);
            ModMenuGUI.ModMenus.Add(newMenu);
        }
    }

    /// <summary>
    /// Remove menu item with the specified name from the specified menu.<br/>
    /// Can be used to e.g. remove duplicate implementations from mods, especially if the other implementation has issues.
    /// </summary>
    /// <param name="menuItemName">The name of the item to remove.</param>
    /// <param name="menuTitle">The name of the menu the specified item belongs to.</param>
    /// <returns>Whether or not the item was found and removed or not.</returns>
    public static bool RemoveItem(string menuItemName, string menuTitle)
    {
        ModMenuMenuItem? menu = ModMenuGUI.ModMenus.Find(x => x.MenuTitle.ToLower().Equals(menuTitle.ToLower()));
        if(menu is null)
            return false;

        menu.MenuItems.RemoveAll(item => item.Metadata.Name.Equals(menuItemName));
        return true;
    }

    /// <summary>
    /// Remove all menu items belonging to the specified assembly.<br/>
    /// Intended to be used when live reloading assemblies with e.g. BepInEx ScriptEngine.
    /// </summary>
    public static void RemoveAllOwnedBy(Assembly ass)
    {
        Plugin.Logger.LogInfo("Removing items from Assembly: " + ass.FullName);
        var assNameNoNum = Regex.Replace(ass.FullName, "[0-9]", "");

        var em = ModMenuGUI.ModMenus.GetEnumerator();
        while (em.MoveNext())
            em.Current.MenuItems.RemoveAll(item => item.ParentAssembly.FullName.Equals(assNameNoNum));
    }

    internal static void RemoveAll()
    {
        Plugin.Logger.LogInfo("Removing all items");

        var em = ModMenuGUI.ModMenus.GetEnumerator();
        while (em.MoveNext())
            em.Current.MenuItems.Clear();
    }
}