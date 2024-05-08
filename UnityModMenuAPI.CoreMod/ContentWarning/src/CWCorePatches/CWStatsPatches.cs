using UnityModMenuAPI.ModMenuItems;

namespace CoreModCW.CorePatches;

class CWStatsPatches
{
    const string menuTitle = "Stats";
    internal static void Init()
    {
        ModMenu.RegisterModMenuItem(new SetMoneyPatch(), menuTitle);
        ModMenu.RegisterModMenuItem(new ResetMoneyPatch(), menuTitle);

        ModMenu.RegisterModMenuItem(new SetMetaCoinsPatch(), menuTitle);
        ModMenu.RegisterModMenuItem(new ResetMetaCoinsPatch(), menuTitle);

        ModMenu.RegisterModMenuItem(new NextDayPatch(), menuTitle);
        ModMenu.RegisterModMenuItem(new FulfillQuotaPatch(), menuTitle);
    }
}

class SetMoneyPatch : ModMenuButtonAction
{
    readonly ModMenuItemMetadata meta = new("Set Money");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 100000000;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
    }
}

class ResetMoneyPatch : ModMenuButtonAction
{
    readonly ModMenuItemMetadata meta = new("Reset Money");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 0;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
    }
}

class SetMetaCoinsPatch : ModMenuButtonAction
{
    readonly ModMenuItemMetadata meta = new("Set Meta Coins");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        MetaProgressionHandler.SetMetaCoins(100000000);
    }
}

class ResetMetaCoinsPatch : ModMenuButtonAction
{
    readonly ModMenuItemMetadata meta = new("Reset Meta Coins");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        MetaProgressionHandler.SetMetaCoins(0);
    }
}

class NextDayPatch : ModMenuButtonAction
{
    ModMenuItemMetadata meta = new("Next Day");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.NextDay();
    }
}

class FulfillQuotaPatch : ModMenuButtonAction
{
    readonly ModMenuItemMetadata meta = new("Fulfill Quota");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.CurrentQuota = SurfaceNetworkHandler.RoomStats.QuotaToReach;
    }
}