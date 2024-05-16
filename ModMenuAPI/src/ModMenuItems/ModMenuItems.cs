using System.Collections.Generic;
using ModMenuAPI.ModMenuItems.BaseItems;

namespace ModMenuAPI.ModMenuItems;

/// <summary>
/// An action button, interfaced by <c>OnClick</c>.
/// </summary>
public abstract class MMButtonAction : MMItemBase
{
    protected MMButtonAction(string itemName) : base(itemName) { }
    protected MMButtonAction(string itemName, string tooltip) : base(itemName, tooltip) { }
    protected MMButtonAction(MMItemMetadata metadata) : base(metadata) { }

    internal override MMItemType ItemType => MMItemType.ActionButton;

    public abstract void OnClick();
    public override void CommonInvoke()
    {
        if(!Clickable)
            return;

        OnClick();
    }
}

/// <summary>
/// A button which contains multiple action buttons after clicking. Interfaced by <c>OnMenuOpened</c> and <c>OnMenuClosed</c>.<br/>
/// Consider using <c><see cref="MMButtonContextMenuInstantiable"/></c> if your context menu button doesn't need special behaviors.
/// </summary>
public abstract class MMButtonContextMenu : MMItemBase
{
    protected MMButtonContextMenu(string itemName) : this(new MMItemMetadata(itemName), new()) { }
    protected MMButtonContextMenu(string itemName, List<MMItemBase> menuItems) : this(new MMItemMetadata(itemName), menuItems) { }

    protected MMButtonContextMenu(string itemName, string tooltip) : this(new MMItemMetadata(itemName, tooltip), new()) { }
    protected MMButtonContextMenu(string itemName, string tooltip, List<MMItemBase> menuItems) : this(new MMItemMetadata(itemName, tooltip), menuItems) { }
    
    protected MMButtonContextMenu(MMItemMetadata metadata) : this(metadata, new()) { }
    protected MMButtonContextMenu(MMItemMetadata metadata, List<MMItemBase> menuItems) : base(metadata)
    {
        MenuItems = menuItems;
    }

    internal override MMItemType ItemType => MMItemType.ContextMenu;
    public readonly List<MMItemBase> MenuItems;
    public abstract void OnMenuOpened();
    public abstract void OnMenuClosed();
    public override void CommonInvoke()
    {
        if(!Clickable)
            return;

        OnMenuOpened();
    }
}

/// <summary>
/// A basic button that opens a context menu. Use <c>MenuItems</c> to manage the items this context menu contains.
/// </summary>
public class MMButtonContextMenuInstantiable : MMButtonContextMenu
{
    public MMButtonContextMenuInstantiable(string itemName) : base(itemName) { }
    public MMButtonContextMenuInstantiable(string itemName, List<MMItemBase> menuItems) : base(itemName, menuItems) { }

    public MMButtonContextMenuInstantiable(string itemName, string tooltip) : base(itemName, tooltip) { }
    public MMButtonContextMenuInstantiable(string itemName, string tooltip, List<MMItemBase> menuItems) : base(itemName, tooltip, menuItems) { }
    
    public MMButtonContextMenuInstantiable(MMItemMetadata metadata) : base(metadata) { }
    public MMButtonContextMenuInstantiable(MMItemMetadata metadata, List<MMItemBase> menuItems) : base(metadata, menuItems) { }

    public override void OnMenuOpened() { }
    public override void OnMenuClosed() { }
}

/// <summary>
/// A toggle button, interfaced by <c>OnEnable</c> and <c>OnDisable</c>
/// </summary>
public abstract class MMButtonToggle : MMItemBase
{
    internal override MMItemType ItemType => MMItemType.ToggleButton;
    private bool _enabled = false;

    protected MMButtonToggle(string itemName) : base(itemName) { }
    protected MMButtonToggle(string itemName, string tooltip) : base(itemName, tooltip) { }
    protected MMButtonToggle(MMItemMetadata metadata) : base(metadata) { }

    /// <summary>
    /// State of the patch.
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
    public override void CommonInvoke()
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
public class MMButtonToggleInstantiable : MMButtonToggle
{
    public MMButtonToggleInstantiable(string itemName) : base(itemName) { }
    public MMButtonToggleInstantiable(MMItemMetadata metadata) : base(metadata) { }
    public MMButtonToggleInstantiable(string itemName, string tooltip) : base(itemName, tooltip) { }

    protected override void OnDisable() { }
    protected override void OnEnable() { }
}