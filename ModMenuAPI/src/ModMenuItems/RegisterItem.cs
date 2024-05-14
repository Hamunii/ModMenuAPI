using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ModMenuAPI.MenuGUI;
using ModMenuAPI.ModMenuItems.BaseItems;

namespace ModMenuAPI.ModMenuItems;

/// <summary>
/// The main class handling everything important, except the GUI.
/// </summary>
public static class ModMenu
{
    /// <summary>
    /// The list of menus the ModMenuAPI has.
    /// </summary>
    internal static List<ModMenuMenuItem> ModMenus { get; } = new();

    /// <summary>
    /// Register a menu item under a menu with the specified name.<br/>
    /// If a menu with the specified title doesn't exist yet, it will be created.
    /// </summary>
    /// <param name="menuItem">The menu item to register.</param>
    /// <param name="menuTitle">The name of the menu this item will be listed under.</param>
    public static void RegisterItem(ModMenuBaseItemBase menuItem, string menuTitle)
    {
        menuItem.ParentAssembly = Assembly.GetCallingAssembly();
        MMLog.Log($"V6 Adding item \"{menuItem.Metadata.Name}\" from assembly: " + menuItem.ParentAssembly.FullName);

        ModMenuMenuItem? menu = ModMenus.Find(x => x.MenuTitle.ToLower().Equals(menuTitle.ToLower()));
        if(menu is not null)
        {
            menu.MenuItems.Add(menuItem);
        }
        else
        {
            var newMenu = new ModMenuMenuItem(menuTitle);
            newMenu.MenuItems.Add(menuItem);
            ModMenus.Add(newMenu);
        }
    }

    /// <summary>
    /// Remove menu item with the specified name from the specified menu.<br/>
    /// Can be used to e.g. remove duplicate implementations from mods.
    /// </summary>
    /// <param name="itemName">The name of the item to remove.</param>
    /// <param name="menuTitle">The name of the menu the specified item belongs to.</param>
    /// <returns>Whether or not the item was found and removed or not.</returns>
    public static bool RemoveItem(string itemName, string menuTitle)
    {
        ModMenuMenuItem? menu = ModMenus.Find(x => x.MenuTitle.ToLower().Equals(menuTitle.ToLower()));
        if(menu is null)
            return false;

        menu.MenuItems.RemoveAll(item => item.Metadata.Name.Equals(itemName));
        return true;
    }

    /// <summary>
    /// Remove all menu items belonging to the specified assembly.<br/>
    /// Intended to be used when live reloading assemblies with e.g. BepInEx ScriptEngine.<br/>
    /// Because ScriptEngine adds a timestamp to the end of the assembly name, this recognizes the assembly through the name, ignoring numbers.
    /// </summary>
    public static void RemoveAllOwnedBy(Assembly ass)
    {
        MMLog.Log("Removing items from Assembly: " + ass.FullName);
        var assNameNoNum = Regex.Replace(ass.FullName, "[0-9]", "");

        var em = ModMenus.GetEnumerator();
        while (em.MoveNext())
            em.Current.MenuItems.RemoveAll(item => Regex.Replace(item.ParentAssembly.FullName, "[0-9]", "").Equals(assNameNoNum));
    }

    /// <summary>
    /// For internal debug purposes.
    /// </summary>
    internal static void RemoveAll()
    {
        MMLog.Log("Removing all items");

        var em = ModMenus.GetEnumerator();
        while (em.MoveNext())
            em.Current.MenuItems.Clear();
    }
}