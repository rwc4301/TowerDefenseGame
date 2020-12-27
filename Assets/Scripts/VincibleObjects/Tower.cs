using Mission;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum TowerType { SingleTarget = 1, MultiTarget = 2, AOE = 4, Special = 8, ResourceGatherer = 16 }

[RequireComponent(typeof(NavMeshObstacle), typeof(SphereCollider), typeof(Rigidbody))]
public class Tower : MonoBehaviour, ICard, IVincible
{
	public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }

	[SerializeField] private float m_range = 3;
	[SerializeField] private Rarity m_rarity = Rarity.Common;
	[SerializeField] private int m_damage = 10, m_attackSpeed = 1;
	[SerializeField] private Projectile m_projectile = null;
	[SerializeField] private Transform m_projectileHook = null;
	[SerializeField] private TowerType m_type = TowerType.SingleTarget;
	[SerializeField] private SpecialProperty[] m_onAttackProperties = new SpecialProperty[0];
	[SerializeField] private Material m_damagedMaterial = null;
	[SerializeField] private bool m_respondToAttacks;
	[SerializeField] protected string m_name = "", m_description = "";
	[SerializeField] protected int m_maxHP = 10;
	[SerializeField] protected float m_lifeTime = 0;  //0 life time is infinite
	[SerializeField] protected int m_naniteCost = 0, m_WBCCost = 0;
	[SerializeField] protected bool m_oneTimeUse = false;
	[SerializeField] protected Sprite m_sprite = null;
	[SerializeField] protected SpecialProperty[] m_onSpawnProperties = new SpecialProperty[0];

	protected int m_HP = 0;
	protected float m_remainingLifeTime = 0;

	private List<Enemy> m_targets = new List<Enemy>();

	private Enemy CurrentTarget { get; set; }
	private float m_cooldownTime = 0;

	private event AttackHandler OnAttack;
	private delegate void AttackHandler();

	public event DestroyedHandler OnDestroyed;
	public delegate void DestroyedHandler();

	/// <summary>Return the number of enemies that are attacking this turret</summary>
	public int Attackers { get { return FindObjectsOfType<Enemy>().Where(x => x.CurrentTarget == this).Count(); } }
	public float DPS { get { return m_damage / m_attackSpeed; } }

	public string Name { get => m_name; }
	public Sprite Sprite { get => m_sprite; }
	public int NaniteCost { get => m_naniteCost; }
	public int WBCCost { get => m_WBCCost; }
	public bool OneTimeUse { get => m_oneTimeUse; }

	public static Tower GetClosestWithoutAttackers(Vector3 position)
	{
		return FindObjectsOfType<Tower>().Where(x => x.Attackers < 4).OrderBy(x => Vector3.Distance(position, x.transform.position)).First();
	}

	public static Tower GetClosestWithoutAttackers(Vector3 position, TowerType type)
	{
		return FindObjectsOfType<Tower>().Where(x => x.Attackers < 4 && x.m_type == type).OrderBy(x => Vector3.Distance(position, x.transform.position)).First();
	}

	public static Tower GetStrongestOfType(TowerType type)
	{
		return FindObjectsOfType<Tower>().Where(x => (x.m_type & type) > 0).OrderBy(x => x.DPS).First();
	}

	public static Tower GetStrongestOfTypeWithoutAttackers(TowerType type)
	{
		return FindObjectsOfType<Tower>().Where(x => (x.m_type & type) > 0 && x.Attackers < 4).OrderBy(x => x.DPS).First();
	}

	protected void Start()
	{
		m_HP = m_maxHP;
		if (m_lifeTime > 0)
			m_remainingLifeTime = m_lifeTime;

		GetComponent<SphereCollider>().isTrigger = true;
		GetComponent<SphereCollider>().radius = m_range;

		switch (m_type) {
			case TowerType.SingleTarget:
				OnAttack += SingleAttack;
				break;
			case TowerType.AOE:
				OnAttack += AOEAttack;
				break;
			case TowerType.ResourceGatherer:
				OnAttack += GatherResources;
				break;
		}

		foreach (SpecialProperty p in m_onSpawnProperties) {
			if (p.isAOEDamaging) {
				foreach (RaycastHit hit in Physics.SphereCastAll(transform.position, m_range, transform.forward)) {
					Enemy target = hit.transform.GetComponent<Enemy>();
					if (target.Type == p.damageVs)
						target.OnAttacked(new AttackedEventArgs
						{
							Attacker = this,
							Attacked = target,
							Damage = m_damage,
							OnAttackProperties = new SpecialProperty[1] { p }
						});
				}
			}
			else if (p.wbcGeneration > 0) {
				MissionController.current.WBCPerSecond += p.wbcGeneration;
				OnDestroyed += () => { MissionController.current.WBCPerSecond -= p.wbcGeneration; };
			}
		}
	}

	protected virtual void Update()
	{
		m_remainingLifeTime -= Time.deltaTime;
		if (m_lifeTime > 0 && m_remainingLifeTime <= 0) //0 life time is infinite
			Destroy(gameObject);

		if (m_cooldownTime > 0)
			m_cooldownTime -= Time.deltaTime;

		for (int i = m_targets.Count - 1; i >= 0; i--)
			if (!m_targets[i])
				m_targets.RemoveAt(i);

		if (m_targets.Count > 0) {
			if (m_type == TowerType.SingleTarget)    //single target turrets track target, final version will depend on turret implementation, to be checked with artist
				transform.LookAt(m_targets[0].transform.position); 
			else if (m_type == TowerType.MultiTarget) { }

			if (m_cooldownTime <= 0)
				OnAttack?.Invoke();
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.GetComponent<Enemy>() && (m_type != TowerType.SingleTarget || m_targets.Count == 0))
			m_targets.Add(collider.GetComponent<Enemy>());
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.GetComponent<Enemy>())
			m_targets.Remove(collider.GetComponent<Enemy>());
	}

	private void SingleAttack()
	{
		m_cooldownTime = m_attackSpeed;

		if (!m_projectile) {
			Debug.Log(Name + " cannot attack without a projectile prefab", this);
			return;
		}

		Projectile p = m_projectile.InstantiateProjectile(gameObject, m_projectileHook, m_range);
		p.OnHit += (IVincible target) =>
		{
			target.OnAttacked(new AttackedEventArgs { Attacker = this, Attacked = target, Damage = m_damage, OnAttackProperties = m_onAttackProperties });
		};
	}

	private void AOEAttack()
	{
		m_cooldownTime = m_attackSpeed;

		foreach (Enemy target in m_targets) {
			target.OnAttacked(new AttackedEventArgs { Attacker = this, Attacked = target, Damage = m_damage, OnAttackProperties = m_onAttackProperties });
		}
	}

	private void GatherResources()
	{
		m_cooldownTime = m_attackSpeed;

		MissionController.current.AddWBC(m_damage);
	}

	public void OnAttacked(AttackedEventArgs args)
	{
		m_HP -= args.Damage;

		foreach (SpecialProperty p in args.OnAttackProperties) {

		}

		if (m_HP <= 0) {
			OnDestroyed?.Invoke();
			Destroy(gameObject);
		}
		else {
			if (m_respondToAttacks) {
				CurrentTarget = (Enemy)args.Attacker;
			}

			StartCoroutine(OnDamagedEffects());
		}
	}

	private IEnumerator OnDamagedEffects()
	{
		var rend = GetComponent<MeshRenderer>();
		Material mat = rend.material;

		rend.material = m_damagedMaterial;

		yield return new WaitForSeconds(0.1f);

		rend.material = mat;
	}
}