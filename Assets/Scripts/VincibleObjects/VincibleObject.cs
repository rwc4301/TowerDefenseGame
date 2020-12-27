using UnityEngine;

public abstract class VincibleObject : MonoBehaviour
{
	[SerializeField] protected string m_name = "", m_description = "";
	[SerializeField] protected int m_maxHP = 10;
	[SerializeField] protected float m_lifeTime = 0;  //0 life time is infinite

	protected int m_HP = 0;
	protected float m_remainingLifeTime = 0;

	public string Name { get { return m_name; } }
	public string Description { get { return m_description; } }

	protected virtual void Start()
	{
		m_HP = m_maxHP;
		if (m_lifeTime > 0)
			m_remainingLifeTime = m_lifeTime;
	}

	public virtual void OnAttacked(AttackedEventArgs args)
	{
		
	}
}