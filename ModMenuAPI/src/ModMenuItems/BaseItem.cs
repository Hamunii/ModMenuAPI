using System;
using System.Reflection;

namespace ModMenuAPI.ModMenuItems.BaseItems;

/// <summary>
/// A base class to derive from by menu item classes. <br/>
/// Only use this if you are developing new button types for ModMenuAPI.
/// </summary>
public abstract class MMItemBase
{
    /// <inheritdoc cref="MMItemBase(string, string)" />
    public MMItemBase(string itemName) : this(itemName, null) { }
    /// <summary>
    /// A Constructor for this menu item.
    /// </summary>
    /// <param name="itemName">The name of this item. Internally used as the <c>itemName</c> argument of <see cref="MMItemMetadata"/></param>
    /// <param name="tooltip">The tooltip for your menu item. Shown when hovering on the item. Internally used as the <c>tooltip</c> argument of <see cref="MMItemMetadata"/></param>
    public MMItemBase(string itemName, string? tooltip) : this(new MMItemMetadata(itemName, tooltip)) { }
    /// <summary>
    /// A Constructor for this menu item.
    /// </summary>
    /// <param name="metadata">The metadata for this item.</param>
    public MMItemBase(MMItemMetadata metadata)
    {
        Metadata = metadata;
        if (Metadata.InvokeOnInit)
            CommonInvoke();
    }

    /// <summary>
    /// The type of this menu item.
    /// </summary>
    public abstract Type ItemType { get; }

    /// <summary>
    /// The metadata for this menu item.
    /// </summary>
    public readonly MMItemMetadata Metadata;

    /// <summary>
    /// The common interface for clicking different types of buttons programmatically.
    /// </summary>
    public abstract void CommonInvoke();

    /// <summary>
    /// Determines whether or not this button can be clicked. Useful if an action requires specific circumstances to be executed successfully.
    /// </summary>
    public bool Clickable { get; set; } = true;

    internal Assembly ParentAssembly = null!;
}