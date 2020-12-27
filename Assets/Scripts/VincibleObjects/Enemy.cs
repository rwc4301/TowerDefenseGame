using UnityEngine;
using Mission;
using UnityEngine.AI;
using System.Collections;

public enum EnemyType { Virus, Bacteria, Worm, Fungi, Cancer_Cell }

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IVincible
{
	private const float MELEE_ATTACK_RANGE = 1.0f;

	public enum PreferredTarget { None, Turrets, Resources, Any }
	public enum AttackType { Ranged, Melee }
	public enum MovementSpeed { Very_Slow = 1, Slow = 2, Medium = 3, Fast = 4, Very_Fast = 5 }

	public enum Behaviour { Reach_Closest_Exit, Attack_Primary_Target, Attack_Strongest_Turret, Attack_Strongest_Resource, Attack_Closest_Tower }

	//depending on the mission type, enemies may be of any level, only below player level, within 2 levels of player, etc.
	[SerializeField] private int m_level = 0, m_damage = 10, m_attackSpeed = 1;
	[SerializeField] private float m_range = 0;
	[SerializeField] private EnemyType m_type = EnemyType.Virus;
	[SerializeField] private Enemy m_weakerVariation = null, m_strongerVariation = null;
	[SerializeField] private PreferredTarget m_preferredTarget = PreferredTarget.None;
	[SerializeField] private Material m_damagedMaterial = null;
	[SerializeField] private Projectile m_projectile = null;
	[SerializeField] private Transform m_projectileHook = null;
	[SerializeField] private AttackType m_attackType = AttackType.Ranged;
	[SerializeField] private SpecialProperty[] m_onAttackProperties = new SpecialProperty[0];
	[SerializeField] private MovementSpeed m_moveSpeed = MovementSpeed.Medium;
	[SerializeField] private Behaviour m_primaryBehaviour = Behaviour.Reach_Closest_Exit, m_secondaryBehaviour = Behaviour.Attack_Closest_Tower;
	[SerializeField] private bool m_respondToAttacks = true;
	[SerializeField] protected int m_maxHP = 10;
	[SerializeField] protected float m_lifeTime = 0;  //0 life time is infinite
	[SerializeField] protected SpecialProperty[] m_onSpawnProperties = new SpecialProperty[0];
	[SerializeField] protected string m_name;

	protected int m_HP = 0;
	protected float m_remainingLifeTime = 0;

	private Vector3 m_exitPos;
	private float m_cooldownTime = 0;
	private IVincible m_prospectiveTarget;
	private Behaviour m_currentBehaviour;

	public string Name { get => m_name; }
	public int Level { get { return m_level; } }
	public EnemyType Type { get { return m_type; } }
	public IVincible CurrentTarget { get; private set; }
	public IVincible NavTarget { get { return m_prospectiveTarget; }
		set {
			m_prospectiveTarget = value;
			if (value != null)
				navMeshAgent.SetDestination(value.gameObject.transform.position);
			else
				navMeshAgent.ResetPath();
		}
	}
	private NavMeshAgent navMeshAgent { get; set; }

	public int Attackers => throw new System.NotImplementedException();

	public event DestroyedHandler OnDestroyed;
	public event ReachedExitHandler OnReachedExit;
	private event AttackHandler OnAttack;

	public delegate void DestroyedHandler();
	public delegate void ReachedExitHandler();
	private delegate void AttackHandler();

	protected void Start()
	{
		m_HP = m_maxHP;
		if (m_lifeTime > 0)
			m_remainingLifeTime = m_lifeTime;

		navMeshAgent = GetComponent<NavMeshAgent>();
		navMeshAgent.speed = (float)m_moveSpeed;

		if (m_attackType == AttackType.Melee)
			m_range = MELEE_ATTACK_RANGE;
		navMeshAgent.stoppingDistance = m_range;

		m_currentBehaviour = m_primaryBehaviour;

		switch (m_attackType) {
			case AttackType.Melee:
				OnAttack += MeleeAttack;
				break;
			case AttackType.Ranged:
				OnAttack += RangedAttack;
				break;
		}

		StartCoroutine(CheckForBehaviourInterruptions());
	}

	public void OnAttacked(AttackedEventArgs args)
	{
		m_HP -= args.Damage;

		foreach (SpecialProperty p in args.OnAttackProperties) {
			if (m_type == p.damageVs)
				m_HP -= p.damageModifier;
		}

		if (m_HP <= 0) {
			OnDestroyed?.Invoke();
			Destroy(gameObject);
		}
		else {
			if (m_respondToAttacks) {
				NavTarget = (Tower)args.Attacker;
			}

			StartCoroutine(OnDamagedEffects());
		}
	}

	/// <summary>
	/// Continually checks that the enemy's path has not been cut off by a tower, and if it has then interrupt the behaviour
	/// </summary>
	private IEnumerator CheckForBehaviourInterruptions()
	{
		NavMeshPath path = new NavMeshPath();
		WaitForSeconds wait = new WaitForSeconds(1.0f);

		while (true) {
			//if the enemy cannot reach its target, this is an interruption and the secondary behaviour will take place
			if (navMeshAgent.hasPath && !NavMesh.CalculatePath(transform.position, navMeshAgent.destination, NavMesh.AllAreas, path)) {
				Debug.Log("interrupted", this);
				NavTarget = null;
				m_currentBehaviour = m_currentBehaviour == m_primaryBehaviour ? m_secondaryBehaviour : m_primaryBehaviour;

			}
			//if the nav target already has four attackers and we are not one of them, lose the nav target so we can find another
			else if (NavTarget?.Attackers >= 4 && NavTarget != CurrentTarget) { //when we're not yet attacking the target but it has the max number of attackers
				Debug.Log("interrupted", this);
				NavTarget = null;
			}

			yield return wait;
		}
	}

	private void Update()
	{
		if (m_cooldownTime > 0)
			m_cooldownTime -= Time.deltaTime;

		if (m_cooldownTime <= 0)
			OnAttack?.Invoke();

		switch (m_currentBehaviour) {
			case Behaviour.Reach_Closest_Exit:
				if (m_exitPos == default) { //find the closest exit point
					//TODO: calculate closest exit by following the navigation path
					float dist = float.MaxValue;
					foreach (Vector3 exit in MissionController.current.ExitPoints)
						if (dist > (dist = Vector3.Distance(transform.position, exit)))
							m_exitPos = exit;

					navMeshAgent.SetDestination(m_exitPos);
				}
				else if (Vector3.Distance(transform.position, m_exitPos) < 2) {	//when in range of its destination, exit the scene
					OnReachedExit?.Invoke();
					Destroy(gameObject);
				}
				break;
			case Behaviour.Attack_Primary_Target:
				//we don't have a nav target, choose the primary one
				if (NavTarget == null) { 
                    NavTarget = MissionController.current.DefenseTarget;
                }
				//if the primary target is already saturated with attackers, we will change to the secondary behaviour
				if (NavTarget?.Attackers >= 4 && NavTarget != CurrentTarget) {
					m_currentBehaviour = m_secondaryBehaviour;
					NavTarget = null;
				}
				break;
			case Behaviour.Attack_Strongest_Turret:
				//while we've been moving to our target, four of us have started attacking it. we can set our target to null and search for a new one
				if (NavTarget?.Attackers >= 4 && NavTarget != CurrentTarget) {
					NavTarget = null;
				}
				//we don't have a target, find the strongest tower with less than four attackers
				if (NavTarget == null) {
					Tower t = Tower.GetStrongestOfTypeWithoutAttackers(TowerType.SingleTarget | TowerType.MultiTarget | TowerType.AOE | TowerType.Special);
					NavTarget = t;
				}
				break;
			case Behaviour.Attack_Strongest_Resource:
				//while we've been moving to our target, four of us have started attacking it. we can set our target to null and search for a new one
				if (NavTarget?.Attackers >= 4 && NavTarget != CurrentTarget) {
					NavTarget = null;
				}
				//we don't have a target, find the strongest tower with less than four attackers
				if (NavTarget == null) {
					Tower t = Tower.GetStrongestOfTypeWithoutAttackers(TowerType.ResourceGatherer);
					NavTarget = t;
				}
				break;
			case Behaviour.Attack_Closest_Tower:
				//while we've been moving to our target, four of us have started attacking it. we can set our target to null and search for a new one
				if (NavTarget?.Attackers >= 4 && NavTarget != CurrentTarget) {
					NavTarget = null;
				}
				//we don't have a target, find the strongest tower with less than four attackers
				if (NavTarget == null) {
					Tower t = Tower.GetClosestWithoutAttackers(transform.position);
					NavTarget = t;
				}
				break;
		}
	}

	private void MeleeAttack()
	{
		m_cooldownTime = m_attackSpeed;

		//TODO: play attacking animation
		//possibly add a collision check to the attack function so towers are damaged when an attack actually connects

		CurrentTarget.OnAttacked(new AttackedEventArgs { Attacker = this, Attacked = CurrentTarget, Damage = m_damage, OnAttackProperties = m_onAttackProperties });
	}

	private void RangedAttack()
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

	private IEnumerator OnDamagedEffects()
	{
		var rend = GetComponent<MeshRenderer>();
		Material mat = rend.material;

		rend.material = m_damagedMaterial;

		yield return new WaitForSeconds(0.1f);

		rend.material = mat;
	}

	public Enemy GetWeakerVariation()
	{
		return m_weakerVariation;
	}

	public Enemy GetStrongerVariation()
	{
		return m_strongerVariation;
	}

	public Enemy GetStrongestVariation(Enemy currentVariation, int maxlevel)
	{
		if (currentVariation == null)
			throw new System.Exception("All variations of this enemy are stronger than the maxmimum level");

		if (GetStrongerVariation()?.Level <= maxlevel)
			return GetStrongestVariation(GetStrongerVariation(), maxlevel);
		else if (Level > maxlevel)
			return GetStrongestVariation(GetWeakerVariation(), maxlevel);
		else
			return this;
	}

	public Enemy GetWeakestVariation(Enemy currentVariation, int minlevel)
	{
		if (currentVariation == null)
			throw new System.Exception("All variations of this enemy are weaker than the minimum level");

		if (GetWeakerVariation()?.Level >= minlevel)
			return GetWeakestVariation(GetWeakerVariation(), minlevel);
		else if (Level < minlevel)
			return GetWeakestVariation(GetStrongerVariation(), minlevel);
		else
			return this;
	}
}