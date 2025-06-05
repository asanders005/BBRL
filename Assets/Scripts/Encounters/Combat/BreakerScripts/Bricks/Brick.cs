using UnityEngine;
using UnityEngine.Events;

public class Brick : MonoBehaviour
{
    [SerializeField] private UnityEvent onHitEvent;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with a projectile
        if (collision.gameObject.CompareTag("Breaker"))
        {
            onHitEvent?.Invoke();

            Destroy(gameObject);
        }
    }
}
