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

internal class ModMenuGUI : MonoBehaviour
{
    public bool isMenuOpen;
    private float MenuWidth = 500;
    private float MenuHeight = 100;
    private float MenuX;
    private float MenuY;
    private float ItemWidth = 300;
    private float CenterX;
    private float scrollStart;

    private GUIStyle menuStyle = null!;
    private GUIStyle toggleEnabledButtonStyle = null!;
    private GUIStyle toggleDisabledButtonStyle = null!;
    private GUIStyle actionButtonStyle = null!;
    private GUIStyle hScrollStyle = null!;
    private GUIStyle vScrollStyle = null!;

    // private Vector2 scrollPosition;
    internal static bool DevToolsMenuOpen = false;
    internal static bool canOpenDevToolsMenu = true;
    public static List<ModMenuBaseItem> menuMethods = new();
    public static List<ModMenuMenuItem> ModMenus = new();
    internal static bool menuExists = false;    
    private void Awake()
    {
        isMenuOpen = false;
        MenuWidth = 250;
        MenuHeight = Screen.width / 4;
        ItemWidth = MenuWidth / 1.1f;

        // this is center at center of menu
        //MENUX = (Screen.width / 2) - (MENUWIDTH / 2);

        // this is center at left side of menu
        //MENUX = (Screen.width / 2);

        // this is right off the edge of the screen on the right side
        MenuX = 20;
        MenuY = 20;
        CenterX = MenuX + ((MenuWidth / 2) - (ItemWidth / 2));
        scrollStart = MenuY + 30;
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private void InitializeMenu()
    {
        if (menuStyle == null)
        {
            menuStyle = new GUIStyle(UnityEngine.GUI.skin.box);
            toggleEnabledButtonStyle = new GUIStyle(UnityEngine.GUI.skin.button);
            toggleDisabledButtonStyle = new GUIStyle(UnityEngine.GUI.skin.button);
            actionButtonStyle = new GUIStyle(UnityEngine.GUI.skin.button);
            hScrollStyle = new GUIStyle(UnityEngine.GUI.skin.horizontalScrollbar);
            vScrollStyle = new GUIStyle(UnityEngine.GUI.skin.verticalScrollbar);

            menuStyle.normal.textColor = Color.white;
            menuStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.01f, .8f));
            menuStyle.fontSize = 18;
            menuStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            toggleEnabledButtonStyle.normal.textColor = Color.white;
            toggleEnabledButtonStyle.normal.background = MakeTex(2, 2, new Color(0.5f, 0.5f, 0.8f, .8f));
            toggleEnabledButtonStyle.hover.background = MakeTex(2, 2, new Color(0.8f, 0.05f, 0.5f, .8f));
            toggleEnabledButtonStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            toggleDisabledButtonStyle.normal.textColor = Color.white;
            toggleDisabledButtonStyle.normal.background = MakeTex(2, 2, new Color(0.3f, 0.3f, 0.3f, .8f));
            toggleDisabledButtonStyle.hover.background = MakeTex(2, 2, new Color(0.4f, 0.4f, 0.6f, .8f));
            toggleDisabledButtonStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            actionButtonStyle.normal.textColor = Color.white;
            actionButtonStyle.normal.background = MakeTex(2, 2, new Color(0.4f, 0.45f, 0.35f, .8f));
            actionButtonStyle.hover.background = MakeTex(2, 2, new Color(0.6f, 0.7f, 0.5f, .8f));
            actionButtonStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            hScrollStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.1f, 0f));

            GUI.skin.textArea.alignment = TextAnchor.LowerLeft;
            GUI.skin.textArea.fontSize = 16;
        }
    }

    private void OnGUI()
    {
        if(menuStyle == null) { InitializeMenu(); }

        if(!canOpenDevToolsMenu) { return; }
        if(!DevToolsMenuOpen) return;
        int menuIdx = 0;
        foreach (var menu in ModMenus)
        {
            GUI.Box(new Rect(MenuX + menuIdx * MenuWidth * 1.05f, MenuY, MenuWidth, menu.MenuItems.Count * 30 + 42), menu.MenuTitle, menuStyle);
            // scrollPosition = GUI.BeginScrollView(new Rect(MENUX, MENUY + 30, MENUWIDTH, MENUHEIGHT - 50), scrollPosition, new Rect(MENUX, scrollStart, ITEMWIDTH, menuMethods.Count * 30), false, true, hScrollStyle, vScrollStyle);

            int ItemIdx = 0;
            foreach (var menuItem in menu.MenuItems)
            {   
                GUIStyle? currentButtonStyle = null;
                if (menuItem.ItemType == ModMenuItemType.ToggleButton)
                    currentButtonStyle = ((ModMenuButtonToggle)menuItem).Enabled ? toggleEnabledButtonStyle : toggleDisabledButtonStyle;
                else
                    currentButtonStyle = actionButtonStyle;

                if (GUI.Button(new Rect(CenterX + menuIdx * MenuWidth * 1.05f, MenuY + 30 + (ItemIdx * 30), ItemWidth, 30), $"{menuItem.Metadata.Name}", currentButtonStyle))
                {
                    menuItem.CommonInvoke();
                }
                ItemIdx++;
            }
            menuIdx++;
        }
    }
}