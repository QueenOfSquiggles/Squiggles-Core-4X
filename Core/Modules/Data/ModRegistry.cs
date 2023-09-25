namespace Squiggles.Core.Data;

using System.Collections.Generic;
using System.Reflection;
using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Modification;

/// <summary>
/// The singleton which handles mod loading. As it stands, this mostly only supports resource swapping and GDScript. You would need to compile with unsafe code enabled to be able to load C# assembly files at runtime. I'm gonna assume that if you're smart enough to make that work, you're smart enough to add that feature too because I could only get part way. See commented out code for my attempts (T.T)
///
/// Also an important note, all of the methods in this are called internally. Basically no touchey my stuffey!
/// </summary>
public static class ModRegistry {

  private const string MODS_PATH = "user://Mods";

  /// <summary>
  /// A publicly available count of how many mods have been loaded. Not currently used internally but if you find a use for it then enjoy!
  /// </summary>
  public static int LoadedMods { get; private set; }
  private static readonly List<IModificationAdapter> _mods = new();



  public static void OnRegisterMods() {
    Print.Debug($"[ModRegistry] Loaded {_mods.Count} mods");
    foreach (var mod in _mods) {
      mod.OnRegister();
    }
  }

  public static void OnUnRegisterMods() {
    foreach (var mod in _mods) {
      mod.OnUnRegister();
    }
  }


  public static void LoadModsRecursively() {
    if (OS.HasFeature("editor")) {
      // Print.Warn("Mod loading is disabled while in editor. Use an exported version in debug mode for testing mods.");
      return;
    }

    LoadedMods = 0;
    var directory = ProjectSettings.GlobalizePath(MODS_PATH);
    if (!DirAccess.DirExistsAbsolute(directory)) {
      DirAccess.MakeDirRecursiveAbsolute(directory);
    }

    using var dir = DirAccess.Open(directory);
    if (dir == null) {
      Print.Error($"Failed to load mods during mod step. Error: {DirAccess.GetOpenError()}");
      return;
    }
    dir.IncludeHidden = false;
    dir.IncludeNavigational = false;
    var dirs = dir.GetDirectories();
    foreach (var modDir in dirs) {
      LoadModFromDir(directory.PathJoin(modDir));
    }

    LoadModFromDir(directory); // try load mod from root, has unreliable results with multiple mods
  }

  private static void LoadModFromDir(string directory) {
    Print.Debug($"Attempting to load mod from directory: '{directory}'");
    using var dir = DirAccess.Open(directory);
    if (dir == null) {
      Print.Warn("failed to open dir");
      return;
    }
    string packFile = null;
    string dllFile = null;
    foreach (var file in dir.GetFiles()) {
      if (file.ToLower().EndsWith("dll")) {
        dllFile = file;
      }

      if (file.ToLower().EndsWith("pck")) {
        packFile = file;
      }
    }
    if (packFile is null) {
      Print.Warn("No pack file found. Not a valid mod");
      return;
    }
    Print.Debug($"Found pack file '{packFile}'" + (dllFile is null ? "" : $", DLL found '{dllFile}'"));

    // DLL is technically optional. Asset swapping doesn't require code. And some small mods could just use GDScript
    if (dllFile is not null) {
      Assembly.LoadFile(directory.PathJoin(dllFile));
    }

    // Load in the pack file. By default files are replaced to allow asset swapping.
    var result = ProjectSettings.LoadResourcePack(directory.PathJoin(packFile));
    if (!result) {
      Print.Error($"Failed to load Patch/Mod archive from: {packFile}");
      return;
    }
    LoadedMods++;
    Print.Debug($"Loaded modification from {packFile}");
    if (dllFile is not null) {
      // Mod has loaded C# code. Find the adapter and get things set up.
      // example adapter path: res://HD_Models_And_Textures/ModAdapter.cs
      var modName = packFile.GetBaseName();
      var modAdapterScriptPath = $"res://{modName}/ModAdapter.cs";
      if (ResourceLoader.Load(modAdapterScriptPath) is not CSharpScript) {
        Print.Error($"Failed to load C# script from {modAdapterScriptPath}. In order to receive method calls, an adapter must be placed in this exact position for this mod '{modName}'");
        return;
      }
      // LoadAdapterFromPointer(script.NativeInstance, modName);
    }
  }



  // INFO: this requires "<AllowUnsafeBlocks>true</AllowUnsafeBlocks>" to be in the csproj file. Otherwise it will throw an error
  // If that is something you want for your game, you can do a bit of tweaking here to ensure the C# dlls are loaded properly. Otherwise this feature is omitted for safety. Without this feature, resource overrides and custom GDScript can still be integrated.

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
