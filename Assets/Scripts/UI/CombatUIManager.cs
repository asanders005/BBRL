using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;

    [Header("Breaker UI")]
    [SerializeField] private GameObject breakerPanel;

    [SerializeField] private GameObject breakerDetailsPanel;
    [SerializeField] private Image breakerDetailsIcon;
    [SerializeField] private Sprite defaultBreakerIcon; // Default icon if no breaker is selected
    [SerializeField] private TMP_Text breakerDetailsDamage;
    [SerializeField] private TMP_Text breakerDetailsText;

    [SerializeField] private GameObject breakerQueuePanel;
    [SerializeField] private GameObject breakerQueueIconPrefab;

    [SerializeField] private GameObject breakerDiscardPanel;
    [SerializeField] private TMP_Text breakerDiscardText;
    [SerializeField] private Button breakerDiscardButton;

    [Header("Character UI")]
    [SerializeField] private GameObject characterPanel;
    [SerializeField] private Image characterIcon;

    [SerializeField] private Slider characterHealthSlider;
    [SerializeField] private TMP_Text characterHealth;
    [SerializeField] private TMP_Text characterMaxHealth;

    [Header("Events")]
    [SerializeField] private EncounterNameEvent onEncounterStart;
    [SerializeField] private Event onBreakerQueueUpdate;
    [SerializeField] private Event onBreakerDiscardUpdate;
    [SerializeField] private GameObjectEvent onCharacterHealthUpdate;

    private Queue<GameObject> breakerQueue = new Queue<GameObject>();
    private LinkedList<GameObject> breakerQueueIcons = new LinkedList<GameObject>();

    private Vector2 currentScreenSize;

    private void OnEnable()
    {
        onEncounterStart.Subscribe(OnEncounterStart);

        onBreakerQueueUpdate.Subscribe(UpdateBreakerQueue);
        onBreakerDiscardUpdate.Subscribe(UpdateBreakerDiscard);
        onCharacterHealthUpdate.Subscribe(onUpdateCharacterHealth);
    }

    private void OnDisable()
    {
        onEncounterStart.Unsubscribe(OnEncounterStart);

        onBreakerQueueUpdate.Unsubscribe(UpdateBreakerQueue);
        onBreakerDiscardUpdate.Unsubscribe(UpdateBreakerDiscard);
        onCharacterHealthUpdate.Unsubscribe(onUpdateCharacterHealth);
    }

    private void Update()
    {
        Vector2 screenSize = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

        if (screenSize != currentScreenSize)
        {
            currentScreenSize = screenSize;

            var mainBreakerPanel = breakerPanel.GetComponent<RectTransform>();
            mainBreakerPanel.sizeDelta = new Vector2(screenSize.x * 0.32f, screenSize.y * 0.6f);
            breakerDetailsPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(mainBreakerPanel.sizeDelta.x * 0.75f, mainBreakerPanel.sizeDelta.y * 0.65f);
            breakerQueuePanel.GetComponent<RectTransform>().sizeDelta = new Vector2(mainBreakerPanel.sizeDelta.x * 0.25f, mainBreakerPanel.sizeDelta.y);

            var discardPanel = breakerDiscardPanel.GetComponent<RectTransform>();
            discardPanel.sizeDelta = new Vector2(mainBreakerPanel.sizeDelta.x * 0.75f, mainBreakerPanel.sizeDelta.y * 0.35f);
            breakerDiscardButton.GetComponent<RectTransform>().sizeDelta = new Vector2(discardPanel.sizeDelta.x * 0.66f, 50);

            var mainCharacterPanel = characterPanel.GetComponent<RectTransform>();
            mainCharacterPanel.sizeDelta = new Vector2(screenSize.x * 0.32f, screenSize.y * 0.3f);
            characterIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(mainCharacterPanel.sizeDelta.x * 0.3f, mainCharacterPanel.sizeDelta.x * 0.3f);
            characterHealthSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(mainCharacterPanel.sizeDelta.x * 0.65f, mainCharacterPanel.sizeDelta.y * 0.15f);
        }
    }

    private void OnEncounterStart(EncounterType type, string name)
    {
        if (type != EncounterType.Combat)
        {
            return;
        }

        breakerQueue.Clear();
        breakerQueueIcons.Clear();
    }

    private void UpdateBreakerDiscard()
    {
        breakerDiscardText.text = $"{combatManager.DiscardCount}/{combatManager.MaxDiscardCount}";
    }

    private void UpdateBreakerQueue()
    {
        BreakerBase breaker = null;

        if (breakerQueue.Count < 1)
        {
            breakerQueue = combatManager.BreakerQueue;
            var breakers = breakerQueue.ToArray();

            foreach (var b in breakers)
            {
                breakerQueueIcons.AddLast(Instantiate(breakerQueueIconPrefab, breakerQueuePanel.transform));
                if (breakerQueueIcons.Last.Value.TryGetComponent<Image>(out var icon))
                {
                    icon.sprite = b.GetComponent<SpriteRenderer>().sprite;
                }
                else
                {
                    Debug.LogError($"Breaker icon {breakerQueueIcons.Last.Value} does not have an Image component.");
                }
            }
        }
        else
        {
            breaker = breakerQueue.Dequeue().GetComponent<BreakerBase>();

            Destroy(breakerQueueIcons.First.Value);
            breakerQueueIcons.RemoveFirst();
        }

        // Update breaker queue panel
        var breakerQueueTransform = breakerQueuePanel.GetComponent<RectTransform>();

        Vector2 currentLocation;
        currentLocation.x = breakerQueueTransform.position.x - (breakerQueueTransform.sizeDelta.x * 0.5f);
        currentLocation.y = breakerQueueTransform.position.y + ((breakerQueueTransform.sizeDelta.y * 0.5f) - 10);

        foreach (var i in breakerQueueIcons)
        {
            if (i.TryGetComponent<RectTransform>(out var icon))
            {
                icon.transform.position = currentLocation;
                currentLocation.y -= icon.sizeDelta.y;
            }
            else
            {
                Debug.LogError($"Breaker icon {icon} does not have a RectTransform component.");
            }
        }

        // Update breaker details panel
        breakerDetailsIcon.sprite = breaker?.GetComponent<SpriteRenderer>().sprite ?? defaultBreakerIcon;
        breakerDetailsDamage.text = (breaker != null) ? $"DMG: {breaker.Damage.ToString()}" : "DMG: --";
        breakerDetailsText.text = breaker?.description ?? "No active breaker";
    }

    private void onUpdateCharacterHealth(GameObject character)
    {
        var characterComponent = character.GetComponent<CombatCharacter>();
        var enemyComponent = character.GetComponent<EnemyBase>();

        if (enemyComponent == null)
        {
            characterHealthSlider.value = (float)characterComponent.CurrentHealth / characterComponent.MaxHealth;
            characterHealth.text = characterComponent.CurrentHealth.ToString();
            characterMaxHealth.text = characterComponent.MaxHealth.ToString();
        }
    }
}
