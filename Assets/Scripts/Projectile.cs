using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	private const int FORCE = 1000;

	private Object m_sender;
	private Vector3 m_initialPos;
	private float m_range;

	public event HitHandler OnHit;
	public delegate void HitHandler(IVincible target);

	public Projectile InstantiateProjectile(Object sender, Transform projectileHook, float range)
	{
		Projectile projectile = Instantiate(this, projectileHook.position, projectileHook.rotation);

		projectile.m_sender = sender;
		projectile.m_initialPos = projectileHook.position;
		projectile.m_range = range;

		projectile.GetComponent<Rigidbody>().AddForce(projectileHook.forward * FORCE, ForceMode.Force);

		return projectile;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject == m_sender)
			return;

		var target = collision.collider.GetComponent<IVincible>();
		if (target != null)
			OnHit?.Invoke(target);

		Destroy(gameObject);
	}

	private void Update()
	{
		if (Vector3.Distance(m_initialPos, transform.position) >= m_range)
			Destroy(gameObject);
	}
}