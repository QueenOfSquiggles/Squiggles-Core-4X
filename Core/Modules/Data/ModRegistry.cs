using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Godot;
using queen.error;
using queen.modification;

namespace queen.data;

public static class ModRegistry
{

    private const string MODS_PATH = "user://Mods";
    public static int LoadedMods { get; private set; } = 0;
    private static List<IModificationAdapter> mods = new();



    public static void OnRegisterMods()
    {
        Print.Debug($"[ModRegistry] Loaded {mods.Count} mods");
        foreach (var mod in mods)
            mod.OnRegister();
    }

    public static void OnUnRegisterMods()
    {
        foreach (var mod in mods)
            mod.OnUnRegister();
    }


    public static void LoadModsRecursively()
    {
        if (OS.HasFeature("editor"))
        {
            // Print.Warn("Mod loading is disabled while in editor. Use an exported version in debug mode for testing mods.");
            return;
        }

        LoadedMods = 0;
        var directory = ProjectSettings.GlobalizePath(MODS_PATH);
        if (!DirAccess.DirExistsAbsolute(directory)) DirAccess.MakeDirRecursiveAbsolute(directory);
        using var dir = DirAccess.Open(directory);
        if (dir == null)
        {
            Print.Error($"Failed to load mods during mod step. Error: {DirAccess.GetOpenError()}");
            return;
        }
        dir.IncludeHidden = false;
        dir.IncludeNavigational = false;
        var dirs = dir.GetDirectories();
        foreach (var modDir in dirs)
            LoadModFromDir(directory.PathJoin(modDir));
        LoadModFromDir(directory); // try load mod from root, has unreliable results with multiple mods
    }

    private static void LoadModFromDir(string directory)
    {
        Print.Debug($"Attempting to load mod from directory: '{directory}'");
        using var dir = DirAccess.Open(directory);
        if (dir == null)
        {
            Print.Warn("failed to open dir");
            return;
        }
        string? packFile = null;
        string? dllFile = null;
        foreach (var file in dir.GetFiles())
        {
            if (file.ToLower().EndsWith("dll")) dllFile = file;
            if (file.ToLower().EndsWith("pck")) packFile = file;
        }
        if (packFile is null)
        {
            Print.Warn("No pack file found. Not a valid mod");
            return;
        }
        Print.Debug($"Found pack file '{packFile}'" + (dllFile is null ? "" : $", DLL found '{dllFile}'"));

        // DLL is technically optional. Asset swapping doesn't require code. And some small mods could just use GDScript
        if (dllFile is not null) Assembly.LoadFile(directory.PathJoin(dllFile));

        // Load in the pack file. By default files are replaced to allow asset swapping.
        var result = ProjectSettings.LoadResourcePack(directory.PathJoin(packFile));
        if (!result)
        {
            Print.Error($"Failed to load Patch/Mod archive from: {packFile}");
            return;
        }
        LoadedMods++;
        Print.Debug($"Loaded modification from {packFile}");
        if (dllFile is not null)
        {
            // Mod has loaded C# code. Find the adapter and get things set up.
            // example adapter path: res://HD_Models_And_Textures/ModAdapter.cs
            var modName = packFile.GetBaseName();
            var modAdapterScriptPath = $"res://{modName}/ModAdapter.cs";
            if (ResourceLoader.Load(modAdapterScriptPath) is not CSharpScript)
            {
                Print.Error($"Failed to load C# script from {modAdapterScriptPath}. In order to receive method calls, an adapter must be placed in this exact position for this mod '{modName}'");
                return;
            }
            // LoadAdapterFromPointer(script.NativeInstance, modName);
        }
    }



    // TODO unsafe code must be compiled with the unsafe flag. How to change compile flags for Godot?
    // private unsafe static void LoadAdapterFromPointer(IntPtr ptr, string modName)
    // {
    //     try
    //     {
    //         var objRef = Marshal.PtrToStructure(ptr, typeof(IModificationAdapter));
    //         if (objRef is IModificationAdapter mod)
    //         {
    //             mods.Add(mod);
    //             Print.Warn($"C# Context has been found for mod '{modName}'. Never allow this with code you do not trust.");
    //         }
    //     }
    //     catch (Exception e)
    //     {
    //         Print.Error($"Failed to load C# Adapter for mod '{modName}'. Error: {e.GetBaseException()}\n---------\n\t{e}");
    //     }

    // }
}