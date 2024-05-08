using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityModMenuAPI.ModMenuItems;
using UnityModMenuAPI.ModMenuItems.BaseItems;

namespace UnityModMenuAPI.MenuGUI;

internal class ModMenuMenuItem
{
    internal ModMenuMenuItem(string menuTitle)
    {
        MenuTitle = menuTitle;
    }
    internal string MenuTitle = "Unnamed Menu";
    internal List<ModMenuBaseItem> MenuItems = new();
}