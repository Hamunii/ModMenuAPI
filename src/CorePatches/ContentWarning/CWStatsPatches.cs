using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityModMenuAPI.ModMenuItems;

namespace UnityModMenuAPI.CorePatches.CW;

class CWStatsPatches
{
    const string menuTitle = "Stats";
    internal static void Init()
    {
        ModMenu.RegisterModMenuItem(new SetMoneyPatch(), menuTitle);
        ModMenu.RegisterModMenuItem(new ResetMoneyPatch(), menuTitle);
        ModMenu.RegisterModMenuItem(new NextDayPatch(), menuTitle);
        ModMenu.RegisterModMenuItem(new FulfillQuotaPatch(), menuTitle);
    }
}

class SetMoneyPatch : ModMenuButtonAction
{
    ModMenuItemMetadata meta = new("Set Money");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 100000000;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
    }
}

class ResetMoneyPatch : ModMenuButtonAction
{
    ModMenuItemMetadata meta = new("Reset Money");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 0;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
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
    ModMenuItemMetadata meta = new("Fulfill Quota");
    public override ModMenuItemMetadata Metadata => meta;

    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.CurrentQuota = SurfaceNetworkHandler.RoomStats.QuotaToReach;
    }
}