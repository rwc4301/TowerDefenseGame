using UnityEngine;

public interface IVincible
{
	GameObject gameObject { get; }
	int Attackers { get; }

	void OnAttacked(AttackedEventArgs args);
}