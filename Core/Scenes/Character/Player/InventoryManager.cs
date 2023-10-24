namespace Squiggles.Core.Scenes.Character;

using System;
using Godot;
using Squiggles.Core.Attributes;
using Squiggles.Core.Data;

/// <summary>
/// A manager component to storing a character's inventory
/// </summary>
[MarkForRefactor("Removing Vestigial Classes", "This is used in CuteFarmSim but has fewer general purpose applications.")]
public partial class InventoryManager : Node {

  /// <summary>
  /// A signal for handling when a slot in the inventory is updated.
  /// </summary>
  /// <param name="index">the slot index</param>
  /// <param name="item">the item id</param>
  /// <param name="qty">the new current quantity of the item</param>
  [Signal] public delegate void SlotUpdateEventHandler(int index, string item, int qty);
  /// <summary>
  /// A signal for handling when an inventory slot is selected
  /// </summary>
  /// <param name="index">the index of the slot</param>
  [Signal] public delegate void SlotSelectEventHandler(int index);
  /// <summary>
  /// The maximum allowed number of items per slot.
  /// </summary>
  public int MaxItemsPerSlot = 10;

  /// <summary>
  /// The currently selected index
  /// </summary>
  public int Selected { get; private set; }

  private Slot[] _inventorySlots = Array.Empty<Slot>();

  /// <summary>
  /// Resizes the inventory to the new number of slots
  /// </summary>
  /// <param name="slots">the number of slots to hold</param>
  public void ResizeInventory(int slots) {
    Array.Resize(ref _inventorySlots, slots);
    ClearEmpty();
  }

  /// <summary>
  /// Tries to add an item to the inventory, handling failure
  /// </summary>
  /// <param name="item">the item id to add</param>
  /// <param name="qty">the quantity to add</param>
  /// <returns>true if the item was able to be added, false if not.</returns>
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

  /// <summary>
  /// Attempts to remove the currently selected item.
  /// </summary>
  /// <returns>true if successful in removing selected item. False if not</returns>
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

  /// <returns>The item ID of the currently selected item slot, or "" if invalid</returns>
  public string GetSelectedItem() => _inventorySlots[Selected]?.Item ?? "";

  /// <summary>
  /// Assign the new selection index
  /// </summary>
  /// <param name="select">the new index to select</param>
  public void SetSelection(int select) {
    if (select < 0 || select >= _inventorySlots.Length) {
      return;
    }

    Selected = select;
    EmitSignal(nameof(SlotSelect), Selected);
  }


  /// <summary>
  /// Selects the "next" slot, wrapping at the extents of the inventory.
  /// </summary>
  public void SelectNext() {
    Selected = Mathf.PosMod(Selected + 1, _inventorySlots.Length);
    EmitSignal(nameof(SlotSelect), Selected);
  }
  /// <summary>
  /// Selects the "previous" slot, wrapping at the extents of the inventory
  /// </summary>
  public void SelectPrevious() {
    Selected = Mathf.PosMod(Selected - 1, _inventorySlots.Length);

    EmitSignal(nameof(SlotSelect), Selected);
  }

  /// <summary>
  /// Clears out all items that are now null, have an invalid ID, or have a quantity less than or equal to zero.
  /// </summary>
  private void ClearEmpty()
    => DoForSlots((index, item, qty) => {
      if (item == "" || qty <= 0) {
        _inventorySlots[index] = null;
        EmitSignal(nameof(SlotUpdate), index, "", 0);
      }
    });

  /// <summary>
  /// Saves the inventory data out to a <see cref="SaveDataBuilder"/>.
  /// </summary>
  /// <param name="build">the save data builder to reference</param>
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

  /// <summary>
  /// Loads the inventory data from the specified <see cref="SaveDataBuilder"/>
  /// </summary>
  /// <param name="build">the save data builder to reference</param>
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

  /// <summary>
  /// Triggers an update GUI request for ALL slots
  /// </summary>
  /// <param name="triggerUpdatePlayeInventoryDisplay"></param>
  public void UpdateGUICall(Action<int, string, int> triggerUpdatePlayeInventoryDisplay)
    => DoForSlots((index, item, qty)
      => triggerUpdatePlayeInventoryDisplay(index, item, qty));

  /// <summary>
  /// A delegate used for handling processes that must be done on a per slot basis with no regard for other slots.
  /// </summary>
  /// <param name="index">current slot index</param>
  /// <param name="item">the item id of the current slot, "" if no item found</param>
  /// <param name="quantity">the current quantity of the slot, 0 if no item found</param>
  public delegate void SlotDataAction(int index, string item, int quantity);

  /// <summary>
  /// Performs a <see cref="SlotDataAction"/> for each slot in this inventory.
  /// </summary>
  /// <param name="action"></param>
  public void DoForSlots(SlotDataAction action) {
    for (var i = 0; i < _inventorySlots.Length; i++) {
      var slot = _inventorySlots[i];
      action(i, slot?.Item ?? "", slot?.Qty ?? 0);
    }
  }


  /// <summary>
  /// A class for storing the data of an individual slot.
  /// </summary>
  /// <remarks>
  /// </remarks>
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


