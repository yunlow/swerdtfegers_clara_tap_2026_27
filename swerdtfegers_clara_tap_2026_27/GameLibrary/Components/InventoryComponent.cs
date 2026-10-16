using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Data;
using GameLibrary.Data;
using System;
using System.Collections.Generic;

namespace GameLibrary.Components
{
    public partial class InventoryComponent : Component
    {
        private readonly List<InventoryEntry> _inventoryEntryTable = new List<InventoryEntry>();

        public InventoryComponent(GameObject game_object) : base(game_object)
        {

        }

        public int GetEntryCount()
        {
            return _inventoryEntryTable.Count;
        }

        public void Add(ItemData item_data, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero");
            }

            InventoryEntry inventory_entry = GetEntry(item_data._id);

            if (inventory_entry == null)
            {
                inventory_entry = new InventoryEntry(item_data, 0);
                _inventoryEntryTable.Add(inventory_entry);
            }

            inventory_entry.SetQuantity(inventory_entry.GetQuantity() + quantity);
        }

        public InventoryEntry GetEntryAtIndex(int entry_index)
        {
            return _inventoryEntryTable[entry_index];
        }

        public int GetQuantity(string entry_reference)
        {
            int quantity = 0;
            InventoryEntry inventory_entry = GetEntry(entry_reference);

            if (inventory_entry != null)
            {
                quantity = inventory_entry.GetQuantity();
            }

            return quantity;
        }

        public InventoryEntry GetEntry(string entry_reference)
        {
            InventoryEntry inventory_entry = null;

            for (int item_index = 0; item_index < _inventoryEntryTable.Count; item_index++)
            {
                if (_inventoryEntryTable[item_index].GetItem()._id == entry_reference)
                {
                    inventory_entry = _inventoryEntryTable[item_index];
                }
            }

            return inventory_entry;
        }

        public bool TryRemove(string entry_reference, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero");
            }

            bool does_remove_succeed = false;
            InventoryEntry inventory_entry = GetEntry(entry_reference);

            if ((inventory_entry != null) && (inventory_entry.GetQuantity() >= quantity))
            {
                inventory_entry.SetQuantity(Math.Max(0, inventory_entry.GetQuantity() - quantity));
                does_remove_succeed = true;
            }

            return does_remove_succeed;
        }
    }
}