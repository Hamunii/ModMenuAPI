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
    private float MENUWIDTH = 800;
    private float MENUHEIGHT = 400;
    private float MENUX;
    private float MENUY;
    private float ITEMWIDTH = 300;
    private float CENTERX;
    private float scrollStart;

    private GUIStyle menuStyle = null!;
    private GUIStyle enabledButtonStyle = null!;
    private GUIStyle hScrollStyle = null!;
    private GUIStyle vScrollStyle = null!;

    private Vector2 scrollPosition;
    internal static bool DevToolsMenuOpen = false;
    internal static bool canOpenDevToolsMenu = true;
    public static List<ModMenuBaseItem> menuMethods = new();
    internal static bool menuExists = false;
    const uint queueSize = 30;  // number of messages to keep
    // Queue<string> myLogQueue;
    private void Awake()
    {
        // myLogQueue = new();
        // InternalUnityLogger.OnUnityInternalLog += InternalUnityLogger_OnUnityInternalLog;

        isMenuOpen = false;
        MENUWIDTH = Screen.width / 6;
        MENUHEIGHT = Screen.width / 4;
        ITEMWIDTH = MENUWIDTH / 1.2f;

        // this is center at center of menu
        //MENUX = (Screen.width / 2) - (MENUWIDTH / 2);

        // this is center at left side of menu
        //MENUX = (Screen.width / 2);

        // this is right off the edge of the screen on the right side
        MENUX = Screen.width - MENUWIDTH;
        MENUY = (Screen.height / 2) - (MENUHEIGHT / 2);
        CENTERX = MENUX + ((MENUWIDTH / 2) - (ITEMWIDTH / 2));
        scrollStart = MENUY + 30;
    }

    // int printedThisFrame = 0;
    // long lastFrameCount = 0;
    // private void InternalUnityLogger_OnUnityInternalLog(object sender, UnityLogEventArgs e)
    // {
    //     if(e.Message.StartsWith("[Error  :")
    //     || e.Message.StartsWith("[Message:")
    //     || e.Message.StartsWith("[Info   :")
    //     || e.Message.StartsWith("[Warning:")
    //     || e.Message.StartsWith("[Debug  :"))
    //         myLogQueue.Enqueue(e.Message);
    //     else{
    //         string level = e.LogLevel.ToString();
    //         if(level == "Debug") return;
    //         if(level == "Log") level = "Info";
    //         level = level.PadRight(6);
    //         myLogQueue.Enqueue($"[{level}: Unity Log] {e.Message}");
    //     }
    //     if (e.LogLevel == InternalLogLevel.Exception){
    //         StackTrace stackTrace = new StackTrace(); 
    //         myLogQueue.Enqueue(stackTrace.GetFrames().ToString());
    //     }
    //     while (myLogQueue.Count > queueSize)
    //         myLogQueue.Dequeue();

    //     if(Time.frameCount == lastFrameCount){
    //         printedThisFrame++;
    //         if(printedThisFrame >= 30){
    //             StartCoroutine(DisableForAFewFrames());
    //             return;
    //         }
    //     }
    //     else printedThisFrame = 0;

    //     lastFrameCount = Time.frameCount;
    // }

    // private IEnumerator DisableForAFewFrames(){
    //     InternalUnityLogger.OnUnityInternalLog -= InternalUnityLogger_OnUnityInternalLog;
    //     yield return new WaitUntil(() => Time.frameCount > lastFrameCount + 1);
    //     InternalUnityLogger.OnUnityInternalLog += InternalUnityLogger_OnUnityInternalLog;
    // }

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
            enabledButtonStyle = new GUIStyle(UnityEngine.GUI.skin.button);
            hScrollStyle = new GUIStyle(UnityEngine.GUI.skin.horizontalScrollbar);
            vScrollStyle = new GUIStyle(UnityEngine.GUI.skin.verticalScrollbar);

            menuStyle.normal.textColor = Color.white;
            menuStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.1f, .9f));
            menuStyle.fontSize = 18;
            menuStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            enabledButtonStyle.normal.textColor = Color.white;
            enabledButtonStyle.normal.background = MakeTex(2, 2, new Color(0.0f, 0.01f, 0.2f, .9f));
            enabledButtonStyle.hover.background = MakeTex(2, 2, new Color(0.4f, 0.01f, 0.1f, .9f));
            enabledButtonStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            hScrollStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.1f, 0f));

            GUI.skin.textArea.alignment = TextAnchor.LowerLeft;
            GUI.skin.textArea.fontSize = 16;
        }
    }

    private void OnGUI()
    {
        // Plugin.Logger.LogInfo("\n" + string.Join("\n Mirror > ", myLogQueue.ToArray()));
        if(menuStyle == null) { InitializeMenu(); }

        // GUI.Label(
        //     new Rect(
        //         5,                      // x, left offset
        //         Screen.height / 2,      // y, top offset
        //         Screen.width / 3,       // width
        //         540                     // height
        //     ),
        //     "\n" + string.Join("", myLogQueue.ToArray()),
        //     GUI.skin.textArea
        // );

        if(!canOpenDevToolsMenu) { return; }
        if(!DevToolsMenuOpen) return;
        GUI.Box(new Rect(MENUX, MENUY, MENUWIDTH, MENUHEIGHT), "UnityModMenuAPI", menuStyle);
        scrollPosition = GUI.BeginScrollView(new Rect(MENUX, MENUY + 30, MENUWIDTH, MENUHEIGHT - 50), scrollPosition, new Rect(MENUX, scrollStart, ITEMWIDTH, menuMethods.Count * 30), false, true, hScrollStyle, vScrollStyle);

        int idx = 0;
        foreach (var menuItem in menuMethods)
        {   
            GUIStyle? currentButtonStyle = null;
            if (menuItem.ItemType == ModMenuItemType.ToggleButton)
                currentButtonStyle = ((ModMenuButtonToggle)menuItem).Enabled ? enabledButtonStyle : GUI.skin.button;
            else
                currentButtonStyle = GUI.skin.button;

            if (GUI.Button(new Rect(CENTERX, MENUY + 30 + (idx * 30), ITEMWIDTH, 30), $"{menuItem.Config.Name}", currentButtonStyle))
            {
                menuItem.CommonInvoke();
            }
            idx++;
        }

        // End the scroll view that we began above.
        GUI.EndScrollView();
    }
}