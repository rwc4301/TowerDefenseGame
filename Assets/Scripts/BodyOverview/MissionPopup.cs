using UnityEngine;
using UnityEngine.UI;
using Mission;

namespace BodyOverview.UI
{
	public class MissionPopup : MonoBehaviour
	{
		public static void Show(MissionNode mission)
		{
			var popup = FindObjectOfType<MissionPopup>() ?? Instantiate(Resources.Load<MissionPopup>("UI/MissionPopup"), FindObjectOfType<Canvas>().transform);

			popup.transform.Find("MissionName").GetComponent<Text>().text = mission.Name;
			popup.transform.Find("MissionDescription").GetComponent<Text>().text = mission.Description;

			for (int i = 0; i < MissionController.ENEMIES_PER_SCENE; i++) {
				popup.transform.Find("Enemy" + i).GetComponentInChildren<Text>().text = "Enemy";
			}

			popup.transform.Find("Play").GetComponent<Button>().onClick.AddListener(delegate { GameManager.LoadMission(mission.SceneName, mission.GetComponent<Testing_MissionEnemyList>()?.enemies, FindObjectOfType<Testing_PlayerCardDeck>().playerDeck); });
		}
	}
}