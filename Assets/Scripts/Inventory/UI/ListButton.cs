using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace InventoryManagement.UI
{
	public class ListButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		[SerializeField] private Text ItemName;
		[SerializeField] private Image ItemIcon;
		[SerializeField] private Text ItemCost;
		[SerializeField] private Text ItemCount;

		private CardObject ItemReference;

		public int ListIndex { get; set; }
		public InventoryPanel Panel { get; set; }

		public void SetItemReference(CardObject item, int stacksize)
		{
			ItemReference = item;

			if (ItemName != null)
				ItemName.text = (item == null) ? item.Name : "None";

			if (ItemIcon != null)
				ItemIcon.sprite = (item == null) ? item.Sprite : null;

			if (ItemCost != null)
				ItemCost.text = (item == null) ? string.Format("{0:#.00}", item.NaniteCost) : "";

			if (ItemCount != null)
				ItemCount.text = stacksize == 1 ? "" : stacksize.ToString();
		}

		public void PostionAndSize(float left, float top, float width)
		{
			RectTransform t = GetComponent<RectTransform>();

			t.pivot = new Vector2(0, 1);
			t.anchorMin = new Vector2(0, 0);
			t.anchorMax = new Vector2(0, 0);
			t.anchoredPosition = new Vector2(left, top);
			t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		}

		public void OnPointerEnter(PointerEventData args)
		{
			Panel.OnItemButtonHover(ItemReference);
		}

		public void OnPointerExit(PointerEventData args)
		{
			Panel.OnItemButtonLeave();
		}

		public void OnPointerClick(PointerEventData args)
		{
			Panel.OnItemButtonClick(ItemReference);
		}
	}
}