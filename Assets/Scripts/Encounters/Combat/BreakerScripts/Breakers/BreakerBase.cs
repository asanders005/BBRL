using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class BreakerBase : MonoBehaviour
{
    [SerializeField] public string breakerName;
    [SerializeField, TextArea] public string description;

    [Header("Movement Settings")]
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected float maxSpeed = 10f;
    [SerializeField] protected float timeToMaxSpeed = 2f;
    [SerializeField] protected LayerMask collisionMask;

    [Header("Damage Settings")]
    [SerializeField] protected int damage = 1;
    [SerializeField] protected BreakerType breakerType = BreakerType.Projectile;
    [SerializeField, Tooltip("The amount of adjacent tiles to hit for piercing/AoE breakers")] protected int adjacencyCount = 0;

    public int Damage { get => damage; }
    public BreakerType AttackType { get => breakerType; }
    public int AdjacencyCount { get => adjacencyCount; }

    [Header("Events")]
    [SerializeField] protected Vector2Event onBreakerFire;
    [SerializeField] protected UnityEvent onBrickHit;
    [SerializeField] protected GameObjectIntEvent onBreakerDestroyed;

    protected Rigidbody2D rb;
    protected int hitCount = 0;

    private bool isFired = false;

    private float acceleration;

    private float despawnTimer = 120f;

    public enum BreakerType
    {
        Projectile,
        Piercing,
        Target,
        AreaOfEffect,
    }

    protected void OnEnable()
    {
        onBreakerFire.Subscribe(Fire);
    }

    protected void OnDisable()
    {
        onBreakerFire.Unsubscribe(Fire);
    }

    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
    }

    protected void Start()
    {
        acceleration = (maxSpeed - speed) / timeToMaxSpeed;
    }

    protected void Fire(Vector2 direction)
    {
        if (isFired) return;
        rb.bodyType = RigidbodyType2D.Dynamic;

        transform.up = direction.normalized;

        rb.linearVelocity = transform.up * speed;
        isFired = true;
    }

    private void Update()
    {
        despawnTimer -= Time.deltaTime;
        if (despawnTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (isFired)
        {
            onBreakerDestroyed.RaiseEvent(gameObject, hitCount * damage);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!isFired) return;

        Vector2 direction = rb.linearVelocity.normalized;
        // Apply acceleration to the ball's speed
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(direction * acceleration);
        }
        // Ensure the ball's speed does not exceed maxSpeed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = direction * maxSpeed;
        }

        // Rotate the ball based on its velocity direction
        

        transform.up = rb.linearVelocity;
        var rotation = transform.rotation;

        rotation.x = 0;
        rotation.y = 0;
        transform.rotation = rotation;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collisionMask & (1 << collision.gameObject.layer)) != 0)
        {
            if (collision.gameObject.CompareTag("Brick"))
            {
                hitCount++;
                onBrickHit?.Invoke();
            }
        }
        else
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
    }
}
