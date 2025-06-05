using UnityEngine;

public class PiercingBreaker : BreakerBase
{
    [Header("Piercing Breaker Settings")]
    [SerializeField, Range(0, 5)] private int pierceCount = 1; // How many bricks to pierce through before stopping

    private float currentSpeed;
    private Vector2 currentDirection;
    private int piercedBricksCount = 0;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        currentSpeed = rb.linearVelocity.magnitude;
        currentDirection = rb.linearVelocity.normalized;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Brick"))
        {
            if (piercedBricksCount < pierceCount)
            {
                piercedBricksCount++;

                // Calculate the new speed after hitting a brick
                float newSpeed = currentSpeed;
                rb.linearVelocity = currentDirection * newSpeed;
            }
        }
    }
}
