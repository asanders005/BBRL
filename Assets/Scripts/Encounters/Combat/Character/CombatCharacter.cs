using UnityEngine;
using UnityEngine.Events;

public class CombatCharacter : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private GameObjectIntEvent onCharacterDamage;
    [SerializeField] private GameObjectEvent onCharacterHealthUIUpdate;

    public int CurrentHealth { get => health; }
    public int MaxHealth { get => maxHealth; }

    private void OnEnable()
    {
        onCharacterDamage.Subscribe(onDamage);
    }

    private void OnDisable()
    {
        onCharacterDamage.Unsubscribe(onDamage);
    }

    public void onHeal(GameObject target, int healAmount)
    {
        if (gameObject != target)
        {
            return;
        }
        health = Mathf.Min(health + healAmount, maxHealth);
    }

    public void onDamage(GameObject target, int damage)
    {
        if (gameObject != target)
        {
            return;
        }

        health -= damage;
        if (health <= 0)
        {
            health = 0;
            onDeath();
        }
        else
        {
            onCharacterHealthUIUpdate.RaiseEvent(gameObject);
        }
    }

    private void onDeath()
    {
        Destroy(gameObject);
    }
}
