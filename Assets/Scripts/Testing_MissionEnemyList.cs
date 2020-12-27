using Mission;
using UnityEngine;

class Testing_MissionEnemyList : MonoBehaviour
{
	public Enemy[] enemies;

	public void Start()
	{
		var mc = FindObjectOfType<MissionController>();
		if (mc)
			mc.EnemyTypes = enemies;
	}
}
