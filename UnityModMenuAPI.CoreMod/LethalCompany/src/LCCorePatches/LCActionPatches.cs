using UnityEngine;
using UnityModMenuAPI.ModMenuItems;

namespace CoreModLC.CorePatches;

class LCActionPatches
{
    const string menuTitle = "Action";
    internal static void Init()
    {
        ModMenu.RegisterModMenuItem(new TeleportSelfToEntranceAction(), menuTitle);
    }
}

class TeleportSelfToEntranceAction : ModMenuButtonAction
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
