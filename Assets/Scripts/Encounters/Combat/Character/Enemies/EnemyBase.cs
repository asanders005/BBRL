using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] private int moveSpeed = 1;
    [SerializeField] protected int damage = 5;

    [Header("Events")]
    [SerializeField] private GameObjectEvent onEnemyTurn;
    [SerializeField] private GameObjectIntEvent onEnemyMove;
    [SerializeField] protected IntEvent onEnemyAttack;
    [SerializeField] private GameObjectEvent onEnemyDestroyed;

    public int CurrentLocation { get; set; }

    private void OnEnable()
    {
        onEnemyTurn.Subscribe(OnEnemyTurn);
    }

    private void OnDisable()
    {
        onEnemyTurn.Unsubscribe(OnEnemyTurn);
    }

    protected void MoveEnemy()
    {
        CurrentLocation -= moveSpeed;
        if (CurrentLocation < 0)
        {
            CurrentLocation = 0; // Prevent moving out of bounds
        }
        onEnemyMove.RaiseEvent(gameObject, CurrentLocation);
    }

    protected abstract void OnEnemyTurn(GameObject enemy);
    protected virtual void OnAttack()
    {
        onEnemyAttack.RaiseEvent(damage);
    }

    private void OnDestroy()
    {
        Debug.Log($"Enemy {gameObject.name} destroyed.");
        onEnemyTurn.Unsubscribe(OnEnemyTurn);
        onEnemyDestroyed.RaiseEvent(gameObject);
    }
}