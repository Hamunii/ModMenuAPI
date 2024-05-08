using UnityModMenuAPI.MenuGUI;
using UnityModMenuAPI.ModMenuItems.BaseItems;

namespace UnityModMenuAPI;

public static class ModMenu
{
    public static void RegisterModMenuItem(ModMenuBaseItem menuItem)
    {
        ModMenuGUI.menuMethods.Add(menuItem);
    }
}