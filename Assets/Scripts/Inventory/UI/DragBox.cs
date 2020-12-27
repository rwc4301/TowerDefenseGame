using UnityEngine;
using System.Collections;

namespace InventoryManagement.UI
{
	public class DragBox : MonoBehaviour
	{
		private CardObject _item;
		private Inventory _source;

		public void Open(CardObject item, Inventory src)
		{
			_item = item;
			_source = src;
		}

		public void Use(Inventory dest)
		{
			if (_source != dest) {
				dest.Add(_item.name);
				_source.Remove(_item.name);
			}
			Destroy(gameObject);
		}

		private void Start()
		{
			StartCoroutine(FollowMouse());
		}

		private IEnumerator FollowMouse()
		{
			WaitForSeconds delay = new WaitForSeconds(0.2f);
			Vector2 lastMousePos = Vector2.zero;
			Vector2 delta = Vector2.zero;
			RectTransform rt = GetComponent<RectTransform>();

			while (true) {
				delta = (Vector2)Input.mousePosition - lastMousePos + new Vector2(10, 0);
				lastMousePos = (Vector2)Input.mousePosition + new Vector2(10, 0);
				rt.anchoredPosition += delta;
				yield return delay;
			}
		}
	}
}