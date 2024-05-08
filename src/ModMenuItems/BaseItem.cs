namespace UnityModMenuAPI.ModMenuItems.BaseItems;

/// <summary>
/// A base class to derive from by menu item classes. <br/>
/// Only use this if you are developing new button types for UnityModMenuAPI.
/// </summary>
public abstract class ModMenuBaseItem
{
    /// <summary>
    /// The type of this menu item.
    /// </summary>
    internal abstract ModMenuItemType ItemType { get; }

    /// <summary>
    /// The metadata for this menu item.
    /// </summary>
    public abstract ModMenuItemConfig Config { get; }

    /// <summary>
    /// The common interface for clicking different types of buttons programmatically.
    /// </summary>
    public abstract void CommonInvoke();

    private bool _clickable = true;

    /// <summary>
    /// Determines whether or not this button can be clicked. Useful if an action requires specific circumstances to be executed successfully.
    /// </summary>
    public bool Clickable 
    {
        get { return _clickable; }
        set { _clickable = value; }
    }
}