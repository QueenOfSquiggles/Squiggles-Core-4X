using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using queen.error;
using queen.extension;

namespace queen.data;

public static class Data
{
    private const string SAVE_SLOT_ROOT = "user://save_slot/";
    private static DataPath DEFAULT_DATA_PATH = new DataPath("user://", false); // disallow deletion just in case bad code

    /// <summary>
    /// An alternate save path pointing towards the current save slot. This can be altered with SetSaveSlot.
    /// See also: <seealso cref="SetSaveSlot"/>
    /// </summary>
    public static DataPath CurrentSaveSlot { get; private set; } = new(SAVE_SLOT_ROOT + "default/");

    /// <summary>
    /// Checks the root directory of save slots and finds all save slot directories. Meta-data should be collected afterwards.
    /// </summary>
    /// <returns>A string array of save slots, sorted alphabetically </returns>
    public static string[] GetKnownSaveSlots()
    {
        using var dir = DirAccess.Open(SAVE_SLOT_ROOT);
        if (dir == null) return Array.Empty<string>();
        dir.IncludeHidden = false;
        dir.IncludeNavigational = false;
        return dir.GetDirectories();
    }

    /// <summary>
    /// Sets CurrentSaveSlot to point at a new save slot directory. This will force new files saved to save in that new directory.
    /// See also: <seealso cref="CurrentSaveSlot"/>
    /// </summary>
    /// <param name="slot_name">The name of the file slot. Must be valid for a directory name on the host system. </param>
    public static void SetSaveSlot(string slot_name)
    {
        if (!slot_name.IsValidFileName())
        {
            Print.Error($"'{slot_name}' is not a valid file name. Make sure save slots are being saved using a name that is valid on this system!");
            return;
        }
        var path = $"{SAVE_SLOT_ROOT}{slot_name}";
        if (!path.EndsWith('/')) path += '/';
        CurrentSaveSlot = new DataPath(path);
    }

    public static void LoadDefaultSaveSlot()
    {
        SetSaveSlot("default");
    }

    public static bool HasSaveData()
    {
        return GetKnownSaveSlots().Length > 1;
    }

    public static void LoadMostRecentSaveSlot()
    {
        var slots = GetKnownSaveSlots();
        string most_recent = "default"; // default to default slot just in case no valid extra ones are found
        long recent_time = 0;
        foreach (var slot in slots)
        {
            if (slot == "default") continue;
            var date = ParseSaveSlotName(slot);
            var time = date.ToFileTimeUtc();
            if (time <= recent_time) continue;
            most_recent = slot;
            recent_time = time;
        };
        if (most_recent == "") return;
        SetSaveSlot(most_recent);
    }

    /// <summary>
    /// Saves data in JSON format to the specified path, relative to the root of "user://"
    /// </summary>
    /// <typeparam name="T">inferred type of data</typeparam>
    /// <param name="data">the serializable class to save</param>
    /// <param name="path">the path relative to "user://"</param>
    /// <param name="do_flush">Whether or not to force an IO flush. This is typically unnecessary and will cause a small pause in the game. But forcing a flush does prevent loss of data in the case of a crash. So this is recommended for level save data. </param>
    public static void Save<T>(T data, string path, bool do_flush = false) where T : class => DEFAULT_DATA_PATH.Save<T>(data, path, do_flush);

    /// <summary>
    /// Loads data from a json file to the serializable class.
    /// </summary>
    /// <typeparam name="T">data type to load. Must be specified when it cannot be inferred.</typeparam>
    /// <param name="path">The path of a json file relative to "user://"</param>
    /// <returns>The data of type T that was loaded from file. </returns>
    public static T? Load<T>(string path, bool print_errors = true) where T : class => DEFAULT_DATA_PATH.Load<T>(path, print_errors);

    public static void SetJsonSettings(JsonSerializerOptions options) => DEFAULT_DATA_PATH.SetJsonSettings(options);

    public static string CreateSaveSlotName()
    {
        var d = DateTime.Now;
        return $"Y{d.Year}-M{d.Month}-D{d.Day}-H{d.Hour}-m{d.Minute}-S{d.Second}-F{d.Millisecond}";
    }

    public static DateTime ParseSaveSlotName(string slot)
    {
        var parts = slot.Split('-');

        int year, month, day, hours, minutes, seconds, milli;
        year = month = day = hours = minutes = seconds = milli = 0;

        foreach (var arg in parts)
        {
            var key = arg[0];
            var value = arg.Substr(1, arg.Length - 1);
            if (!int.TryParse(value, out int intValue)) continue;

            switch (key)
            {
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
            }
        }
        try
        {
            var parsedDate = new DateTime(year, month, day, hours, minutes, seconds, milli);
            return parsedDate;
        }
        catch (Exception e)
        {
            Print.Error($"failed to create a date time from string: '{slot}'. Error Message follows \n {e}");
        }

        return DateTime.MaxValue;
    }

}

public class DataPath
{

    private const string META_DATA_FILE_PATH = "save_slot_meta.json";
    public string CurrentPath { get; private set; }
    public JsonSerializerOptions JsonSettings { get; set; } = null;

    private Dictionary<string, string> SaveSlotMetaData = new();
    private readonly bool AllowDelete;

    public DataPath(string sub_dir, bool allow_delete = true)
    {
        CurrentPath = sub_dir;
        AllowDelete = allow_delete;
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
    public void Save<T>(T data, string path, bool do_flush = false, bool print_errors = true) where T : class
    {
        if (JsonSettings == null) LoadDefaultJsonSettings();
        try
        {
            var json_text = JsonSerializer.Serialize(data, JsonSettings);
            SaveText(json_text, path);
        }
        catch (Exception e)
        {
#if DEBUG
            if (print_errors) Print.Error($"Failed on JSON serialization process for '{data}' type='{data.GetType().FullName}'.\n\tPath={path}\n\tError: {e.Message}");
#endif
        }
    }
    /// <summary>
    /// Loads data from a json file to the serializable class.
    /// </summary>
    /// <typeparam name="T">data type to load. Must be specified when it cannot be inferred.</typeparam>
    /// <param name="path">The path of a json file relative to the assigned path (likely a save slot)</param>
    /// <returns>The data of type T that was loaded from file. </returns>
    public T? Load<T>(string path, bool print_errors = true) where T : class
    {
        if (JsonSettings == null) LoadDefaultJsonSettings();
        try
        {
            var json_text = LoadText(path, print_errors);
            if (json_text is null) return null;
            if (json_text.EndsWith("}}")) json_text = json_text.Replace("}}", "}");
            var data = JsonSerializer.Deserialize<T>(json_text, JsonSettings);
            return data;
        }
        catch (Exception e)
        {
#if DEBUG
            if (print_errors) Print.Error($"Failed on JSON serialization process for type '{typeof(T).FullName}'.\n\tPath={path}\n\tError: {e.Message}");
#endif
        }
        return null;
    }

    public void SaveText(string text, string path, bool do_flush = false, bool print_errors = true)
    {
        try
        {
            path = CurrentPath + path;
            // Print.Debug($"Saving Text:\nDir: {path.GetBaseDir()}\nFile: {path.GetFile()}");
            EnsureDirectoryPaths(path);
            { // deletes the original file to ensure no degenerate files are generated
                using var dir = DirAccess.Open(path.GetBaseDir());
                if (dir is not null && dir.FileExists(path.GetFile())) dir.Remove(path.GetFile());
            }
            using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
            if (file is null)
            {
                Print.Warn($"Failed to open file at path\n\t{path}\n\tError Code #{FileAccess.GetOpenError()}");
                return;
            }

            file.StoreString(text);
            if (do_flush) file.Flush(); // forces a disk write. But also slows down performance
            AddSlotMetaData("last_acessed", DateTime.Now.ToString());
        }
        catch (Exception e)
        {
#if DEBUG
            if (print_errors) Print.Error($"Failed to write data out for 'TEXT' => '{text}'.\n\tPath={path}\n\tError: {e.Message}");
#endif
        }
        // Only when saving a different file, save the meta data.
        if (!path.Contains(META_DATA_FILE_PATH)) SaveMetaData();
    }

    public string? LoadText(string path, bool print_errors)
    {

        try
        {
            path = CurrentPath + path;
            EnsureDirectoryPaths(path);
            using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            var text = file.GetAsText(true);
            AddSlotMetaData("last_acessed", DateTime.Now.ToString());
            return text;
        }
        catch (Exception e)
        {
#if DEBUG
            if (print_errors) Print.Error($"Failed to read file.\n\tPath={path}\n\tError: {e.Message}");
#endif
        }
        return null;
    }

    private void EnsureDirectoryPaths(string file_path)
    {

        var globalPath = ProjectSettings.GlobalizePath(file_path.GetBaseDir());
        if (DirAccess.DirExistsAbsolute(globalPath)) return;

        // Print.Debug($"Creating directory path: {globalPath}");
        var err = DirAccess.MakeDirRecursiveAbsolute(globalPath);
        if (err != Error.Ok)
            Print.Error($"Failed to create folder structure for {globalPath}.\n\tError={err}");
    }

    private void LoadDefaultJsonSettings()
    {
        SetJsonSettings(new JsonSerializerOptions()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            IncludeFields = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
        });
    }

    public void SetJsonSettings(JsonSerializerOptions options) => JsonSettings = options;

    public void AddSlotMetaData(string key, string value)
    {
        SaveSlotMetaData.AddSafe(key, value);
    }

    public string GetMetaData(string key)
    {
        if (!SaveSlotMetaData.ContainsKey(key)) return "";
        return SaveSlotMetaData[key];
    }

    public void SaveMetaData()
    {
        var list = new List<string>();
        foreach (var entry in SaveSlotMetaData)
        {
            list.Add($"{entry.Key}={entry.Value}");
        }
        Save(list.ToArray(), META_DATA_FILE_PATH);
    }

    public void LoadMetaData()
    {
        var data = Load<string[]>(META_DATA_FILE_PATH, false);
        if (data is null) return;
        foreach (var entry in data)
        {
            var parts = entry.Split('=', 2);
            if (parts.Length < 2) continue;
            SaveSlotMetaData.AddSafe(parts[0], parts[1]);
        }
    }

    public void DeleteSaveSlot()
    {
        if (!AllowDelete) return;
        var err = OS.MoveToTrash(ProjectSettings.GlobalizePath(CurrentPath));
        if (err != Error.Ok)
        {
            Print.Error($"Failed to delete save slot! Error: {err}");
        }
    }

}