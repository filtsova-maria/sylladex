﻿using Microsoft.Xna.Framework;
using Sylladex.Entities;
using System.Diagnostics;

namespace Sylladex.FetchModi
{
    /// <summary>
    /// Represents the Queue modus. Items are enqueued into the available slots, pushing the rest to the right, and only the last card can be accessed (=first item inserted, FIFO).
    /// </summary>
    public class QueueSylladex : SylladexModus
    {
        public static string GetName() => "Queue";
        public static Color GetColor() => Color.Orange;
        public override bool[] SlotEnabledMask { get; }
        public override string Name => GetName();
        public QueueSylladex(ref Item?[] items) : base(ref items)
        {
            Tint = GetColor();
            SlotEnabledMask = new bool[_items.Length];
            for (int i = 0; i < SlotEnabledMask.Length - 1; i++)
            {
                SlotEnabledMask[i] = false;
            }
            SlotEnabledMask[^1] = true;
        }

        public override void InsertItem(Item item)
        {
            Item? propagatedItem = null;
            for (int i = 0; i < _items.Length - 1; i++)
            {
                Item? cur = _items[i];
                Item? next = _items[i + 1];
                // If the current slot is empty, fill it
                if (cur is null)
                {
                    MoveToInventory(item, i);
                    Debug.WriteLine($"Inserted item: {item.Name} to index {i}");
                    return;
                }
                // If the current slot is full and the next slot is empty, push the current item to the right and store the new item in its place
                else if (next is null)
                {
                    if (propagatedItem is null)
                    {
                        _items[i + 1] = cur; // push the current item to the right
                        MoveToInventory(item, i);
                        Debug.WriteLine($"Inserted item: {item.Name} to index {i}, pushed {cur.Name} to the right");
                        return;
                    }
                    _items[i + 1] = propagatedItem; // store pushed item to the next available slot
                    return;
                }
                // If the current slot is full and the next slot is full, store the current item and push the rest to the right
                else
                {
                    if (propagatedItem is null)
                    {
                        MoveToInventory(item, i);
                        _items[i + 1] = cur;
                        Debug.WriteLine($"Inserted item: {item.Name} to index {i}");
                    }
                    else
                    {
                        _items[i + 1] = propagatedItem;
                    }
                    propagatedItem = next;
                    Debug.WriteLine($"Propagating {propagatedItem.Name} to the right");
                }
            }
            if (propagatedItem is not null)
            {
                Debug.WriteLine($"Inventory full, ejecting {propagatedItem.Name}");
                EjectFromInventory(propagatedItem, null);
            }
        }
        public override void FetchItem(Item item)
        {
            // Queue modus always fetches the last inventory slot
            Item? itemToFetch = _items[^1];
            if (itemToFetch is null)
            {
                Debug.WriteLine($"Failed to fetch item: {item.Name}");
                return;
            }
            _items[^1] = null;
            EjectFromInventory(itemToFetch, _items.Length - 1);
        }
    }
}