using BepInEx;
using BepInEx.Logging;

namespace ModMenuAPI;

internal static class MMLog
{
    internal static ManualLogSource Logger = null!;
    internal static void Log(object data)
    {
        Logger.LogInfo(data);
    }
}