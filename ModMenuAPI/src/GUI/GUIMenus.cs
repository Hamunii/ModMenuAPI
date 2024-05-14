using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using ModMenuAPI.ModMenuItems;
using ModMenuAPI.ModMenuItems.BaseItems;

namespace ModMenuAPI.MenuGUI;

internal class ModMenuMenuItem
{
    internal ModMenuMenuItem(string menuTitle)
    {
        MenuTitle = menuTitle;
    }
    internal string MenuTitle = "Unnamed Menu";
    internal List<ModMenuBaseItemBase> MenuItems = new();
}