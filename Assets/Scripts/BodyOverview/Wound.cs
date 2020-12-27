using Newtonsoft.Json;
using UnityEngine;

namespace BodyOverview
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Wound : MonoBehaviour
	{
		public const int MISSIONS_BEFORE_INFECTION = 4;

		[JsonProperty] private int m_missionsSinceInception = 0;
		public int MissionsSinceInception {
			get { return m_missionsSinceInception; }
			set {
				m_missionsSinceInception = value;
				if (m_missionsSinceInception >= MISSIONS_BEFORE_INFECTION)
					InfectClosestOrgan();
			}
		}

		private void InfectClosestOrgan()
		{
			Organ organ = null;
			float dist = float.MaxValue;
			foreach (Organ o in FindObjectOfType<BodyOverviewController>().Organs)
				if (dist < (dist = Vector3.Distance(transform.position, o.transform.position)))
					organ = o;

			organ.Infect();
		}
	}
}