using System;
using System.Collections.Generic;
using ModMenuAPI.ModMenuItems.BaseItems;

namespace ModMenuAPI.ModMenuItems;

/// <summary>
/// An action button, interfaced by <c>OnClick</c>.
/// </summary>
public abstract class MMButtonAction : MMItemBase
{
    /// <inheritdoc/>
    public sealed override Type ItemType { get; } = typeof(MMButtonAction);
    /// <inheritdoc/>
    protected MMButtonAction(string itemName) : base(itemName) { }
    /// <inheritdoc/>
    protected MMButtonAction(string itemName, string tooltip) : base(itemName, tooltip) { }
    /// <inheritdoc/>
    protected MMButtonAction(MMItemMetadata metadata) : base(metadata) { }

    /// <inheritdoc/>
    protected abstract void OnClick();
    /// <inheritdoc/>
    public override void CommonInvoke()
    {
        if(!Clickable)
            return;

        OnClick();
    }
}

/// <summary>
/// A button which contains multiple action buttons after clicking. Interfaced by <c>OnMenuOpened</c> and <c>OnMenuClosed</c>.<br/>
/// Consider using <c><see cref="MMButtonMenuInstantiable"/></c> if your context menu button doesn't need special behaviors.
/// </summary>
public abstract class MMButtonMenu : MMItemBase
{
    /// <inheritdoc/>
    public sealed override Type ItemType { get; } = typeof(MMButtonMenu);

    /// <inheritdoc/>
    protected MMButtonMenu(string itemName)                             : this(itemName, tooltip: null) { }
    /// <inheritdoc cref="MMButtonMenu(string, string, List{MMItemBase})"/> // Why does this one need the cref to work???
    protected MMButtonMenu(string itemName, List<MMItemBase> menuItems) : this(itemName, tooltip: null, menuItems) { }


    /// <inheritdoc/>
    protected MMButtonMenu(string itemName, string? tooltip)             : this(itemName, tooltip, []) { }
    /// <inheritdoc cref="MMItemBase(string, string)"/> <param name="itemName"></param> <param name="tooltip"></param>
    /// <param name="menuItems">The list of menu items in this menu.</param>
    protected MMButtonMenu(string itemName, string? tooltip, List<MMItemBase> menuItems) : this(new MMItemMetadata(itemName, tooltip), menuItems) { }


    /// <inheritdoc/>
    protected MMButtonMenu(MMItemMetadata metadata) : this(metadata, new()) { }
    /// <inheritdoc cref="MMItemBase(MMItemMetadata)"/> <param name="metadata"></param>
    /// <param name="menuItems">The list of menu items in this menu.</param>
    protected MMButtonMenu(MMItemMetadata metadata, List<MMItemBase> menuItems) : base(metadata)
    {
        MenuItems = menuItems;
    }

    /// <summary>
    /// The list of menu items in this menu.
    /// </summary>
    public readonly List<MMItemBase> MenuItems;
    
    /// <summary>
    /// Runs when the submenu is opened.
    /// </summary>
    public abstract void OnMenuOpened();
    /// <summary>
    /// Runs when the submenu is closed.
    /// </summary>
    public abstract void OnMenuClosed();
    /// <inheritdoc/>
    public sealed override void CommonInvoke()
    {
        if(!Clickable)
            return;

        OnMenuOpened();
    }
}

/// <summary>
/// A basic button that opens a context menu. Use <c>MenuItems</c> to manage the items this context menu contains.
/// </summary>
public sealed class MMButtonMenuInstantiable : MMButtonMenu
{
    /// <inheritdoc/>
    public MMButtonMenuInstantiable(string itemName) : base(itemName) { }
    /// <inheritdoc/>
    public MMButtonMenuInstantiable(string itemName, List<MMItemBase> menuItems) : base(itemName, menuItems) { }


    /// <inheritdoc/>
    public MMButtonMenuInstantiable(string itemName, string tooltip) : base(itemName, tooltip) { }
    /// <inheritdoc/>
    public MMButtonMenuInstantiable(string itemName, string tooltip, List<MMItemBase> menuItems) : base(itemName, tooltip, menuItems) { }


    /// <inheritdoc/>
    public MMButtonMenuInstantiable(MMItemMetadata metadata) : base(metadata) { }
    /// <inheritdoc/>
    public MMButtonMenuInstantiable(MMItemMetadata metadata, List<MMItemBase> menuItems) : base(metadata, menuItems) { }
    /// <inheritdoc/>
    public override void OnMenuOpened() { }
    /// <inheritdoc/>
    public override void OnMenuClosed() { }
}

/// <summary>
/// A toggle button, interfaced by <c>OnEnable</c> and <c>OnDisable</c>
/// </summary>
public abstract class MMButtonToggle : MMItemBase
{
    /// <inheritdoc/>
    public sealed override Type ItemType { get; } = typeof(MMButtonToggle);

    /// <inheritdoc/>
    protected MMButtonToggle(string itemName)                   : base(itemName) { }
    /// <inheritdoc/>
    protected MMButtonToggle(string itemName, string tooltip)   : base(itemName, tooltip) { }
    /// <inheritdoc/>
    protected MMButtonToggle(MMItemMetadata metadata)           : base(metadata) { }

    private bool _enabled = false;
    /// <summary>
    /// The state of the toggle. Toggling this will call <c>OnEnable</c> / <c>OnDisable</c>.
    /// </summary>
    public bool Enabled
    {
        get { return _enabled; }
        set 
        {
            if (_enabled == value)
                return;

            _enabled = value;

            if (value) 
                OnEnable();
            else 
                OnDisable();
        }
    }
    /// <summary>
    /// Runs when <c>Enabled</c> is set to <c>true</c> and the value changed.
    /// </summary>
    protected abstract void OnEnable();
    /// <summary>
    /// Runs when <c>Enabled</c> is set to <c>false</c> and the value changed.
    /// </summary>
    protected abstract void OnDisable();
    /// <inheritdoc/>
    public sealed override void CommonInvoke()
    {
        if(!Clickable)
            return;

        switch(_enabled)
        {
            case true: Enabled = false; break;
            case false: Enabled = true; break;
        }
    }
}

/// <summary>
/// A basic toggle button. Use <c>Enabled</c> to get the state of the button.
/// </summary>
public sealed class MMButtonToggleInstantiable : MMButtonToggle
{
    /// <inheritdoc/>
    public MMButtonToggleInstantiable(string itemName)                  : base(itemName) { }
    /// <inheritdoc/>
    public MMButtonToggleInstantiable(MMItemMetadata metadata)          : base(metadata) { }
    /// <inheritdoc/>
    public MMButtonToggleInstantiable(string itemName, string tooltip)  : base(itemName, tooltip) { }
    /// <inheritdoc/>
    protected override void OnDisable() { }
    /// <inheritdoc/>
    protected override void OnEnable() { }
}