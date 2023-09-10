using System;
using System.Collections.Generic;
using Godot;
using queen.data;
using queen.error;
using queen.events;

public partial class InventoryManager : Node
{

    [Signal] public delegate void SlotUpdateEventHandler(int index, string item, int qty);
    [Signal] public delegate void SlotSelectEventHandler(int index);
    public int MaxItemsPerSlot = 10;
    public int Selected { get => _Selected; }
    private Slot[] _InventorySlots = Array.Empty<Slot>();
    private int _Selected = 0;

    public void ResizeInventory(int slots)
    {
        Array.Resize(ref _InventorySlots, slots);
        ClearEmpty();
    }

    public bool TryAddItem(string item, int qty = 1)
    {
        if (item == "" || qty <= 0 || qty > MaxItemsPerSlot) return false;
        Slot slot;
        int remaining = qty;

        // Try add to selected
        slot = _InventorySlots[_Selected];
        if (slot is null)
        {
            _InventorySlots[_Selected] = new Slot(item, qty);
            EmitSignal(nameof(SlotUpdate), _Selected, item, qty);
            return true;
        }
        else if (slot.Item == item)
        {
            int space = MaxItemsPerSlot - slot.Qty;
            if (space > remaining) space = remaining;
            if (space > 0)
            {
                slot.Qty += space;
                remaining -= space;

                if (remaining <= 0)
                {
                    EmitSignal(nameof(SlotUpdate), _Selected, item, slot.Qty);
                    return true;
                }
            }
        }
        // Add to existing
        for (int i = 0; i < _InventorySlots.Length; i++)
        {
            slot = _InventorySlots[i];
            if (slot is null) continue;
            if (slot.Item != item) continue;
            int space = MaxItemsPerSlot - slot.Qty;
            if (space < 0) continue;
            if (space > remaining) space = remaining;
            slot.Qty += space;
            remaining -= space;

            if (remaining > 0) continue;
            EmitSignal(nameof(SlotUpdate), i, item, slot.Qty);
            return true;
        }

        // Try add to new slot
        for (int i = 0; i < _InventorySlots.Length; i++)
        {
            if (_InventorySlots[i] != null) continue;
            _InventorySlots[i] = new Slot(item, qty);
            EmitSignal(nameof(SlotUpdate), i, item, qty);
            return true;
        }
        // No slots available. Fail
        return false;
    }

    public bool RemoveItem()
    {
        var slot = _InventorySlots[_Selected];
        if (slot is null || slot.Qty <= 0) return false;
        slot.Qty -= 1;
        if (slot.Qty <= 0) slot.Item = "";
        EmitSignal(nameof(SlotUpdate), _Selected, slot.Item, slot.Qty);
        ClearEmpty();
        return true;
    }

    public string GetSelectedItem()
    {
        return _InventorySlots[_Selected]?.Item;
    }

    public void SetSelection(int select)
    {
        if (select < 0 || select >= _InventorySlots.Length) return;
        _Selected = select;
        EmitSignal(nameof(SlotSelect), _Selected);
    }


    public void SelectNext()
    {
        _Selected = Mathf.PosMod(_Selected + 1, _InventorySlots.Length);
        EmitSignal(nameof(SlotSelect), _Selected);
    }
    public void SelectPrevious()
    {
        _Selected = Mathf.PosMod(_Selected - 1, _InventorySlots.Length);

        EmitSignal(nameof(SlotSelect), _Selected);
    }

    private void ClearEmpty()
    {
        DoForSlots((index, item, qty) =>
        {
            if (item == "" || qty <= 0)
            {
                _InventorySlots[index] = null;
                EmitSignal(nameof(SlotUpdate), index, "", 0);
            }
        });
    }

    public void SaveToData(ref SaveDataBuilder build)
    {
        build.PutInt("Inv_SlotCount", _InventorySlots.Length);
        build.PutInt("Inv_Select", _Selected);
        build.PutInt("Inv_MaxCount", MaxItemsPerSlot);
        for (int i = 0; i < _InventorySlots.Length; i++)
        {
            var slot = _InventorySlots[i];
            string item = slot is null ? "" : slot.Item;
            int qty = slot is null ? 0 : slot.Qty;
            build.PutString($"Inv_Slot{i}", $"{item}::{qty}");
        }
    }

    public void LoadFromData(SaveDataBuilder build)
    {

        if (!build.GetInt("Inv_SlotCount", out int slots)) return;
        if (!build.GetInt("Inv_Select", out int sel)) return;
        _InventorySlots = new Slot[slots];
        _Selected = sel;

        MaxItemsPerSlot = build.GetInt("Inv_MaxCount");
        for (int i = 0; i < slots; i++)
        {
            if (!build.GetString($"Inv_Slot{i}", out string s) || s == "") continue;
            var parts = s.Split("::");
            if (parts.Length != 2) continue;
            string item = parts[0];
            int qty = int.Parse(parts[1]);
            _InventorySlots[i] = item == "" ? null : new Slot(item, qty);
            EmitSignal(nameof(SlotUpdate), i, item, qty);
        }
        EmitSignal(nameof(SlotSelect), _Selected);
    }

    public void UpdateGUICall(Action<int, string, int> triggerUpdatePlayeInventoryDisplay)
    {
        DoForSlots((index, item, qty) => triggerUpdatePlayeInventoryDisplay(index, item, qty));
    }

    private delegate void SlotDataAction(int index, string item, int quantity);
    private void DoForSlots(SlotDataAction action)
    {
        for (int i = 0; i < _InventorySlots.Length; i++)
        {
            var slot = _InventorySlots[i];
            action(i, slot is null ? "" : slot.Item, slot is null ? 0 : slot.Qty);
        }
    }



    public class Slot
    {
        public string Item = "";
        public int Qty = 0;

        public Slot(string item, int qty)
        {
            Item = item;
            Qty = qty;
        }

        public override string ToString()
        {
            return $"({Item})" + (Qty > 1 ? $" x{Qty}" : "");
        }
    }
}


