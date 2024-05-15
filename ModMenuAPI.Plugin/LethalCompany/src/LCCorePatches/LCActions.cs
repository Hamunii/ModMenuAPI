using UnityEngine;
using ModMenuAPI.ModMenuItems;

namespace PluginLC.CorePatches;

class LCActionPatches
{
    internal static QuickMenuManager QMM_Instance = null!;
    const string menuTitle = "Action";

    internal static void Init()
    {
        On.QuickMenuManager.Start += QuickMenuManager_Start;
        ModMenu.RegisterItem(new TeleportSelfToEntranceAction(), menuTitle);
        ModMenu.RegisterItem(new ToggleTestRoomAction(), menuTitle);
    }

    private static void QuickMenuManager_Start(On.QuickMenuManager.orig_Start orig, QuickMenuManager self)
    {
        QMM_Instance = self;
        orig(self);
    }
}

class TeleportSelfToEntranceAction : ModMenuButtonActionBase
{
    readonly ModMenuItemMetadata meta = new("Teleport Self To Entrance");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        var self = StartOfRound.Instance.localPlayerController;
        int id = 0; // Main entrance
        var entrances = GameObject.FindObjectsByType<EntranceTeleport>(FindObjectsSortMode.None);
        foreach (var entrance in entrances)
        {
            if (entrance.entranceId != id)
                continue;
                
            // IF inside, set outside, or vice-versa.
            if (self.isInsideFactory != entrance.isEntranceToBuilding)
            {
                entrance.TeleportPlayer(); // Teleport self
                return;
            }
        }
    }
}

class ToggleTestRoomAction : ModMenuButtonActionBase
{
    readonly ModMenuItemMetadata meta = new("Toggle Test Room");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        LCActionPatches.QMM_Instance.Debug_ToggleTestRoom();
    }
}