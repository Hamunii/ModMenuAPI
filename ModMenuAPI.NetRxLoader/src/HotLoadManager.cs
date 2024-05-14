using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NetRx;

namespace ModMenuAPI.NetRxLoader;

public interface IHotLoadManagerID0
{
    public void OnLoad();
    public void Dispose();
}

public interface IHotLoadManagerID1
{
    public void OnLoad();
    public void Dispose();
}

internal static class HotLoadManager
{
    static HotLoader hotLoaderAPI = null!;
    static IHotLoadManagerID0 HotLoadClassID0 = null!;
    static IHotLoadManagerID1 HotLoadClassID1 = null!;
    // static Type? hotLoadImplementationID0;
    // static Type? hotLoadImplementationID1;
    internal static void Init()
    {
        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        hotLoaderAPI = new HotLoader(Path.Combine(thisLocation, "Hamunii.ModMenuAPI.CoreBundle.dll.NetRx"));
        hotLoaderAPI.OnHotLoadOccurred += OnHotLoadOccurred;
        hotLoaderAPI.Watch();
    }

    private static void OnHotLoadOccurred(Assembly? prevAssembly, Assembly newAssembly)
    {
        // prevAssembly is null the first load.
        Plugin.Logger.LogInfo(prevAssembly != null ? "Hot reload detected!" : "New load detected!");
        
        HotLoadClassID0?.Dispose();
        HotLoadClassID1?.Dispose();

        var hotLoadImplementationID0 = GetTypesSafe(newAssembly).FirstOrDefault(typeInfo =>
            typeof(IHotLoadManagerID0).IsAssignableFrom(typeInfo));
        
        Plugin.Logger.LogInfo("thing: " + hotLoadImplementationID0);

        var hotLoadImplementationID1 = GetTypesSafe(newAssembly).FirstOrDefault(typeInfo =>
            typeof(IHotLoadManagerID1).IsAssignableFrom(typeInfo));

        if (hotLoadImplementationID0 is not null)
        {
            HotLoadClassID0 = (IHotLoadManagerID0)Activator.CreateInstance(hotLoadImplementationID0);
            HotLoadClassID1 = (IHotLoadManagerID1)Activator.CreateInstance(hotLoadImplementationID1);
            HotLoadClassID0?.OnLoad();
            HotLoadClassID1?.OnLoad();
        }
        else
        {
            Plugin.Logger.LogError("hotLoadImplementationID0 is null!");
        }
        
        //     HotLoadClass0?.OnLoad();
        // if (hotLoadImplementation is not null)
        // {
        //     var em = hotLoadImplementation.GetEnumerator();
        //     while(em.MoveNext())
        //         HotLoadClassID?.Dispose();
        // }
        
        // // Try to get any IHotLoadClass from the new DLL
        // hotLoadImplementation = GetTypesSafe(newAssembly).Where(typeInfo =>
        //     typeof(IHotLoadManagerID).IsAssignableFrom(typeInfo));

        // var emx = hotLoadImplementation.GetEnumerator();
        // while(emx.MoveNext())
        // {
        //     HotLoadClassID = (IHotLoadManagerID)Activator.CreateInstance(emx.Current);
        //     HotLoadClassID?.OnLoad();
        // }

        // hotLoadImplementation = GetTypesSafe(newAssembly).Where(typeInfo =>
        //     typeof(IHotLoadManagerID).IsAssignableFrom(typeInfo));

    }

    // Source: BepInEx.Debug ScriptEngine: https://github.com/BepInEx/BepInEx.Debug/blob/master/src/ScriptEngine/ScriptEngine.cs
    private static IEnumerable<Type> GetTypesSafe(Assembly ass)
    {
        try
        {
            return ass.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            var sbMessage = new StringBuilder();
            sbMessage.AppendLine("\r\n-- LoaderExceptions --");
            foreach (var l in ex.LoaderExceptions)
                sbMessage.AppendLine(l.ToString());
            sbMessage.AppendLine("\r\n-- StackTrace --");
            sbMessage.AppendLine(ex.StackTrace);
            Plugin.Logger.LogError(sbMessage.ToString());
            return ex.Types.Where(x => x != null);
        }
    }
}