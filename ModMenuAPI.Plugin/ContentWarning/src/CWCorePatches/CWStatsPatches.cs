using ModMenuAPI.ModMenuItems;

namespace PluginCW.CorePatches;

class CWStatsPatches
{
    const string menuTitle = "Stats";
    internal static void Init()
    {
        ModMenu.RegisterItem(new SetMoneyPatch(), menuTitle);
        ModMenu.RegisterItem(new ResetMoneyPatch(), menuTitle);

        ModMenu.RegisterItem(new SetMetaCoinsPatch(), menuTitle);
        ModMenu.RegisterItem(new ResetMetaCoinsPatch(), menuTitle);

        ModMenu.RegisterItem(new NextDayPatch(), menuTitle);
        ModMenu.RegisterItem(new FulfillQuotaPatch(), menuTitle);
    }
}

class SetMoneyPatch : ModMenuButtonActionBase
{
    readonly ModMenuItemMetadata meta = new("Set Money");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 100000000;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
    }
}

class ResetMoneyPatch : ModMenuButtonActionBase
{
    readonly ModMenuItemMetadata meta = new("Reset Money");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 0;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
    }
}

class SetMetaCoinsPatch : ModMenuButtonActionBase
{
    readonly ModMenuItemMetadata meta = new("Set Meta Coins");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        MetaProgressionHandler.SetMetaCoins(100000000);
    }
}

class ResetMetaCoinsPatch : ModMenuButtonActionBase
{
    readonly ModMenuItemMetadata meta = new("Reset Meta Coins");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        MetaProgressionHandler.SetMetaCoins(0);
    }
}

class NextDayPatch : ModMenuButtonActionBase
{
    ModMenuItemMetadata meta = new("Next Day");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.NextDay();
    }
}

class FulfillQuotaPatch : ModMenuButtonActionBase
{
    readonly ModMenuItemMetadata meta = new("Fulfill Quota");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.CurrentQuota = SurfaceNetworkHandler.RoomStats.QuotaToReach;
    }
}