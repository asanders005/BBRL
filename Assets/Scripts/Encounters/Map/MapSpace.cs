using UnityEngine;

public class MapSpace : MonoBehaviour
{
    [SerializeField] private MapSpace nextSpaceLeft;
    [SerializeField] private MapSpace nextSpaceRight;

    [SerializeField] private EncounterType encounterType = EncounterType.Map;
    private string encounterName;

    public EncounterType EncounterType { get => encounterType; }
    public string EncounterName { get => encounterName; }

    public MapSpace GetNextSpace(bool isLeft)
    {
        return isLeft ? nextSpaceLeft : nextSpaceRight;
    }

    private void Awake()
    {
        var encounterManager = FindAnyObjectByType<EncounterManager>();
        if (encounterManager != null)
        {
            // encounterType = (EncounterType)Random.Range(1, (int)EncounterType.Boss); -- Use when all encounters are made

            if (encounterType == EncounterType.Map)
            {
                float randomValue = Random.value;
                encounterType = (randomValue < 0.65f) ? EncounterType.Combat : EncounterType.Event;

                var spriteRenderer = GetComponent<SpriteRenderer>();
                var spriteDistributor = FindAnyObjectByType<MapSpaceSpriteDistributor>();
                if (spriteRenderer != null && spriteDistributor != null)
                {
                    spriteRenderer.sprite = spriteDistributor.GetSprite(encounterType);
                }
                else
                {
                    Debug.LogError("SpriteRenderer or MapSpaceSpriteDistributor not found in the scene.");
                }
            }

            encounterName = encounterManager.GetRandomEncounter(encounterType);
        }
        else
        {
            Debug.LogError("EncounterManager not found in the scene.");
        }
    }
}
