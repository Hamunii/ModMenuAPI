using System.Collections.Generic;
using ModMenuAPI.ModMenuItems.BaseItems;

namespace ModMenuAPI.ModMenuItems;

/// <summary>
/// An action button, interfaced by <c>OnClick</c>.
/// </summary>
public abstract class ModMenuButtonActionBase : ModMenuBaseItemBase
{
    protected ModMenuButtonActionBase(string itemName) : base(itemName) { }
    protected ModMenuButtonActionBase(string itemName, string tooltip) : base(itemName, tooltip) { }
    protected ModMenuButtonActionBase(ModMenuItemMetadata metadata) : base(metadata) { }

    internal override ModMenuItemType ItemType => ModMenuItemType.ActionButton;

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
/// Consider using <c><see cref="ModMenuButtonContextMenuInstantiable"/></c> if your context menu button doesn't need special behaviors.
/// </summary>
public abstract class ModMenuButtonContextMenuBase : ModMenuBaseItemBase
{
    protected ModMenuButtonContextMenuBase(string itemName) : this(new ModMenuItemMetadata(itemName), new()) { }
    protected ModMenuButtonContextMenuBase(string itemName, List<ModMenuBaseItemBase> menuItems) : this(new ModMenuItemMetadata(itemName), menuItems) { }

    protected ModMenuButtonContextMenuBase(string itemName, string tooltip) : this(new ModMenuItemMetadata(itemName, tooltip), new()) { }
    protected ModMenuButtonContextMenuBase(string itemName, string tooltip, List<ModMenuBaseItemBase> menuItems) : this(new ModMenuItemMetadata(itemName, tooltip), menuItems) { }
    
    protected ModMenuButtonContextMenuBase(ModMenuItemMetadata metadata) : this(metadata, new()) { }
    protected ModMenuButtonContextMenuBase(ModMenuItemMetadata metadata, List<ModMenuBaseItemBase> menuItems) : base(metadata)
    {
        MenuItems = menuItems;
    }

    internal override ModMenuItemType ItemType => ModMenuItemType.ContextMenu;
    public readonly List<ModMenuBaseItemBase> MenuItems;
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
public class ModMenuButtonContextMenuInstantiable : ModMenuButtonContextMenuBase
{
    public ModMenuButtonContextMenuInstantiable(string itemName) : base(itemName) { }
    public ModMenuButtonContextMenuInstantiable(string itemName, List<ModMenuBaseItemBase> menuItems) : base(itemName, menuItems) { }

    public ModMenuButtonContextMenuInstantiable(string itemName, string tooltip) : base(itemName, tooltip) { }
    public ModMenuButtonContextMenuInstantiable(string itemName, string tooltip, List<ModMenuBaseItemBase> menuItems) : base(itemName, tooltip, menuItems) { }
    
    public ModMenuButtonContextMenuInstantiable(ModMenuItemMetadata metadata) : base(metadata) { }
    public ModMenuButtonContextMenuInstantiable(ModMenuItemMetadata metadata, List<ModMenuBaseItemBase> menuItems) : base(metadata, menuItems) { }

    public override void OnMenuOpened() { }
    public override void OnMenuClosed() { }
}

/// <summary>
/// A toggle button, interfaced by <c>OnEnable</c> and <c>OnDisable</c>
/// </summary>
public abstract class ModMenuButtonToggleBase : ModMenuBaseItemBase
{
    internal override ModMenuItemType ItemType => ModMenuItemType.ToggleButton;
    private bool _enabled = false;

    protected ModMenuButtonToggleBase(string itemName) : base(itemName) { }
    protected ModMenuButtonToggleBase(string itemName, string tooltip) : base(itemName, tooltip) { }
    protected ModMenuButtonToggleBase(ModMenuItemMetadata metadata) : base(metadata) { }

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
public class ModMenuButtonToggleInstantiable : ModMenuButtonToggleBase
{
    public ModMenuButtonToggleInstantiable(string itemName) : base(itemName) { }
    public ModMenuButtonToggleInstantiable(ModMenuItemMetadata metadata) : base(metadata) { }
    public ModMenuButtonToggleInstantiable(string itemName, string tooltip) : base(itemName, tooltip) { }

    protected override void OnDisable() { }
    protected override void OnEnable() { }
}