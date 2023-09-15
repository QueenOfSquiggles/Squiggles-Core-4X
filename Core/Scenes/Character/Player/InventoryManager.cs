namespace Squiggles.Core.Scenes.Character;

using System;
using Godot;
using Squiggles.Core.Data;

public partial class InventoryManager : Node {

  [Signal] public delegate void SlotUpdateEventHandler(int index, string item, int qty);
  [Signal] public delegate void SlotSelectEventHandler(int index);
  public int MaxItemsPerSlot = 10;
  public int Selected { get; private set; }
  private Slot[] _inventorySlots = Array.Empty<Slot>();

  public void ResizeInventory(int slots) {
    Array.Resize(ref _inventorySlots, slots);
    ClearEmpty();
  }

  public bool TryAddItem(string item, int qty = 1) {
    if (item == "" || qty <= 0 || qty > MaxItemsPerSlot) {
      return false;
    }

    Slot slot;
    var remaining = qty;

    // Try add to selected
    slot = _inventorySlots[Selected];
    if (slot is null) {
      _inventorySlots[Selected] = new Slot(item, qty);
      EmitSignal(nameof(SlotUpdate), Selected, item, qty);
      return true;
    }
    else if (slot.Item == item) {
      var space = MaxItemsPerSlot - slot.Qty;
      if (space > remaining) {
        space = remaining;
      }

      if (space > 0) {
        slot.Qty += space;
        remaining -= space;

        if (remaining <= 0) {
          EmitSignal(nameof(SlotUpdate), Selected, item, slot.Qty);
          return true;
        }
      }
    }
    // Add to existing
    for (var i = 0; i < _inventorySlots.Length; i++) {
      slot = _inventorySlots[i];
      if (slot is null) {
        continue;
      }

      if (slot.Item != item) {
        continue;
      }

      var space = MaxItemsPerSlot - slot.Qty;
      if (space < 0) {
        continue;
      }

      if (space > remaining) {
        space = remaining;
      }

      slot.Qty += space;
      remaining -= space;

      if (remaining > 0) {
        continue;
      }

      EmitSignal(nameof(SlotUpdate), i, item, slot.Qty);
      return true;
    }

    // Try add to new slot
    for (var i = 0; i < _inventorySlots.Length; i++) {
      if (_inventorySlots[i] != null) {
        continue;
      }

      _inventorySlots[i] = new Slot(item, qty);
      EmitSignal(nameof(SlotUpdate), i, item, qty);
      return true;
    }
    // No slots available. Fail
    return false;
  }

  public bool RemoveItem() {
    var slot = _inventorySlots[Selected];
    if (slot is null || slot.Qty <= 0) {
      return false;
    }

    slot.Qty -= 1;
    if (slot.Qty <= 0) {
      slot.Item = "";
    }

    EmitSignal(nameof(SlotUpdate), Selected, slot.Item, slot.Qty);
    ClearEmpty();
    return true;
  }

  public string GetSelectedItem() => _inventorySlots[Selected]?.Item ?? "";

  public void SetSelection(int select) {
    if (select < 0 || select >= _inventorySlots.Length) {
      return;
    }

    Selected = select;
    EmitSignal(nameof(SlotSelect), Selected);
  }


  public void SelectNext() {
    Selected = Mathf.PosMod(Selected + 1, _inventorySlots.Length);
    EmitSignal(nameof(SlotSelect), Selected);
  }
  public void SelectPrevious() {
    Selected = Mathf.PosMod(Selected - 1, _inventorySlots.Length);

    EmitSignal(nameof(SlotSelect), Selected);
  }

  private void ClearEmpty()
    => DoForSlots((index, item, qty) => {
      if (item == "" || qty <= 0) {
        _inventorySlots[index] = null;
        EmitSignal(nameof(SlotUpdate), index, "", 0);
      }
    });

  public void SaveToData(ref SaveDataBuilder build) {
    build.PutInt("Inv_SlotCount", _inventorySlots.Length);
    build.PutInt("Inv_Select", Selected);
    build.PutInt("Inv_MaxCount", MaxItemsPerSlot);
    for (var i = 0; i < _inventorySlots.Length; i++) {
      var slot = _inventorySlots[i];
      var item = slot is null ? "" : slot.Item;
      var qty = slot is null ? 0 : slot.Qty;
      build.PutString($"Inv_Slot{i}", $"{item}::{qty}");
    }
  }

  public void LoadFromData(SaveDataBuilder build) {

    if (!build.GetInt("Inv_SlotCount", out var slots)) {
      return;
    }

    if (!build.GetInt("Inv_Select", out var sel)) {
      return;
    }

    _inventorySlots = new Slot[slots];
    Selected = sel;

    MaxItemsPerSlot = build.GetInt("Inv_MaxCount");
    for (var i = 0; i < slots; i++) {
      if (!build.GetString($"Inv_Slot{i}", out var s) || s == "") {
        continue;
      }

      var parts = s.Split("::");
      if (parts.Length != 2) {
        continue;
      }

      var item = parts[0];
      var qty = int.Parse(parts[1]);
      _inventorySlots[i] = item == "" ? null : new Slot(item, qty);
      EmitSignal(nameof(SlotUpdate), i, item, qty);
    }
    EmitSignal(nameof(SlotSelect), Selected);
  }

  public void UpdateGUICall(Action<int, string, int> triggerUpdatePlayeInventoryDisplay)
    => DoForSlots((index, item, qty)
      => triggerUpdatePlayeInventoryDisplay(index, item, qty));

  private delegate void SlotDataAction(int index, string item, int quantity);
  private void DoForSlots(SlotDataAction action) {
    for (var i = 0; i < _inventorySlots.Length; i++) {
      var slot = _inventorySlots[i];
      action(i, slot is null ? "" : slot.Item, slot is null ? 0 : slot.Qty);
    }
  }



  public class Slot {
    public string Item = "";
    public int Qty;

    public Slot(string item, int qty) {
      Item = item;
      Qty = qty;
    }

    public override string ToString() => $"({Item})" + (Qty > 1 ? $" x{Qty}" : "");
  }
}


