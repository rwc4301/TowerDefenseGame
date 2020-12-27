using UnityEngine;

public abstract class CardObject : VincibleObject
{
	[SerializeField] protected int m_naniteCost = 0, m_wbcCost = 0;
	[SerializeField] protected bool m_oneTimeUse = false;
	[SerializeField] protected Sprite m_sprite = null;
	[SerializeField] protected SpecialProperty[] m_onSpawnProperties = new SpecialProperty[0];

	public Sprite Sprite { get { return m_sprite; } }
	public int NaniteCost { get { return m_naniteCost; } }
	public int WBCCost { get { return m_wbcCost; } }
	public bool OneTimeUse { get { return m_oneTimeUse; } }

	protected override void Start()
	{
		base.Start();
	}
}
