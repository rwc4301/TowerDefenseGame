using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public interface ICard
{
	[JsonProperty] string name { get; }
	GameObject gameObject { get; }
	string Name { get; }
	Sprite Sprite { get; }
	int NaniteCost { get; }
	int WBCCost { get; }
	bool OneTimeUse { get; }
}