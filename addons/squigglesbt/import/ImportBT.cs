namespace SquigglesBT;
using System;
using Godot;
using Godot.Collections;

[Tool]
public partial class ImportBT : EditorImportPlugin {
  private enum Presets {
    DEFAULT
  }

  public override Error _Import(string sourceFile, string savePath, Dictionary options, Array<string> platformVariants, Array<string> genFiles) {
    using var file = FileAccess.Open(sourceFile, FileAccess.ModeFlags.Read);
    if (file is null) { GD.Print($"Failed to open file : {sourceFile}"); return FileAccess.GetOpenError(); }
    var text = file.GetAsText();
    var data = Json.ParseString(text).AsGodotDictionary();

    if (data is null || data.Count <= 0) { GD.Print($"No valid data found in file: \n{text}"); return Error.InvalidData; }

    // actually load the nodes
    var res = new BehaviourTree {
      Name = options["OverrideName"].AsString() ?? savePath.GetFile().Split('.', 2)[0],
      JSONData = data,
    };
    res.RebuildTree();
    return ResourceSaver.Save(res, $"{savePath}.{_GetSaveExtension()}");
  }



  public override Array<Dictionary> _GetImportOptions(string path, int presetIndex)
    => presetIndex switch {
      0 => new(new[] {
                Param("OverrideName", ""),
            }),
      _ => throw new NotImplementedException()
    };

  private static Dictionary Param(string label, Variant value, PropertyHint? hint = null, string hint_string = "") {
    var dict = new Dictionary {
      ["name"] = label,
      ["default_value"] = value
    };
    if (hint is null) {
      return dict;
    }
    dict["property_hint"] = (int)hint;
    dict["hint_string"] = hint_string;

    return dict;
  }


  // Metadata

  public override string _GetImporterName() => "squiggles.behaviourtree";
  public override string _GetVisibleName() => "Behaviour Tree";
  public override string[] _GetRecognizedExtensions() => new string[] { "json" };
  public override string _GetSaveExtension() => "tres";
  public override string _GetResourceType() => "BehaviourTree";
  public override int _GetPresetCount() => 1;
  public override string _GetPresetName(int presetIndex) => presetIndex switch {
    0 => Presets.DEFAULT.ToString(),
    _ => "Unrecognized Preset"
  };
  public override bool _GetOptionVisibility(string path, StringName optionName, Dictionary options) => true;
  public override float _GetPriority() => 1.0f; // lower priority so other JSON files aren't imported as BT by default
  public override int _GetImportOrder() => 0;


}
