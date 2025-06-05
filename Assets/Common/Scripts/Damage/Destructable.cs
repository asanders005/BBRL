using UnityEngine;
using UnityEngine.Events;

// Component that can take damage and be destroyed
public class Destructable : MonoBehaviour, IDamagable
{
	// Health configuration
	[SerializeField] float health = 100;			// Current health amount
	[SerializeField] float maxHealth = 100;			// Maximum possible health
	[SerializeField] GameObject destroyFxPrefab;	// Visual effect spawned on destruction
	[SerializeField] UnityEvent onDestroyed;		// Event to call on destruction

	bool destroyed = false;  // Track if object has been destroyed to prevent multiple destructions

	// Public properties to read health values
	public float Health => health;      // Current health
	public float MaxHealth => maxHealth; // Maximum health

	// Called when damage is applied to this object
	public void ApplyDamage(DamageInfo damage)
	{
		// Prevent damage if already destroyed
		if (destroyed) return;

		// Reduce health by damage amount
		health -= damage.amount;
		// Clamp health between 0 and max health
		health = Mathf.Clamp(health, 0, maxHealth);

		// Check if health is depleted
		if (health <= 0)
		{
			destroyed = true;

			// Call event when destroyed
			onDestroyed.Invoke();
			// Spawn destruction effect if one is set
			if (destroyFxPrefab != null) Instantiate(destroyFxPrefab, transform.position, Quaternion.identity);
			// Destroy this game object
			Destroy(gameObject);
		}
	}
}
