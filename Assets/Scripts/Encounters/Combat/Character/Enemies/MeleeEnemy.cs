using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    protected override void OnEnemyTurn(GameObject enemy)
    {
        if (enemy == gameObject)
        {
            if (CurrentLocation > 0)
            {
                MoveEnemy();
            }
            else
            {
                OnAttack();
            }
        }
    }
}
