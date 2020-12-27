using Newtonsoft.Json;
using UnityEngine;

namespace BodyOverview
{
	[JsonObject(MemberSerialization.OptIn)]
	public class BodyOverviewController : MonoBehaviour
	{
		[SerializeField] [JsonProperty] private Organ[] m_organs = new Organ[0];
		[SerializeField] [JsonProperty] private Wound[] m_wounds = new Wound[0];
		[SerializeField] private Bounds[] m_woundBounds = new Bounds[0];			//bounding boxes for wound instantiations
		//[SerializeField] private Pharmacy m_pharmacy = new Pharmacy();

		public Organ[] Organs { get { return m_organs; } }

		public void InstantiateWound()
		{
			Wound wound = m_wounds[Random.Range(0, m_wounds.Length)];
			Bounds bounds = m_woundBounds[Random.Range(0, m_woundBounds.Length)];

			Vector3 pos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));

			Instantiate(wound, pos, Quaternion.identity);
		}

		/// <summary>
		/// Call this when the body overview screen is loaded after a mission is completed, increases mission count for the wounds etc.
		/// </summary>
		public void ReturnFromMission()
		{
			foreach (Wound w in m_wounds)
				w.MissionsSinceInception++;
		}
	}
}