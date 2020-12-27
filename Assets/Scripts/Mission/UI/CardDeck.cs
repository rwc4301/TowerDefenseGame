using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mission.UI
{
	public class CardDeck : MonoBehaviour
	{
		[SerializeField] private Image m_nextCard = null, m_salvageCard = null;
		[SerializeField] private Image[] m_visibleDeckImages = null;
		[SerializeField] private Button[] m_visibleDeckButtons = null;
		[SerializeField] private Text m_wbcValueText = null;

		Material matPreviewOK, matPreviewRestricted;

		private void Awake()
		{
			matPreviewOK = Resources.Load<Material>("Materials/PreviewMaterialOK");
			matPreviewRestricted = Resources.Load<Material>("Materials/PreviewMaterialRestricted");

			MissionController.current.OnCardDeckChanged += OnCardDeckChanged;
			MissionController.current.OnWBCCountChanged += OnWBCCountChanged;
		}

		private void OnWBCCountChanged(int wbc)
		{
			if (m_wbcValueText)
				m_wbcValueText.text = wbc.ToString();
		}

		private IEnumerator DragCard(Image cardImage, ICard card)
		{
			//WaitForSeconds delay = new WaitForSeconds(0.1f);
			RectTransform rt = cardImage.GetComponent<RectTransform>();
			Vector2 lastMousePos = Input.mousePosition;
			Vector2 anchoredPos = rt.anchoredPosition;
			Vector2 delta = Vector2.zero;
			bool hasBeenDragged = false;

			while (true) {
				hasBeenDragged = hasBeenDragged || delta != Vector2.zero;

				delta = (Vector2)Input.mousePosition - lastMousePos;
				lastMousePos = Input.mousePosition;
				//rt.anchoredPosition += delta;

				//find the point on the terrain directly beneath the mouse cursor
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) {
					//place turret preview on closest point to mouse on grid
					Vector3 pos = new Vector3(Mathf.RoundToInt(hit.point.x), 0, Mathf.RoundToInt(hit.point.z));
					bool canPlace = MissionController.current.GetCanBuildOnPosition((int)pos.x, (int)pos.z);
					Mesh mesh = card.gameObject.GetComponent<MeshFilter>().sharedMesh;
					Material mat = canPlace ? matPreviewOK : matPreviewRestricted;

					//do a second raycast to find the height of the terrain at this grid point
					if (Physics.Raycast(pos, Vector3.up * 100, out hit))
						pos = hit.point;

					//modify the y ordinate so the lower bound of the mesh sits on the terrain
					pos.y += mesh.bounds.extents.y;

					Graphics.DrawMesh(mesh, pos, Quaternion.identity, mat, 0);

					if (canPlace && hasBeenDragged && Input.GetMouseButtonUp(0)) {
						if (MissionController.current.UseCard(card, pos))
							Destroy(cardImage.gameObject);
					}
				}

				yield return null;
			}
		}

		private void OnCardDeckChanged(List<ICard> deck)
		{
			for (int i = 0; i < m_visibleDeckImages.Length; i++) {
				int index = i;
				m_visibleDeckImages[i].sprite = deck[i]?.Sprite;

				m_visibleDeckButtons[i].onClick.RemoveAllListeners();
				m_visibleDeckButtons[i].onClick.AddListener(() =>
				{
					OnCardClicked(m_visibleDeckImages[index], deck[index]);
				});
			}

			m_nextCard.sprite = deck[5]?.Sprite;
			
		}

		private void OnCardClicked(Image cardImage, ICard card)
		{
			Image i = Instantiate(cardImage, transform);
			Destroy(i.GetComponent<Button>());

			i.StartCoroutine(DragCard(i, card));
		}
	}
}