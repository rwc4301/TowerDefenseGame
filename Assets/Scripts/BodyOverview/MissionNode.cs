using Newtonsoft.Json;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[JsonObject(MemberSerialization.OptIn)]
public class MissionNode : MonoBehaviour
{
	[SerializeField] private string m_missionName = "", m_missionDescription = "", m_sceneName = "";
	[SerializeField] private EnemyType[] m_enemyTypes = new EnemyType[0];
	[JsonProperty] private bool m_completed = false;

	public string Name { get { return m_missionName; } }
	public string Description { get { return m_missionDescription; } }
	public string SceneName { get { return m_sceneName; } }
	public EnemyType[] EnemyTypes { get { return m_enemyTypes; } }

	public bool Completed {
		get { return m_completed; }
		set {
			m_completed = value;
			if (value)
				OnCompletedMission();
		}
	}

	private void OnMouseOver()
	{
	}

	private void OnMouseDown()
	{
		GameManager.ShowMissionPopupScreen(this);
	}

	private void OnCompletedMission()
	{

	}
}