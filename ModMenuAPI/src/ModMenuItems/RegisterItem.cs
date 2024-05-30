using System;
using System.Collections.Generic;
using System.Reflection;
using ModMenuAPI.MenuGUI;
using ModMenuAPI.ModMenuItems.BaseItems;

namespace ModMenuAPI.ModMenuItems;

/// <summary>
/// The main class handling everything important, except the GUI.
/// </summary>
public class ModMenu
{
    /// <summary>
    /// The list of menus the ModMenuAPI has.
    /// </summary>
    internal static List<ModMenuMenuItem> ModMenus { get; } = new();

    private readonly string m_menuTitle = null!;
    /// <summary>
    /// Create a menu panel instance to register menu items to, <br/>
    /// without having to specify the menu title for each item.
    /// </summary>
    /// <param name="menuTitle">The name of the menu items registered through this instance will go to.</param>
    public ModMenu(string menuTitle)
    {
        if(menuTitle is null)
            throw new ArgumentNullException(nameof(menuTitle), "Menu Title cannot be null!");

        m_menuTitle = menuTitle;
    }
    /// <summary>
    ///Register a menu item under the menu defined by the current instance of <c>ModMenu</c>.
    /// </summary>
    /// <param name="menuItem">The menu item to register.</param>
    /// <returns>Current instance of <c>ModMenu</c>.</returns>
    public ModMenu RegisterItem(MMItemBase menuItem)
    {
        RegisterItem(menuItem, m_menuTitle);
        return this;
    }

    /// <summary>
    /// Register a menu item under a menu with the specified name.<br/>
    /// If a menu with the specified title doesn't exist yet, it will be created.
    /// </summary>
    /// <param name="menuItem">The menu item to register.</param>
    /// <param name="menuTitle">The name of the menu this item will be listed under.</param>
    /// <returns>The <c>menuItem</c> instance.</returns>
    public static T RegisterItem<T>(T menuItem, string menuTitle) where T : MMItemBase
    {
        menuItem.ParentAssembly = Assembly.GetCallingAssembly();
        // MMLog.Log($"Adding item \"{menuItem.Metadata.Name}\" from assembly: " + menuItem.ParentAssembly.FullName);

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
        return menuItem;
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
    /// Intended to be used when live reloading assemblies with e.g. BepInEx ScriptEngine.
    /// </summary>
    public static void RemoveAllOwnedBy(Assembly ass)
    {
        var em = ModMenus.GetEnumerator();
        while (em.MoveNext())
            em.Current.MenuItems.RemoveAll(item => item.ParentAssembly.Equals(ass));
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