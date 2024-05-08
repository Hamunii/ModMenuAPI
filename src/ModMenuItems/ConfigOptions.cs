namespace UnityModMenuAPI.ModMenuItems;

/// <summary>
/// A generic permission system for games.<br/>
/// This only matters for multiplayer games, and you should set this based on
/// if the patch only works for e.g. the host due to networking limitations.
/// This option merely exists as an option to hide patches that don't work.
/// </summary>
public enum CommonPermission
{
    /// <summary>
    /// This menu item is only shown for the host.
    /// </summary>
    HostOnly,
    /// <summary>
    /// This menu item is shown for all clients.
    /// </summary>
    AllClients
}

/// <summary>
/// Metadata for a mod menu item.
/// </summary>
public class ModMenuItemConfig
{
    /// <summary>
    /// The name of your menu item.
    /// </summary>
    public string Name = "Unnamed Item";
    /// <summary>
    /// The tooltip for your menu item. Shown when hovering on the item. 
    /// </summary>
    public string? ToolTip;
    /// <summary>
    /// The permission visibility for this menu item. If permission isn't met, this menu item will be hidden.
    /// </summary>
    public CommonPermission Permission = CommonPermission.AllClients;
}
