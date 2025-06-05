using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : SceneManagerBase
{
    [Header("UI Elements")]
    [SerializeField] private GameObject[] options;
    [SerializeField] private Image[] optionImages;
    [SerializeField] private TMP_Text[] optionTitles;
    [SerializeField] private TMP_Text[] optionDamage;
    [SerializeField] private TMP_Text[] optionDescriptions;
    [SerializeField] private Button[] optionButtons;

    [Header("Events")]
    [SerializeField] private GameObjectEvent onAddBreaker;

    private GameObject[] eventRewards;

    private Vector2 screenSize;

    override protected void Start()
    {
        base.Start();
    }

    private void Update()
    {
        var screen = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
        if (screen != screenSize)
        {
            screenSize = screen;
            for (int i = 0; i < options.Length; i++)
            {
                options[i].GetComponent<RectTransform>().sizeDelta = new Vector2(screenSize.x * 0.3f, screenSize.y - 20);

                var imageRect = optionImages[i].GetComponent<RectTransform>();
                imageRect.sizeDelta = new Vector2(screenSize.y * 0.25f, screenSize.y * 0.25f);
                imageRect.anchoredPosition = new Vector2(0, (screenSize.y * -0.125f) - 20);

                var titleRect = optionTitles[i].GetComponent<RectTransform>();
                titleRect.sizeDelta = new Vector2(screenSize.x * 0.25f, 50);
                titleRect.anchoredPosition = new Vector2(0, (screenSize.y * -0.125f) - 10);

                optionDamage[i].GetComponent<RectTransform>().sizeDelta = new Vector2(screenSize.x * 0.25f, 50);
                optionDescriptions[i].GetComponent<RectTransform>().sizeDelta = new Vector2(screenSize.x * 0.25f, 50);

                optionButtons[i].GetComponent<RectTransform>().sizeDelta = new Vector2(screenSize.x * 0.25f, 75);
            }
        }
    }

    public void onOptionSelect(int optionNumber)
    {
        if (optionNumber < 0 || optionNumber >= options.Length)
        {
            Debug.LogError("Option selected outside of option range");
            return;
        }

        onAddBreaker.RaiseEvent(eventRewards[optionNumber]);

        OnEncounterEnd();
    }

    protected override void OnEncounterStart(EncounterType type, string name)
    {
        if (type != EncounterType.Event)
        {
            return;
        }

        Debug.Log($"Starting event encounter: {name}");

        var encounter = (EventEncounterData)encounterManager.GetEncounter(type, name);

        if (eventRewards == null)
        {
            eventRewards = new GameObject[encounter.rewardSelection.Length];
        }

        for (int i = 0; i < options.Length; i++)
        {
            eventRewards[i] = encounter.rewardSelection[i];
            if (eventRewards[i] != null)
            {
                optionImages[i].sprite = eventRewards[i].GetComponent<SpriteRenderer>().sprite;

                var breaker = eventRewards[i].GetComponent<BreakerBase>();
                optionTitles[i].text = breaker.breakerName;
                optionDamage[i].text = $"DMG: {breaker.Damage}";
                optionDescriptions[i].text = breaker.description;
            }
            else
            {
                Debug.LogWarning($"Event reward for option {i} is null.");
            }
        }
    }

    protected override void OnEncounterEnd()
    {
        ReturnToMap();
    }
}