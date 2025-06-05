using UnityEngine;

// Structure to hold information about damage being dealt
public struct DamageInfo
{
	public float amount;          // Amount of damage to apply
	public GameObject inflictor;  // GameObject that caused the damage
	public Vector3 hitPoint;      // World position where the damage occurred  
	public Vector3 hitDirection;  // Direction the damage came from
}

public class DamageSource : MonoBehaviour
{
	[SerializeField] float damage = 1;                                   // Amount of damage to deal
	[SerializeField] bool destroyOnHit = true;                           // Whether to destroy this GameObject on hit
	[SerializeField] LayerMask damageableLayers = Physics.AllLayers;     // Layers that can be damaged
	[SerializeField] GameObject hitFxPrefab = null;                      // Visual effect to spawn on hit
	[SerializeField] private float damageRate = 0.1f;                    // Minimum time between damage ticks

	private float lastDamageTime;    // Track when damage was last dealt for rate limiting

	// Handle collision-based damage
	private void OnCollisionEnter(Collision collision)
	{
		// Early exit if target is not on a damageable layer
		if (!OnDamageLayer(collision.gameObject)) return;

		// Try to get the damageable component from the hit object
		if (collision.gameObject.TryGetComponent(out IDamagable component))
		{
			// Create damage info packet
			var damageInfo = new DamageInfo
			{
				amount = damage,
				inflictor = gameObject,
				hitPoint = collision.GetContact(0).point,
				hitDirection = collision.GetContact(0).normal
			};

			// Apply damage and handle effects
			component.ApplyDamage(damageInfo);

			// Spawn hit effect if one is set
			if (hitFxPrefab != null) Instantiate(hitFxPrefab, damageInfo.hitPoint, Quaternion.identity);

			// Destroy this object if configured to do so
			if (destroyOnHit)
			{
				Destroy(gameObject);
			}
		}
	}

	// Handle initial trigger-based damage
	private void OnTriggerEnter(Collider other)
	{
		// Early exit if target is not on a damageable layer
		if (!OnDamageLayer(other.gameObject)) return;

		// Try to get the damageable component from the hit object
		if (other.gameObject.TryGetComponent(out IDamagable component))
		{
			// Create damage info packet
			var damageInfo = new DamageInfo
			{
				amount = damage,
				inflictor = gameObject,
				hitPoint = other.ClosestPoint(transform.position),
				hitDirection = (other.transform.position - transform.position).normalized
			};

			// Apply damage and handle effects
			component.ApplyDamage(damageInfo);

			// Spawn hit effect if one is set
			if (hitFxPrefab != null) Instantiate(hitFxPrefab, damageInfo.hitPoint, Quaternion.identity);

			// Destroy this object if configured to do so
			if (destroyOnHit)
			{
				Destroy(gameObject);
			}

			lastDamageTime = Time.time;
		}
	}

	// Handle continuous trigger-based damage
	private void OnTriggerStay(Collider other)
	{
		// Early exit if damage rate limiting is in effect or target is not on damageable layer
		if (Time.time < lastDamageTime + damageRate) return;
		if (!OnDamageLayer(other.gameObject)) return;

		// Try to get the damageable component from the hit object
		if (other.gameObject.TryGetComponent(out IDamagable component))
		{
			// Create damage info packet
			var damageInfo = new DamageInfo
			{
				amount = damage,
				inflictor = gameObject,
				hitPoint = other.ClosestPoint(transform.position),
				hitDirection = (other.transform.position - transform.position).normalized
			};

			// Apply damage and update last damage time
			component.ApplyDamage(damageInfo);
			lastDamageTime = Time.time;
		}
	}

	// Check if target GameObject is on a layer that can be damaged
	private bool OnDamageLayer(GameObject target)
	{
		return (damageableLayers.value & (1 << target.layer)) != 0;
	}
}