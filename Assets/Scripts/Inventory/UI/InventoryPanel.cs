using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;

namespace InventoryManagement.UI
{
	public class InventoryPanel : MonoBehaviour, IPointerClickHandler
	{
		public enum SourceType { Player, Loot, Store }
		public enum ClickAction { None, Move, Equip, Drag }

		[SerializeField] private RectTransform ItemHolder;
		[SerializeField] private ListButton ItemButton;
		[SerializeField] private Button TakeAllButton;
		[SerializeField] private Button CloseButton;

		[SerializeField] private SourceType Source;

		[SerializeField] private bool ShowWeight;
		[SerializeField] private Text WeightText;
		[SerializeField] private string WeightStringFormat = "Weight: {0}/{1}";

		[SerializeField] private bool ShowMoney;
		[SerializeField] private Text MoneyText;

		[SerializeField] private bool ShowObjectName;
		[SerializeField] private Text ObjectNameText;

		[SerializeField] private bool ShowNoneItem;
		[SerializeField] private bool GridView;

		[SerializeField] private ClickAction OnClick;
		[SerializeField] private Inventory DestInv;
		[SerializeField] private GameObject DragBox;

		[SerializeField] private GameObject Tooltip;
		[SerializeField] private bool TooltipFollowMouse;

		public Inventory Inventory { get; private set; }

		public bool WaitingForEquip { get; private set; }
		public int ActiveSlot { get; set; }

		public bool IsStore { get { return Source == SourceType.Store; } }
		public bool IsOpen { get { return gameObject.activeSelf; } }

		public event PanelOpenedHandler OnPanelOpened;
		public event PanelClosedHandler OnPanelClosed;

		private GameObject TooltipInstance;
		private Type _typeMask = typeof(CardObject);

		/// <summary>
		/// Initializes the panel with a new inventory
		/// </summary>
		/// <param name="src">The source inventory to display</param>
		/// <param name="dest">If set, the inventory to move looted items to</param>
		/// <param name="show">If true show the loot panel, otherwise keep it closed</param> 
		public int Open(Inventory src, Inventory dest = null, bool show = true)
		{
			//If there was a previous inventory being displayed, clean up
			if (Inventory != null)
				Inventory.OnItemListChanged -= Refresh;

			//The source inventory should not be null
			if (src == null) {
				Debug.LogWarning("Tried to open a null inventory");
				return 1;
			}

			//Check all the GUI components are in place
			if (ItemHolder == null) {
				Debug.LogWarning("ItemHolder component cannot be null on " + gameObject.name);
				return 3;
			}

			//Store the new inventory
			Inventory = src;

			//Inventory opened event
			Inventory.HandleInventoryOpened();

			//Refresh when the inventory items change
			Inventory.OnItemListChanged += Refresh;

			//The destination inventory to move items to
			if (dest != null) DestInv = dest;

			//Panel opened handler
			OnPanelOpened?.Invoke();

			//Display the inventory if told to do so
			if (show) Refresh();

			//Event handlers for buttons
			if (TakeAllButton)
				TakeAllButton.onClick.AddListener(() => {
					if (DestInv != null) {
						//Inventory.CopyTo(DestInv);
						Close();
					}
				});

			if (CloseButton)
				CloseButton.onClick.AddListener(Close);

			//Success
			return 0;
		}

		/// <summary>
		/// Update the contents of the inventory panel
		/// </summary>
		public void Refresh()
		{
			gameObject.SetActive(true);

			//SetWeightText(Inventory.Weight);
			//SetMoneyText(Inventory.Money);
			//SetObjectNameText(Inventory.gameObject.name);

			//Remove any buttons left over from a previous inventory
			foreach (Transform t in ItemHolder) Destroy(t.gameObject);

			//Add an extra item if showing a none item at the top
			int count = ShowNoneItem ? Inventory.ItemCount + 1 : Inventory.ItemCount;

			//Button width and height
			float width = ItemButton.GetComponent<RectTransform>().rect.width;
			float height = ItemButton.GetComponent<RectTransform>().rect.height;

			//Size the list to fit all the inventory items
			ItemHolder.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * count);

			//ListButton button;
			//var items = Inventory.GetItems(_typeMask);

			//BaseItem[] keys = new BaseItem[items.Count + Convert.ToInt32(ShowNoneItem)];
			//items.Keys.CopyTo(keys, Convert.ToInt32(ShowNoneItem));
				
			//for (int i = 0; i < keys.Length; i++) {
			//	button = Instantiate(ItemButton);
			//	button.Panel = this;
			//	button.transform.SetParent(ItemHolder, false);
			//	button.PostionAndSize(0, ItemHolder.rect.height - (height * i), width);

			//	//check the item exists first to account for the none item
			//	button.SetItemReference(keys[i], !keys[i] ? 1 : items[keys[i]]);
			//}
		}

		/// <summary>
		/// Set which item types to show with a bit flag
		/// </summary>
		public void Filter(Type typemask)
		{
			if (!WaitingForEquip) {
				_typeMask = typemask;
				Refresh();
			}
		}

		/// <summary>
		/// Set a flag to tell the panel that whatever item is next selected should be equipped into the slot argument
		/// </summary>
		public void WaitForEquip(int slot)
		{
			WaitingForEquip = true;
			ActiveSlot = slot;
		}

		/// <summary>
		/// If the panel is waiting for an equip, clear the flag to say it has been handled
		/// </summary>
		public void SetEquipHandled()
		{
			WaitingForEquip = false;
			FindObjectsOfType<EquipmentButton>().First(x => x.SlotNumber == ActiveSlot).Refresh();
			Filter(typeof(CardObject));
		}

		/// <summary>
		/// Close the panel
		/// </summary>
		public void Close()
		{
			gameObject.SetActive(false);

			OnPanelClosed?.Invoke();

			if (Inventory != null) {
				//Do inventory closed event
				Inventory.HandleInventoryClosed();
			}
		}

		/// <summary>
		/// Called when the panel is clicked
		/// </summary>
		public void OnPointerClick(PointerEventData args)
		{
			DragBox box = GameObject.FindObjectOfType<DragBox>();
			if (box) {
				box.Use(Inventory);
			}
		}

		/// <summary>
		/// Create a box that indicates the player is dragging an item between inventories and allows the item to be added to another inventory
		/// </summary>
		private void CreateDragBox(CardObject item, Inventory src)
		{
			if (DragBox) {
				GameObject box = Instantiate<GameObject>(DragBox);
				box.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
				if (box.GetComponent<DragBox>()) {
					box.GetComponent<DragBox>().Open(item, src);
				}
				else
					Debug.LogWarning(box.name + " needs a DragBox component to drag items");
			}
			else
				Debug.LogWarning("The inventory panel " + name + " needs a drag box prefab to drag items");
		}

		public void OnEquipmentButtonHover(Type slotType)
		{
			Filter(slotType);
		}

		public void OnEquipmentButtonLeave()
		{
			Filter(typeof(CardObject));
		}

		public void OnEquipmentButtonClick(int slotNumber)
		{
			//If the UI is set to drag items, equip the item being dragged or empty the slot if nothing is being dragged
			if (OnClick == InventoryPanel.ClickAction.Drag) {

			}
			//Otherwise, on click choose an item to equip
			else {
				WaitForEquip(slotNumber);
			}
		}

		public void OnItemButtonHover(CardObject focusedItem)
		{
			if (Tooltip)
				OpenTooltip(focusedItem);
		}

		public void OnItemButtonLeave()
		{
			if (Tooltip)
				CloseTooltip();
		}

		public void OnItemButtonClick(CardObject clickedItem)
		{
			////If the panel is set to automatically move an item when clicked
			////And this button has an item reference
			//if (OnClick == InventoryPanel.ClickAction.Move && DestInv && clickedItem != null) {
			//	//If the player must buy the item, check they have enough money
			//	if (!IsStore || (IsStore && DestInv.Buy(clickedItem))) {
			//		//Add the item to the destination inventory
			//		//TODO: checks we don't exceed the weight or capacity limit
			//		DestInv.Add(clickedItem);
			//		//If successfull remove the item from the source inventory
			//		Inventory.Remove(clickedItem);

			//		if (Inventory.IsEmpty)
			//			Close();
			//		else
			//			Refresh();
			//	}
			//	else
			//		Debug.Log("Player did not have enough money to buy this item.");

			//	if (Tooltip)
			//		CloseTooltip();
			//}
			////If an equip request has been made, equip in the active slot
			////Doesn't need an item reference as we can equip nothing
			//else if (WaitingForEquip && OnClick == InventoryPanel.ClickAction.Equip) {
			//	Inventory.Equip(clickedItem, (int)ActiveSlot);

			//	SetEquipHandled();

			//	if (Tooltip)
			//		CloseTooltip();
			//}
			////If the panel is set to pick up an item for the user to drag
			////And this button has an item reference
			//else if (OnClick == InventoryPanel.ClickAction.Drag && DestInv && clickedItem != null) {
			//	if (Tooltip)
			//		CloseTooltip();

			//	CreateDragBox(clickedItem, Inventory);
			//}
			////If the panel is set to do nothing
			//else { }
		}

		private void OpenTooltip(CardObject item)
		{
			GameObject tooltip;

			if (TooltipFollowMouse) {
				tooltip = Instantiate<GameObject>(Tooltip);
				TooltipInstance = tooltip;

				//Find an active canvas to display the tooltip on
				Transform parent = FindObjectOfType<Canvas>().transform;

				tooltip.transform.SetParent(parent, true);
				tooltip.transform.position = transform.position + new Vector3(GetComponent<RectTransform>().rect.xMax, 0);
			}
			else
				tooltip = Tooltip;

			Text text = tooltip.GetComponentInChildren<Text>();
			if (text && item != null)
				text.text = item.Name + "\n\n" + item.Description;
		}

		private void CloseTooltip()
		{
			if (!TooltipFollowMouse)
				Tooltip.GetComponentInChildren<Text>().text = "";
			else if (TooltipInstance) {
				Destroy(TooltipInstance);
				TooltipInstance = null;
			}
		}

		public delegate void PanelOpenedHandler();
		public delegate void PanelClosedHandler();

		private void SetWeightText(int weight)
		{
			//if (ShowWeight && WeightText != null)
			//	WeightText.text = WeightStringFormat.Replace("{0}", weight.ToString()).Replace("{1}", Inventory.WeightLimit.ToString());
		}

		private void SetMoneyText(float money)
		{
			if (ShowMoney && MoneyText != null)
				MoneyText.text = money.ToString();
		}

		private void SetObjectNameText(string name)
		{
			if (ShowObjectName && ObjectNameText != null)
				ObjectNameText.text = name;
		}
	}
}