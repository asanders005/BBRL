using System.Collections;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    [SerializeField] private EncounterLoadEvent onEncounterLoadEvent;
    [SerializeField] private StringData mapSceneName;

    [SerializeField] private BreakerInventory breakerInventory;
    [SerializeField] private GameObjectCollectionData startInventory;

    private void Start()
    {
        breakerInventory.ClearInventory();
        foreach (var breaker in startInventory.Value)
        {
            breakerInventory.AddBreaker(breaker);
        }

        StartCoroutine(LaunchCoroutine());
    }

    private IEnumerator LaunchCoroutine()
    {
        // Wait for any necessary initialization or loading
        yield return new WaitForSeconds(0.25f);
        // Trigger the encounter load event
        onEncounterLoadEvent.RaiseEvent(mapSceneName.Value, "Initial");
    }
}
