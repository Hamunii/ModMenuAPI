using UnityModMenuAPI.ModMenuItems.BaseItems;

namespace UnityModMenuAPI.ModMenuItems;

/// <summary>
/// An action button, interfaced by <c>OnClick</c>.
/// </summary>
public abstract class ModMenuButtonAction : ModMenuBaseItem
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
/// A toggle button, interfaced by <c>OnEnable</c> and <c>OnDisable</c>
/// </summary>
public abstract class ModMenuButtonToggle : ModMenuBaseItem
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