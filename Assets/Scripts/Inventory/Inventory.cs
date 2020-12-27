using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

namespace InventoryManagement
{
	[Serializable] public class ItemDictionary : SerializableDictionary<CardObject, int> { }

	[Serializable]
	public class Inventory
	{
		public enum EquipmentSlot { Head, Gloves, Implant, Shield, Body, Weapon1, Weapon2 }

		//a collection of all the character's _items, stored by prefab name and quantity
		[SerializeField] private ItemDictionary _items = new ItemDictionary();

		public int Currency { get; set; }
		public bool IsEmpty { get { return _items.Count == 0; } }
		public int ItemCount { get { return _items.Count; } }

		public event ItemListChangedHandler OnItemListChanged;
		public event InventoryOpenedHandler OnInventoryOpened;
		public event InventoryClosedHandler OnInventoryClosed;

		/// <summary>Finds the given item name in the inventory</summary>
		public KeyValuePair<CardObject, int> GetItem(string item)
		{
			try {
				return _items.SingleOrDefault(x => x.Key.name == item);
			}
			catch (InvalidOperationException) { Debug.LogWarning("More than one item with the name " + item + " exists in the inventory."); }
			return default;
		}

		/// <summary>Finds the given item in the inventory</summary>
		public KeyValuePair<CardObject, int> GetItem(CardObject item)
		{
			try {
				return _items.SingleOrDefault(x => x.Key == item);
			}
			catch (InvalidOperationException) { Debug.LogWarning("More than one item with the name " + item + " exists in the inventory."); }
			return default;
		}

		/// <summary>Returns true if the specified item is in the current inventory (doesn't check equipment slots)</summary>
		public bool Contains(string item)
		{
			return _items.Any(x => x.Key.name == item);
		}

		/// <summary>Adds an item to the inventory</summary>
		public void Add(string item, int quantity = 1)
		{
			var i = GetItem(item);
			if (i.Equals(default)) {
				UnityEngine.Object obj = Resources.Load(item);
				if (obj is Tower)
					_items.Add((Tower)obj, quantity);
				else if (obj is Spell)
					_items.Add((Spell)obj, quantity);
			}
			else
				_items[i.Key] += quantity;

			OnItemListChanged?.Invoke();
		}

		public void Add(CardObject item, int quantity = 1)
		{
			if (_items.ContainsKey(item))
				_items[item] += quantity;
			else
				_items.Add(item, quantity);

			OnItemListChanged?.Invoke();
		}

		/// <summary>Removes an item from the inventory</summary>
		public void Remove(string item, int quantity = 1)
		{
			if (string.IsNullOrEmpty(item))
				return;

			var i = GetItem(item);
			if (i.Equals(default))
				return;

			if (i.Value > quantity)
				_items[i.Key] -= quantity;
			else
				_items.Remove(i.Key);

            OnItemListChanged?.Invoke();
		}

		public void Remove(CardObject item, int quantity = 1)
		{
			if (_items.ContainsKey(item)) {
				if (_items[item] > quantity)
					_items[item] -= quantity;
				else
					_items.Remove(item);
			}

			OnItemListChanged?.Invoke();
		}


		public void HandleInventoryOpened()
		{
            OnInventoryOpened?.Invoke(this);
        }

		public void HandleInventoryClosed()
		{
            OnInventoryClosed?.Invoke(this);
        }

		public delegate void ItemListChangedHandler();
		public delegate void InventoryOpenedHandler(Inventory inv);
		public delegate void InventoryClosedHandler(Inventory inv);
    }
}