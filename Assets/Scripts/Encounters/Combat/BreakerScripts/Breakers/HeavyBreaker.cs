using UnityEngine;

public class HeavyBreaker : BreakerBase
{
    [Header("Heavy Breaker Settings")]
    [SerializeField, Range(0, 1)] private float momentumPreservation = 0.5f; // How much speed is preserved when hitting a brick

    private float currentSpeed;
    private Vector2 currentDirection;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        currentSpeed = rb.linearVelocity.magnitude;
        currentDirection = rb.linearVelocity.normalized;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if ((collisionMask & (1 << collision.gameObject.layer)) != 0)
        {
            if (collision.gameObject.CompareTag("Brick"))
            {
                // Calculate the new speed after hitting a brick
                float newSpeed = currentSpeed * momentumPreservation;
                rb.linearVelocity = currentDirection * newSpeed;
            }
            //else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Paddle"))
            //{
            //    var collisionDirection = (Vector2)(collision.transform.position - transform.position).normalized;
            //    if (Mathf.Abs(collisionDirection.x) > Mathf.Abs(collisionDirection.y))
            //    {
            //        currentDirection.x = -currentDirection.x;
            //    }
            //    else
            //    {
            //        currentDirection.y = -currentDirection.y;
            //    }

            //    rb.linearVelocity = currentDirection * currentSpeed;
            //}
        }
    }
}
