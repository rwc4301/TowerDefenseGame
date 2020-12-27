using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class DefenseTarget : MonoBehaviour, IVincible
{
	/// <summary>Return the number of enemies that are attacking this turret</summary>
	public int Attackers { get { return FindObjectsOfType<Enemy>().Where(x => x.CurrentTarget == this).Count(); } }

	public void OnAttacked(AttackedEventArgs args)
	{
		throw new System.NotImplementedException();
	}
}
