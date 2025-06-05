using UnityEngine;
using UnityEngine.Events;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private IntEvent onEnemyAttack;
    [SerializeField] private GameObjectIntEvent onCharacterDamage;
    [SerializeField] private Event onCharacterDeath;

    private void OnEnable()
    {
        onEnemyAttack?.Subscribe(onDamage);
    }
    private void OnDisable()
    {
        onEnemyAttack?.Unsubscribe(onDamage);
    }

    private void onDamage(int damage)
    {
        onCharacterDamage.RaiseEvent(gameObject, damage);
    }

    private void OnDestroy()
    {
        onEnemyAttack?.Unsubscribe(onDamage);
        onCharacterDeath.RaiseEvent();
    }
}
