using System.Collections;
using UnityEngine;

public class Spell : MonoBehaviour, ICard
{
	public string Name => throw new System.NotImplementedException();

	public Sprite Sprite => throw new System.NotImplementedException();

	public int NaniteCost => throw new System.NotImplementedException();

	public int WBCCost => throw new System.NotImplementedException();

	public bool OneTimeUse => throw new System.NotImplementedException();
}