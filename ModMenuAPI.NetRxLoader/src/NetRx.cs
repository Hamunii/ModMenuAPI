using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;

namespace NetRx;

public class HotLoader : IDisposable
{
    public Assembly? LastLoaded { get; private set; }
    public FileSystemWatcher FileWatcher { get; private set; }
        
    public delegate void HotLoadEvent(Assembly? prevAssembly, Assembly newAssembly);
    public event HotLoadEvent? OnHotLoadOccurred;

    public HotLoader(string hotDll)
    {
        FileWatcher = new FileSystemWatcher()
        {
            Path = Path.GetDirectoryName(hotDll),
            Filter = Path.GetFileName(hotDll),
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.LastWrite
        };
    }

    public void Watch()
    {
        FileWatcher.Changed += (sender, args) => HotLoad(args.FullPath);
        HotLoad(Path.Combine(FileWatcher.Path, FileWatcher.Filter));
    }

    public void Dispose()
    {
        FileWatcher?.Dispose();
    }
    
    private void HotLoad(string filePath)
    {
        Log("Hot Loading " + Path.GetFileName(filePath));

        using var dll = AssemblyDefinition.ReadAssembly(filePath);
        using var ms = new MemoryStream();
        dll.Name.Name = $"{dll.Name.Name}-{DateTime.Now.Ticks}";
        dll.Write(ms);
        var newLoaded = Assembly.Load(ms.ToArray());

        OnHotLoadOccurred?.Invoke(LastLoaded, newLoaded);
        LastLoaded = newLoaded;
    }
    private static void Log(object o) => Console.WriteLine("[HotLoader] " + o);
}