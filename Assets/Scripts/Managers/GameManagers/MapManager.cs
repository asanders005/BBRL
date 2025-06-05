using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : SceneManagerBase
{
    [Header("Map Data")]
    [SerializeField] private MapSpace startingSpace;
    private MapSpace currentSpace;

    [SerializeField] private GameObject player;

    [Header("UI References")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Encounter Management")]
    [SerializeField] private MapLoader mapLoader;

    [Header("Game Management")]
    [SerializeField] private StringData gameWinSceneName;
    [SerializeField] private StringEvent onSceneLoad;

    private Vector2 currentScreenSize;

    protected override void Start()
    {
        currentSpace = startingSpace;
        player.transform.position = currentSpace.transform.position;

        base.Start();
    }

    private void Update()
    {
        Vector2 screenSize = (new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));

        if (screenSize != currentScreenSize)
        {
            currentScreenSize = screenSize;
            var mainLeftButton = leftButton.GetComponent<RectTransform>();
            mainLeftButton.sizeDelta = new Vector2(screenSize.x * 0.25f, screenSize.y * 0.3f);
            var mainRightButton = rightButton.GetComponent<RectTransform>();
            mainRightButton.sizeDelta = new Vector2(screenSize.x * 0.25f, screenSize.y * 0.3f);
        }
    }

    protected override void OnEncounterStart(EncounterType type, string name)
    {
        if (type != EncounterType.Map)
        {
            return;
        }

        if (name == "")
        {
            if (currentSpace.GetNextSpace(true) == null && currentSpace.GetNextSpace(false) == null)
            {
                OnEncounterEnd();
                return;
            }
            else
            {
                DisplayUI(true);
            }
        }
        else
        {
            StartCoroutine(StartEncounterCoroutine(startingSpace.EncounterType, startingSpace.EncounterName));
        }
    }

    protected override void OnEncounterEnd()
    {
        onSceneLoad.RaiseEvent(gameWinSceneName.Value);
    }

    public void MoveToSpace(bool isLeftSpace)
    {
        if (currentSpace == null)
        {
            Debug.LogError("Current space is not set.");
            return;
        }
        MapSpace nextSpace = currentSpace.GetNextSpace(isLeftSpace);
        if (nextSpace == null)
        {
            Debug.LogWarning("No next space found in the specified direction.");
            return;
        }

        DisplayUI(false);

        currentSpace = nextSpace;
        player.transform.position = currentSpace.transform.position;
        // Trigger encounter start event
        StartCoroutine(StartEncounterCoroutine(currentSpace.EncounterType, currentSpace.EncounterName));
    }

    private IEnumerator StartEncounterCoroutine(EncounterType type, string name)
    {
        yield return new WaitForSeconds(1f); // Simulate some delay before starting the encounter
        mapLoader.LoadEncounter(type, name);
    }

    private void DisplayUI(bool isActive)
    {
        if (isActive)
        {
            if (currentSpace.GetNextSpace(true) != null)
            {
                leftButton.gameObject.SetActive(true);
            }
            if (currentSpace.GetNextSpace(false) != null)
            {
                rightButton.gameObject.SetActive(true);
            }
        }
        else
        {
            leftButton.gameObject.SetActive(false);
            rightButton.gameObject.SetActive(false);
        }
    }
}
