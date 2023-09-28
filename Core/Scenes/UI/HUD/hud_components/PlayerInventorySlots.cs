namespace Squiggles.Core.Scenes.UI.HUD;

using Godot;
using Squiggles.Core.Events;

public partial class PlayerInventorySlots : HBoxContainer {


  [Export] private PackedScene _inventorySlotPacked;

  private int _previousSelectSlot;

  public override void _Ready() {
    EventBus.GUI.UpdatePlayerInventoryDisplay += OnInventorySlotUpdate;
    EventBus.GUI.UpdatePlayerInventoryDisplay += OnInventorySlotUpdate;
    EventBus.GUI.PlayerInventorySelectIndex += OnInventorySelect;
    EventBus.GUI.PlayerInventorySizeChange += EnsureInventorySlots;
  }

  public override void _ExitTree() {
    EventBus.GUI.UpdatePlayerInventoryDisplay -= OnInventorySlotUpdate;
    EventBus.GUI.UpdatePlayerInventoryDisplay -= OnInventorySlotUpdate;
    EventBus.GUI.PlayerInventorySelectIndex -= OnInventorySelect;
    EventBus.GUI.PlayerInventorySizeChange -= EnsureInventorySlots;
  }

  private void OnInventorySlotUpdate(int index, string item, int qty) {
    if (GetChildCount() <= index || index < 0) {
      return;
    }
    (GetChild(index) as ItemSlotDisplay)?.UpdateItem(item, qty);
  }

  private void OnInventorySelect(int index) {
    if (GetChildCount() <= index || index < 0) {
      return;
    }
    (GetChild(_previousSelectSlot) as ItemSlotDisplay)?.OnDeselect();
    (GetChild(index) as ItemSlotDisplay)?.OnSelect();
    _previousSelectSlot = index;
  }

  private void EnsureInventorySlots(int index) {
    while (index > GetChildCount()) {
      var slot = _inventorySlotPacked.Instantiate();
      AddChild(slot);
    }
  }
}
