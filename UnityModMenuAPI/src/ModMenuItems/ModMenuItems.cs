using System.Collections.Generic;
using UnityModMenuAPI.ModMenuItems.BaseItems;

namespace UnityModMenuAPI.ModMenuItems;

/// <summary>
/// An action button, interfaced by <c>OnClick</c>.
/// </summary>
public abstract class ModMenuButtonActionBase : ModMenuBaseItemBase
{
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
    internal override ModMenuItemType ItemType => ModMenuItemType.ContextMenu;
    public abstract List<ModMenuBaseItemBase> MenuItems { get; }
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
    private readonly List<ModMenuBaseItemBase> _menuItems = null!;
    private readonly ModMenuItemMetadata _metadata = null!;

    public ModMenuButtonContextMenuInstantiable(ModMenuItemMetadata metadata, List<ModMenuBaseItemBase>? modMenuItems)
    {
        _metadata = metadata;
        _menuItems = modMenuItems ?? new();
    }
    public ModMenuButtonContextMenuInstantiable(ModMenuItemMetadata meta)
        : this(meta, null) { }
    public ModMenuButtonContextMenuInstantiable(string menuName, List<ModMenuBaseItemBase>? modMenuItems)
        : this(new ModMenuItemMetadata(menuName), modMenuItems) { }
    public ModMenuButtonContextMenuInstantiable(string menuName)
        : this(new ModMenuItemMetadata(menuName), null) { }

    public override List<ModMenuBaseItemBase> MenuItems => _menuItems;
    public override ModMenuItemMetadata Metadata => _metadata;
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