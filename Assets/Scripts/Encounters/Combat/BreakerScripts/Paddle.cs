using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Paddle : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private BoolEvent paddleToggleEvent;
    [SerializeField] private Collider2D boundingBox;

    private bool isActive = false;
    private Collider2D paddleCollider;

    private void Awake()
    {
        paddleCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        paddleToggleEvent?.Subscribe(SetActive);
    }

    private void OnDisable()
    {
        paddleToggleEvent?.Unsubscribe(SetActive);
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 goalPosition = new Vector3(mousePosition.x, transform.position.y, transform.position.z);

        // Clamp the goal position to ensure it stays within the bounding box
        Bounds bounds = boundingBox.bounds;
        Bounds paddleBounds = paddleCollider.bounds;
        float halfBoundsWidth = paddleBounds.size.x * 0.5f;
        goalPosition.x = Mathf.Clamp(goalPosition.x, bounds.min.x + halfBoundsWidth, bounds.max.x - halfBoundsWidth);

        if (transform.position != goalPosition)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, goalPosition, speed * Time.deltaTime);
            transform.position = newPosition;
        }
    }

    private void SetActive(bool active)
    {
        isActive = active;
    }
}
