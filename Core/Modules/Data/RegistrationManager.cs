namespace Squiggles.Core.Scenes.Registration;

using System.Collections.Generic;
using System.Linq;
using Godot;
using Squiggles.Core.Attributes;
using Squiggles.Core.Data;
using Squiggles.Core.Error;
using Squiggles.Core.Events;
using Squiggles.Core.WorldEntity;

/// <summary>
/// The superpower of resource registration, identification, and collection! Used to load resources dynamically from directories which allows mods to add new resources easily. Define known resources to register in the <see cref="SquigglesCoreConfigFile"/> by their type name.
/// </summary>
public partial class RegistrationManager : Node {

  /// <summary>
  /// A struct for storing the data of a registry. Each single registry holds resources based on some ID.
  /// </summary>
  private struct InternalRegistry {
    public string Path;
    public Dictionary<string, Resource> Dict;
  }

  private static readonly Dictionary<string, InternalRegistry> _registries = new();

  [MarkForRefactor("Refactor into _registries", "This is used in CuteFarmSim but is unnecessary outside of that context. Intended to be removed in favour of the configuration registry system")]
  public static Dictionary<string, WorldEntity> Entities { get; private set; } = new();

  /// <summary>
  /// Registers a single registry type. Called internally with configuration data
  /// </summary>
  /// <param name="type">the typename of the base resource type (i.e. "WorldEntity"</param>
  /// <param name="path">the path to the given resource directory. (i.e. "res://src/resource_registry/WorldEntity/")</param>
  public static void RegisterRegistryType(string type, string path) {
    var registry = new InternalRegistry {
      Path = path,
      Dict = new(),
    };
    _registries[type] = registry;
  }

  /// <summary>
  /// Attempts to find a given resource for the id and cast to the type.
  /// </summary>
  /// <typeparam name="T">The type desired. Must be the base class type that was registered</typeparam>
  /// <param name="id">the registered id of the resource</param>
  /// <returns>the resource, cast to T, or null on failure.</returns>
  public static T GetResource<T>(string id) where T : Resource {
    var type_name = typeof(T).Name;
    if (!_registries.ContainsKey(type_name)) {
      Print.Warn($"No registry found for type <{type_name}>");
      return null;
    }

    var registry = _registries[type_name];
    return !registry.Dict.ContainsKey(id) ? null : registry.Dict[id] as T;
  }

  public override void _Ready() {
    ReloadRegistries();
    EventBus.Data.OnModsLoaded += () => {
      Print.Debug("Mods loaded. Reloading registries");
      ReloadRegistries();
    };
  }

  /// <summary>
  /// Iterates through known registry types and loads the data into the registry. This refreshes old data with new data as well as discovering ner additions.
  /// </summary>
  public static void ReloadRegistries() {
    // TODO: Remove old data as well?
    Print.Debug("Begin registration", typeof(RegistrationManager).FullName);
    foreach (var type in _registries.Keys) {
      var registry = _registries[type];
      // TODO: create an IIDProvider interface that we can use to acquire IDs from any arbitrary resource implementation and use resource name as a fallback only
      registry.Dict = LoadRegistry<Resource>(registry.Path, DefaultIDParse, null, type);
      _registries[type] = registry;
    }
  }

  private static string DefaultIDParse(Resource res) => res is IRegistryID regID ? regID.GetRegistryID() : res.ResourceName;

  private delegate string GetIDFor<T>(T obj) where T : Resource; // Delegates for callbacks are frickin awesome. Eat your heart out GDScript!
  private delegate void AddToDictionary<T>(Dictionary<string, T> dict, string id, T obj) where T : Resource;
  private static void DefaultAddToDict<T>(Dictionary<string, T> dict, string id, T obj) where T : Resource => dict[id] = obj;
  private static Dictionary<string, T> LoadRegistry<T>(string root_dir, GetIDFor<T> idGenCallback, AddToDictionary<T> addDict, string label = "unlabeled") where T : Resource {
    var registry = new Dictionary<string, T>();
    var dir = DirAccess.Open(root_dir);
    addDict ??= DefaultAddToDict;
    if (dir is null) {
      Print.Debug($"Failed to find root path for resource [{label}], expected path : {root_dir}");
      return registry; // empty registry
    }
    var files = GetAllFilesRecursive(dir).Where((s) => s.Contains(".tres"));

    foreach (var f in files) {
      var fileName = f.Replace(".remap", ""); // clear remap files to load default version
      var temp = GD.Load<T>(fileName);
      if (temp is not null) {
        addDict(registry, idGenCallback(temp), temp);
      }
      else {
        Print.Debug($"file '{fileName}' is not valid for type [{label}]");
      }
    }
    PrintRegistry(registry, label);
    return registry;
  }

  private static void PrintRegistry<T>(Dictionary<string, T> reg, string label) where T : Resource {
    Print.Debug($"----['{label}' Registry ({reg.Count} elements) ]----");
    foreach (var pair in reg) {
      Print.Debug($"] '{pair.Key}' : {pair.Value.ResourcePath}");
    }
    Print.Debug($"----[End Registry ]----");
  }

  private static List<string> GetAllFilesRecursive(DirAccess dir) {
    var files = new List<string>();
    foreach (var f in dir.GetFiles()) {
      files.Add(dir.GetCurrentDir().PathJoin(f));
    }

    foreach (var d in dir.GetDirectories()) {
      if (d.StartsWith("_")) {
        continue; // allow special hidden folders
      }

      var subdir = DirAccess.Open(dir.GetCurrentDir().PathJoin(d));
      if (subdir is null) {
        continue;
      }

      files.AddRange(GetAllFilesRecursive(subdir));
    }
    return files;
  }

}
