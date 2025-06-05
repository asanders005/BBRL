using UnityEngine;

[CreateAssetMenu(menuName = "Data/Encounter/Encounter Data")]
public class EncounterData : ScriptableObjectBase
{
    [SerializeField] public string encounterName;
    [SerializeField] public EncounterType encounterType;
}
