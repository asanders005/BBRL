using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [SerializeField] private EncounterLoadEvent onEncounterLoad;

    [Header("Scene Names")]
    [SerializeField] private StringData mapSceneName;
    [SerializeField] private StringData combatSceneName;
    [SerializeField] private StringData eventSceneName;

    public void LoadEncounter(EncounterType type, string name)
    {
        switch (type)
        {
            case EncounterType.Map:
                onEncounterLoad.RaiseEvent(mapSceneName.Value, name);
                break;
            case EncounterType.Combat:
                onEncounterLoad.RaiseEvent(combatSceneName.Value, name);
                break;
            case EncounterType.Rest:
                break;
            case EncounterType.Treasure:
                break;
            case EncounterType.Event:
                onEncounterLoad.RaiseEvent(eventSceneName.Value, name);
                break;
            case EncounterType.Shop:
                break;
            case EncounterType.Boss:
                break;
            default:
                break;
        }
    }
}
