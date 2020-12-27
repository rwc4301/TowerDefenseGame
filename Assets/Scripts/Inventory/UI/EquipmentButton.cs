using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace InventoryManagement.UI
{
	public class EquipmentButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		[SerializeField] private InventoryPanel _selectionPanel = null;
		[SerializeField] private Image _image = null;
		[SerializeField] private Sprite _defaultIcon = null;
		[SerializeField] private bool _filterPanelOnHover = true;
		[SerializeField] private int _slotNumber;
		[SerializeField] private bool _hideOnStart = false;

		public Inventory Inventory { get { return _selectionPanel.Inventory; } }
		public int SlotNumber { get { return _slotNumber; } }
		public bool IsOpen { get { return gameObject.activeSelf; } }

		/// <summary>
		/// Update the button
		/// </summary>
		public void Refresh()
		{
			//BaseItem item = ItemDatabase.(SlotNumber);
			//if (_image)
			//	_image.sprite = item ? item.Icon : _defaultIcon;
		}

		/// <summary>
		/// Hide the button
		/// </summary>
		public void Close()
		{
			gameObject.SetActive(false);
		}

		public void OnPointerEnter(PointerEventData args)
		{
			//if (_filterPanelOnHover)
			//	_selectionPanel.OnEquipmentButtonHover(SlotType);
		}

		public void OnPointerExit(PointerEventData args)
		{
			if (_filterPanelOnHover)
				_selectionPanel.OnEquipmentButtonLeave();
		}

		public void OnPointerClick(PointerEventData args)
		{
			_selectionPanel.OnEquipmentButtonClick(SlotNumber);
		}

		private void Start()
		{
			Refresh();
			if (_hideOnStart)
				Close();
		}
	}
}