using UnityEngine;

[CreateAssetMenu(menuName = "Data/Encounter/Event Encounter Data")]
public class EventEncounterData : EncounterData
{
    //[SerializeField, TextArea] public string eventDescription;

    //[SerializeField] public GameEventType eventType;

    [Header("Item Event")]
    //[SerializeField] private string[] optionSelect;
    [SerializeField] public GameObject[] rewardSelection;
}

public enum GameEventType
{
    GainItem,
    GainCurrency,
    Encounter,
}