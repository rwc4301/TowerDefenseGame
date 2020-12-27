using Mission;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing_PlayerCardDeck : MonoBehaviour
{
	public CardObject[] playerDeck;

	public void Start()
	{
		var mc = FindObjectOfType<MissionController>();
		if (mc)
			mc.SetCardDeck(playerDeck);
	}
}
