using UnityEngine;

public class BreakerInventory : MonoBehaviour
{
    [SerializeField] private GameObjectCollectionData breakerInventory;
    [SerializeField] private GameObjectEvent onAddBreaker;
    [SerializeField] private GameObjectEvent onRemoveBreaker;

    private void OnEnable()
    {
        onAddBreaker.Subscribe(AddBreaker);
        onRemoveBreaker.Subscribe(RemoveBreaker);
    }

    private void OnDisable()
    {
        onAddBreaker.Unsubscribe(AddBreaker);
        onRemoveBreaker.Unsubscribe(RemoveBreaker);
    }

    public void AddBreaker(GameObject breaker)
    {
        if (breaker == null) return;

        breakerInventory.AddGameObject(breaker);
    }

    public void RemoveBreaker(GameObject breaker)
    {
        if (breaker == null) return;
        breakerInventory.RemoveGameObject(breaker);
    }

    public void ClearInventory()
    {
        breakerInventory.Value.Clear();
    }
}
