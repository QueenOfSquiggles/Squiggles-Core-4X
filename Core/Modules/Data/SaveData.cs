namespace Squiggles.Core.Data;

using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using Squiggles.Core.Attributes;
using Squiggles.Core.Error;
using Squiggles.Core.Extension;

/// <summary>
/// The singleton for handling saving/loading data to/from disk. Also for managing save slots if your game uses them.
/// </summary>
public static class SaveData {
  private const string SAVE_SLOT_ROOT = "user://save_slot/";
  private static readonly DataPath _defaultDataPath = new("user://", false); // disallow deletion just in case bad code

  /// <summary>
  /// An alternate save path pointing towards the current save slot. This can be altered with SetSaveSlot.
  /// See also: <see cref="SetSaveSlot"/>
  /// </summary>
  public static DataPath CurrentSaveSlot { get; private set; } = new(SAVE_SLOT_ROOT + "default/");

  /// <summary>
  /// Checks the root directory of save slots and finds all save slot directories. Meta-data should be collected afterwards.
  /// </summary>
  /// <returns>A string array of save slots, sorted alphabetically </returns>
  public static string[] GetKnownSaveSlots() {
    using var dir = DirAccess.Open(SAVE_SLOT_ROOT);
    if (dir == null) {
      return Array.Empty<string>();
    }

    dir.IncludeHidden = false;
    dir.IncludeNavigational = false;
    return dir.GetDirectories();
  }

  /// <summary>
  /// Sets CurrentSaveSlot to point at a new save slot directory. This will force new files saved to save in that new directory.
  /// See also: <seealso cref="CurrentSaveSlot"/>
  /// </summary>
  /// <param name="slot_name">The name of the file slot. Must be valid for a directory name on the host system. </param>
  public static void SetSaveSlot(string slot_name) {
    if (!slot_name.IsValidFileName()) {
      Print.Error($"'{slot_name}' is not a valid file name. Make sure save slots are being saved using a name that is valid on this system!", typeof(SaveData).FullName);
      return;
    }
    var path = $"{SAVE_SLOT_ROOT}{slot_name}";
    if (!path.EndsWith('/')) {
      path += '/';
    }

    CurrentSaveSlot = new DataPath(path);
  }

  /// <summary>
  /// Loads the "default" save slot, which coincidentally is named "default"
  /// </summary>
  public static void LoadDefaultSaveSlot() => SetSaveSlot("default");

  /// <summary>
  /// Determines whether there exists save data for this game. Useful for knowing if this is the first session or not
  /// </summary>
  /// <returns>Tru if save data exists</returns>
  public static bool HasSaveData() => GetKnownSaveSlots().Length > 1;

  /// <summary>
  /// Looks through the save slot directory and finds the most recent save slot (based on save slot directory)
  /// </summary>
  public static void LoadMostRecentSaveSlot() {
    var slots = GetKnownSaveSlots();
    var most_recent = "default"; // default to default slot just in case no valid extra ones are found
    long recent_time = 0;
    foreach (var slot in slots) {
      if (slot == "default") {
        continue;
      }

      var date = ParseSaveSlotName(slot);
      var time = date.ToFileTimeUtc();
      if (time <= recent_time) {
        continue;
      }

      most_recent = slot;
      recent_time = time;
    };
    if (most_recent == "") {
      return;
    }

    SetSaveSlot(most_recent);
  }

  /// <summary>
  /// Saves data in JSON format to the specified path, relative to the root of "user://"
  /// </summary>
  /// <typeparam name="T">inferred type of data</typeparam>
  /// <param name="data">the serializable class to save</param>
  /// <param name="path">the path relative to "user://"</param>
  /// <param name="do_flush">Whether or not to force an IO flush. This is typically unnecessary and will cause a small pause in the game. But forcing a flush does prevent loss of data in the case of a crash. So this is recommended for level save data. </param>
  [MarkForRefactor("Currently Borked", "")]
  public static void Save<T>(T data, string path, bool do_flush = false) where T : class => _defaultDataPath.Save(data, path, do_flush);

  /// <summary>
  /// Loads data from a json file to the serializable class.
  /// </summary>
  /// <typeparam name="T">data type to load. Must be specified when it cannot be inferred.</typeparam>
  /// <param name="path">The path of a json file relative to "user://"</param>
  /// <returns>The data of type T that was loaded from file. </returns>
  [MarkForRefactor("Currently Borked")]
  public static T Load<T>(string path, bool print_errors = true) where T : class => _defaultDataPath.Load<T>(path, print_errors);

  /// <summary>
  /// Change the <see cref="JsonSerializerOptions"/> for the root (non-save slot) SaveData system. This can be used to modify how types are serialized in JSON. Make sure this works for the Squiggles.Core.Data singletons if you want to mess with it.
  /// </summary>
  /// <param name="options"></param>
  public static void SetJsonSettings(JsonSerializerOptions options) => _defaultDataPath.SetJsonSettings(options);

  /// <summary>
  /// Creates a new save slot name using <c>DateTime.Now</c>. Only returns the string, no changes to the SaveData are made.
  /// </summary>
  /// <returns>A string in to form of "YXXX-MXX-DXX-HXX-mXX-SXX-FXXXX" for the current time which is a valid directory name.</returns>
  public static string CreateSaveSlotName() {
    var d = DateTime.Now;
    return $"Y{d.Year}-M{d.Month}-D{d.Day}-H{d.Hour}-m{d.Minute}-S{d.Second}-F{d.Millisecond}";
  }

  /// <summary>
  /// Parses the <see cref="DateTime"/> from a save slot directory name. Useful for determining when the save slot was created and not much else.
  /// </summary>
  /// <param name="slot">The name of the save slot directory</param>
  /// <returns>A <see cref="DateTime"/> set to the time at which the given save slot was created. Assuming it was created with <see cref="CreateSaveSlotName"/></returns>
  public static DateTime ParseSaveSlotName(string slot) {
    var parts = slot.Split('-');

    int year, month, day, hours, minutes, seconds, milli;
    year = month = day = hours = minutes = seconds = milli = 0;

    foreach (var arg in parts) {
      var key = arg[0];
      var value = arg.Substr(1, arg.Length - 1);
      if (!int.TryParse(value, out var intValue)) {
        continue;
      }

      switch (key) {
        case 'Y':
          year = intValue;
          break;
        case 'M':
          month = intValue;
          break;
        case 'D':
          day = intValue;
          break;
        case 'H':
          hours = intValue;
          break;
        case 'm':
          minutes = intValue;
          break;
        case 'S':
          seconds = intValue;
          break;
        case 'F':
          milli = intValue;
          break;
        default:
          break;
      }
    }
    try {
      var parsedDate = new DateTime(year, month, day, hours, minutes, seconds, milli);
      return parsedDate;
    }
    catch (Exception e) {
      Print.Error($"failed to create a date time from string: '{slot}'. Error Message follows \n {e}", typeof(SaveData).FullName);
    }

    return DateTime.MaxValue;
  }

}

/// <summary>
/// A utility class used for handling multiple save slots. Different DataPath instances can have different properties and rules.
/// </summary>
public class DataPath {

  private const string META_DATA_FILE_PATH = "save_slot_meta.json";
  /// <summary>
  /// The path to the current data path. I.e. the root directory to save/load to/from
  /// </summary>
  public string CurrentPath { get; private set; }
  /// <summary>
  /// Settings for JSON Serialization.
  /// </summary>
  public JsonSerializerOptions JsonSettings { get; set; }

  /// <summary>
  /// Any save slot metadata. Helpful for learning more about the save slot in question when presenting multiple slots.
  /// </summary>
  private readonly Dictionary<string, string> _saveSlotMetaData = new();
  /// <summary>
  /// Whether or not this data path allows deletion operations.
  /// </summary>
  private readonly bool _allowDelete;

  /// <summary>
  /// Creates a new DataPath
  /// </summary>
  /// <param name="sub_dir">The path which serves as the root of this data path</param>
  /// <param name="allow_delete">whether or not this data path allows deletion operations.</param>
  public DataPath(string sub_dir, bool allow_delete = true) {
    CurrentPath = sub_dir;
    _allowDelete = allow_delete;
    AddSlotMetaData("slot_name", sub_dir);
    AddSlotMetaData("last_accessed", DateTime.Now.ToString());
    LoadMetaData();
  }

  /// <summary>
  /// Saves data in JSON format to the specified path
  /// </summary>
  /// <typeparam name="T">inferred type of data</typeparam>
  /// <param name="data">the serializable class to save</param>
  /// <param name="path">the path relative to the assigned path (likely a save slot)</param>
  /// <param name="do_flush">Whether or not to force an IO flush. This is typically unnecessary and will cause a small pause in the game. But forcing a flush does prevent loss of data in the case of a crash. So this is recommended for level save data. </param>
  [MarkForRefactor("Currently Borked")]
  public void Save<T>(T data, string path, bool do_flush = false, bool print_errors = true) where T : class {
    if (JsonSettings == null) {
      LoadDefaultJsonSettings();
    }

    try {
      var json_text = JsonSerializer.Serialize(data, JsonSettings);
      SaveText(json_text, path);
    }
    catch (Exception e) {
#if DEBUG
      if (print_errors) {
        Print.Error($"Failed on JSON serialization process for '{data}' type='{data.GetType().FullName}'.\n\tPath={path}\n\tError: {e.Message}", typeof(SaveData).FullName);
      }
#endif
    }
  }
  /// <summary>
  /// Loads data from a json file to the serializable class.
  /// </summary>
  /// <typeparam name="T">data type to load. Must be specified when it cannot be inferred.</typeparam>
  /// <param name="path">The path of a json file relative to the assigned path (likely a save slot)</param>
  /// <returns>The data of type T that was loaded from file. </returns>
  public T Load<T>(string path, bool print_errors = true) where T : class {
    if (JsonSettings == null) {
      LoadDefaultJsonSettings();
    }

    try {
      var json_text = LoadText(path, print_errors);
      if (json_text is null) {
        return null;
      }

      if (json_text.EndsWith("}}")) {
        json_text = json_text.Replace("}}", "}");
      }

      var data = JsonSerializer.Deserialize<T>(json_text, JsonSettings);
      return data;
    }
    catch (Exception e) {
#if DEBUG
      if (print_errors) {
        Print.Error($"Failed on JSON serialization process for type '{typeof(T).FullName}'.\n\tPath={path}\n\tError: {e.Message}", typeof(SaveData).FullName);
      }
#endif
    }
    return null;
  }

  /// <summary>
  /// Saves out data as a string instead of any custom type. Useful in some cases, but generally used internally to handle object serialization
  /// </summary>
  /// <param name="text">the text to save to disk</param>
  /// <param name="path">the file path to save (relative from this data path's root), no '/' prefix needed</param>
  /// <param name="do_flush">Whether or not to force a file flush. If you don't know what that is, leave it false.</param>
  /// <param name="print_errors">Whether or not to print out errors to console when met. Used to handle situations where a file not existing is expected and handled otherwise. Only has an effect in a DEBUG context</param>
  /// </summary>
  public void SaveText(string text, string path, bool do_flush = false, bool print_errors = true) {
    try {
      path = CurrentPath + path;
      // Print.Debug($"Saving Text:\nDir: {path.GetBaseDir()}\nFile: {path.GetFile()}");
      EnsureDirectoryPaths(path);
      { // deletes the original file to ensure no degenerate files are generated
        using var dir = DirAccess.Open(path.GetBaseDir());
        if (dir is not null && dir.FileExists(path.GetFile())) {
          dir.Remove(path.GetFile());
        }
      }
      using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
      if (file is null) {
        Print.Warn($"Failed to open file at path\n\t{path}\n\tError Code #{FileAccess.GetOpenError()}");
        return;
      }

      file.StoreString(text);
      if (do_flush) {
        file.Flush(); // forces a disk write. But also slows down performance
      }

      AddSlotMetaData("last_acessed", DateTime.Now.ToString());
    }
    catch (Exception e) {
#if DEBUG // only allow this toggle in debug enviornments. Always print errors in a release context.
      if (print_errors)
#endif
      {
        Print.Error($"Failed to write data out for 'TEXT' => '{text}'.\n\tPath={path}\n\tError: {e.Message}", typeof(SaveData).FullName);
      }
    }
    // Only when saving a different file, update and save the meta data.
    if (!path.Contains(META_DATA_FILE_PATH)) {
      SaveMetaData();
    }
  }

  /// <summary>
  /// Loads the full text of a file as a single string.
  /// </summary>
  /// <param name="path">the file path to save (relative from this data path's root), no '/' prefix needed</param>
  /// <param name="print_errors">Whether or not to print out errors to console when met. Used to handle situations where a file not existing is expected and handled otherwise. Only has an effect in a DEBUG context</param>
  /// <returns>the full text of the file as a string. Or null if it failed to load the file.</returns>
  public string LoadText(string path, bool print_errors) {

    try {
      path = CurrentPath + path;
      EnsureDirectoryPaths(path);
      using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
      var text = file.GetAsText(true);
      AddSlotMetaData("last_acessed", DateTime.Now.ToString());
      return text;
    }
    catch (Exception e) {
#if DEBUG
      if (print_errors) {
        Print.Error($"Failed to read file.\n\tPath={path}\n\tError: {e.Message}", typeof(SaveData).FullName);
      }
#endif
    }
    return null;
  }

  private static void EnsureDirectoryPaths(string file_path) {

    var globalPath = ProjectSettings.GlobalizePath(file_path.GetBaseDir());
    if (DirAccess.DirExistsAbsolute(globalPath)) {
      return;
    }

    // Print.Debug($"Creating directory path: {globalPath}");
    var err = DirAccess.MakeDirRecursiveAbsolute(globalPath);
    if (err != Error.Ok) {
      Print.Error($"Failed to create folder structure for {globalPath}.\n\tError={err}", typeof(SaveData).FullName);
    }
  }

  private void LoadDefaultJsonSettings()
    => SetJsonSettings(new JsonSerializerOptions() {
      WriteIndented = true,
      AllowTrailingCommas = true,
      IncludeFields = true,
      ReadCommentHandling = JsonCommentHandling.Skip,
      IgnoreReadOnlyFields = true,
      IgnoreReadOnlyProperties = true,
    });

  /// <summary>
  /// Changes the <see cref="JsonSerializerOptions"/> for this given DataPath. It affects the way types are serialized into JSON when using that functionality.
  /// </summary>
  /// <param name="options">the new options to set</param>
  public void SetJsonSettings(JsonSerializerOptions options) => JsonSettings = options;

  /// <summary>
  /// Assigns a key-value for the metadata of this save slot. Useful for setting identifying information for different save slots.
  /// See <see cref="ISaveSlotInformationProvider"/> for information on how save slot names are generated.
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  public void AddSlotMetaData(string key, string value) => _saveSlotMetaData.AddSafe(key, value);

  /// <summary>
  /// Gets the value of a particular metadata for the given key.
  /// </summary>
  /// <param name="key"></param>
  /// <returns>the metadata value, or "" if not found.</returns>
  public string GetMetaData(string key) => !_saveSlotMetaData.ContainsKey(key) ? "" : _saveSlotMetaData[key];

  /// <summary>
  /// Saves the metadata out to disk.
  /// </summary>
  public void SaveMetaData() {
    var list = new List<string>();
    foreach (var entry in _saveSlotMetaData) {
      list.Add($"{entry.Key}={entry.Value}");
    }
    Save(list.ToArray(), META_DATA_FILE_PATH);
  }

  /// <summary>
  ///  Loads metadata from disk.
  /// </summary>
  public void LoadMetaData() {
    var data = Load<string[]>(META_DATA_FILE_PATH, false);
    if (data is null) {
      return;
    }

    foreach (var entry in data) {
      var parts = entry.Split('=', 2);
      if (parts.Length < 2) {
        continue;
      }

      _saveSlotMetaData.AddSafe(parts[0], parts[1]);
    }
  }

  /// <summary>
  /// Deletes this save slot and all data within it. Only allowed if <see cref="_allowDelete"/> is true. This rudimentarily prevents deleting certain save slots (i.e. the root slot) but is not a hard limit because we're programmers. If there's a will and a page on Stack Overflow, life will find a way.
  /// </summary>
  public void DeleteSaveSlot() {
    if (!_allowDelete) {
      return;
    }

    var err = OS.MoveToTrash(ProjectSettings.GlobalizePath(CurrentPath));
    if (err != Error.Ok) {
      Print.Error($"Failed to delete save slot! Error: {err}", typeof(SaveData).FullName);
    }
    else {
      Print.Debug($"Successfully erased save slot: {CurrentPath}");
    }
  }

}
