using Newtonsoft.Json;
using UnityEngine;

namespace BodyOverview
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Organ : MonoBehaviour
	{
		[SerializeField] [JsonProperty]
		private MissionNode[] m_missionNodes = new MissionNode[0];

		[JsonProperty] private bool m_infected = false;

		public void Infect()
		{
			m_infected = true;
		}

		public void Disinfect()
		{
			m_infected = false;
		}
	}
}