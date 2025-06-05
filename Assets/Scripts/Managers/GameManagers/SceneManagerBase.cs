using UnityEngine;

public abstract class SceneManagerBase : MonoBehaviour
{
    protected string encounterName;
    protected EncounterManager encounterManager;
    protected bool isActive = false;

    [Header("Encounter Events")]
    [SerializeField] protected EncounterNameEvent onEncounterStart;
    [SerializeField] private EncounterLoadEvent onEncounterLoad;

    protected virtual void OnEnable()
    {
        onEncounterStart.Subscribe(OnEncounterStart);
    }

    protected virtual void OnDisable()
    {
        onEncounterStart.Unsubscribe(OnEncounterStart);
    }

    private void Awake()
    {
        encounterManager = FindAnyObjectByType<EncounterManager>();
        if (encounterManager == null)
        {
            Debug.LogError("EncounterManager not found in the scene.");
        }
    }

    protected virtual void Start()
    {
        //if (!string.IsNullOrEmpty(encounterName))
        //{
        //    onEncounterStart.RaiseEvent(EncounterType.Combat, encounterName);
        //}
    }

    protected abstract void OnEncounterStart(EncounterType type, string name);
    protected abstract void OnEncounterEnd();

    protected void ReturnToMap()
    {
        onEncounterLoad.RaiseEvent("Map", string.Empty);
    }
}
