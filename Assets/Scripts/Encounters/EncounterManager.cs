using System.Linq;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private CombatEncounterData[] combatEncounters;
    [SerializeField] private EventEncounterData[] eventEncounters;

    public string GetRandomEncounter(EncounterType type)
    {
        switch (type)
        {
            case EncounterType.Map:
                break;
            case EncounterType.Combat:
                return combatEncounters[Random.Range(0, combatEncounters.Length)].encounterName;
            case EncounterType.Rest:
                break;
            case EncounterType.Treasure:
                break;
            case EncounterType.Event:
                return eventEncounters[Random.Range(0, eventEncounters.Length)].encounterName;
            case EncounterType.Shop:
                break;
            case EncounterType.Boss:
                break;
        }

        return "";
    }

    public EncounterData GetEncounter(EncounterType type, string encounterName)
    {
        EncounterData encounter = null;

        switch (type)
        {
            case EncounterType.Map:
                break;
            case EncounterType.Combat:
                encounter = combatEncounters.FirstOrDefault(encounter => encounter.encounterName == encounterName);
                break;
            case EncounterType.Rest:
                break;
            case EncounterType.Treasure:
                break;
            case EncounterType.Event:
                encounter = eventEncounters.FirstOrDefault(encounter => encounter.encounterName == encounterName);
                break;
            case EncounterType.Shop:
                break;
            case EncounterType.Boss:
                break;
            default:
                break;
        }

        if (encounter == null)
        {
            Debug.LogError($"Encounter '{encounterName}' not found.");
            return null;
        }
        return encounter;
    }
}
