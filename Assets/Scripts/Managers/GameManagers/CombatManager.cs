using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class CombatManager : SceneManagerBase
{
    [Header("Battleground Settings")]
    [SerializeField] private GameObject[] enemyLocations;
    [SerializeField] private GameObject selectedEnemyOverlay;

    private int enemySpawnLocation;
    private Dictionary<int, GameObject> enemyLocationsStatus;

    [Header("Battleground Events")]
    [SerializeField] private Event onPlayerDeath;

    [SerializeField] private GameObjectEvent onEnemyTurn;
    [SerializeField] private GameObjectIntEvent onEnemyMove;
    [SerializeField] private GameObjectIntEvent onEnemyDamage;
    [SerializeField] private GameObjectEvent onEnemyDestroyed;

    [Header("Breaker Settings")]
    [SerializeField] private GameObjectCollectionData breakerInventory; // TODO: Update this to be a ScriptableObject for management in other encounters
    [SerializeField] private GameObject cursor;
    [SerializeField] private Collider2D breakerBounds;
    [SerializeField] private int maxBreakerDiscard = 1;

    [Header("Breaker Events")]
    [SerializeField] private Vector2Event onBreakerFire;
    [SerializeField] private GameObjectIntEvent onBreakerDestroyed;
    [SerializeField] private BoolEvent paddleToggleEvent;

    [Header("UI Settings")]
    [SerializeField] private Canvas canvas;

    [Header("UI Events")]
    [SerializeField] private Event onBreakerQueueUpdate;
    [SerializeField] private Event onBreakerDiscardUpdate;

    [Header("Game Over Events")]
    [SerializeField] private StringEvent onGameOver;
    [SerializeField] private StringData gameOverSceneName;

    private Transform breakerSpawn;
    private Queue<GameObject> breakerQueue = new Queue<GameObject>();
    public Queue<GameObject> BreakerQueue { get => new Queue<GameObject>(breakerQueue); }

    private bool isPlayerTurn = true;
    private bool breakerShot = false;
    private bool reloading = false;

    private bool breakerDiscarded = false;
    private int discardCount = 0;

    public int DiscardCount { get => discardCount; }
    public int MaxDiscardCount { get => maxBreakerDiscard; }

    private GameObject[] brickTypes;
    private Vector3[] brickLocations;

    private List<GameObject> specialEffects = new List<GameObject>();

    private GameObject[] enemies;
    private int spawnedEnemyCount = 0;
    private int enemyCount = 0;
    private int enemyTimer = 0;
    private int maxEnemyTimer = 0;

    private GameObject selectedEnemy;

    override protected void OnEnable()
    {
        base.OnEnable();

        onBreakerDestroyed.Subscribe(Attack);

        onPlayerDeath.Subscribe(OnPlayerDeath);
        onEnemyMove.Subscribe(OnEnemyMove);
        onEnemyDestroyed.Subscribe(OnEnemyDestroyed);
    }

    override protected void OnDisable()
    {
        base.OnDisable();

        onBreakerDestroyed.Unsubscribe(Attack);

        onPlayerDeath.Unsubscribe(OnPlayerDeath);
        onEnemyMove.Unsubscribe(OnEnemyMove);
        onEnemyDestroyed.Unsubscribe(OnEnemyDestroyed);
    }

    override protected void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (!isActive) return;

        // Update the cursor position to match the mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (selectedEnemy != null) selectedEnemyOverlay.transform.position = selectedEnemy.transform.position;
        else selectedEnemyOverlay.SetActive(false);

        if (BoundsContainsPoint(breakerBounds.bounds, mousePosition))
        {
            cursor.transform.position = mousePosition;
        }
    }

    // Because of how scene loading works, this gets called before Start()
    override protected void OnEncounterStart(EncounterType type, string encounterName)
    {
        if (type != EncounterType.Combat)
        {
            return;
        }

        enemySpawnLocation = enemyLocations.Length - 1;

        if (enemyLocationsStatus == null)
        {
            enemyLocationsStatus = new Dictionary<int, GameObject>();
            for (int i = 0; i < enemyLocations.Length; i++)
            {
                enemyLocationsStatus[i] = null;
            }
        }

        breakerSpawn = transform;

        this.encounterName = encounterName;

        var encounter = (CombatEncounterData)encounterManager.GetEncounter(type, encounterName);

        // Initialize the breaker queue with the inventory
        breakerQueue.Clear();
        Reload();

        enemies = new GameObject[encounter.enemies.Length];
        for (int i = 0; i < encounter.enemies.Length; i++)
        {
            enemies[i] = Instantiate(encounter.enemies[i], new Vector3(-500, -500, -2), Quaternion.identity);
            enemies[i].SetActive(false); // Initially deactivate all enemies
        }

        spawnedEnemyCount = 0;
        enemyCount = 0;

        if (enemySpawnLocation == 0)
        {
            enemySpawnLocation = enemyLocations.Length - 1; // Ensure the spawn location is valid
        }

        for (int i = 0; i < encounter.startingEnemyCount; i++)
        {
            if (i < enemies.Length)
            {
                SpawnEnemy();
            }
            else
            {
                Debug.LogWarning("Not enough enemies defined for starting count.");
            }
        }

        isPlayerTurn = encounter.isPlayerTurn;
        breakerSpawn.position = encounter.breakerSpawn;
        maxEnemyTimer = encounter.enemySpawnTimer;
        enemyTimer = maxEnemyTimer;
        brickTypes = encounter.brickTypes;
        brickLocations = encounter.brickLocations;

        spawnBricks();

        if (isPlayerTurn)
        {
            SpawnBreaker();
            cursor.SetActive(true);
            breakerShot = false;
        }

        if (encounter.SpecialEffects != null && encounter.SpecialEffects.Length > 0)
        {
            for(int i = 0; i < encounter.SpecialEffects.Length; i++)
            {
                var effectInstance = Instantiate(encounter.SpecialEffects[i], Vector3.zero, Quaternion.identity);
                effectInstance.transform.position = encounter.specialEffectLocations[i];
                specialEffects.Add(effectInstance);
            }
        }

        isActive = true;
    }

    private void spawnBricks()
    {
        foreach (var brick in brickLocations)
        {
            var brickType = brickTypes[Random.Range(0, brickTypes.Length)];

            Instantiate(brickType, brick, Quaternion.identity);
        }
    }

    override protected void OnEncounterEnd()
    {
        isActive = false;

        // Clean up the combat encounter and reset the state
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        enemies = null;
        selectedEnemy = null;

        breakerQueue.Clear();
        var bricks = FindObjectsByType<Brick>(FindObjectsSortMode.None);
        foreach (var brick in bricks)
        {
            Destroy(brick.gameObject);
        }

        foreach (var effect in specialEffects)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }
        specialEffects.Clear();

        ReturnToMap();
    }

    private void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        if (isPlayerTurn)
        {
            if (breakerQueue.Count == 0)
            {
                StartCoroutine(ReloadCoroutine());
            }
            else
            {
                var bricks = FindObjectsByType<Brick>(FindObjectsSortMode.None);
                if (bricks.Length == 0)
                {
                    spawnBricks();
                }

                SpawnBreaker();

                breakerShot = false;
                cursor.SetActive(true);
            }
        }
        else
        {
            EnemyTurn();
        }
    }

    private void EnemyTurn()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                onEnemyTurn.RaiseEvent(enemy);
            }
        }

        enemyTimer--;
        if (spawnedEnemyCount < enemies.Length && (enemyCount <= 0 || (enemyTimer <= 0 && enemyLocationsStatus[enemySpawnLocation] == null)))
        {
            enemyTimer = maxEnemyTimer;
            SpawnEnemy();
        }

        StartCoroutine(ChangeTurnCoroutine());
    }

    private void OnEnemyMove(GameObject enemy, int newLocation)
    {
        if (enemy == null) return;
        var enemyIndex = System.Array.IndexOf(enemies, enemy);
        if (enemyIndex >= 0 && enemyIndex < enemies.Length)
        {
            foreach (var location in enemyLocationsStatus)
            {
                if (location.Value == enemy)
                {
                    enemyLocationsStatus[location.Key] = null; // Clear the old location
                    break;
                }
            }

            if (!enemyLocationsStatus[newLocation])
            {
                enemyLocationsStatus[newLocation] = enemy;
                enemies[enemyIndex].transform.position = enemyLocations[newLocation].transform.position;
            }
            else
            {
                for (int i = newLocation + 1; i < enemyLocations.Length; i++)
                {
                    if (enemyLocationsStatus[i] == null)
                    {
                        enemyLocationsStatus[i] = enemy; // Move to the next available location
                        enemies[enemyIndex].transform.position = enemyLocations[i].transform.position;

                        enemy.GetComponent<EnemyBase>().CurrentLocation = i; // Update the enemy's current location
                        return; // Exit after moving to the next available location
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Enemy not found in the list: " + enemy.name);
        }
    }

    private void SpawnEnemy()
    {
        if (spawnedEnemyCount >= enemies.Length)
        {
            return;
        }

        var nextEnemy = enemies[spawnedEnemyCount++];

        if (nextEnemy == null)
        {
            return;
        }

        nextEnemy.SetActive(true);
        nextEnemy.transform.position = enemyLocations[enemySpawnLocation].transform.position;
        nextEnemy.GetComponent<EnemyBase>().CurrentLocation = enemySpawnLocation; // Set the enemy's current location
        enemyLocationsStatus[enemySpawnLocation] = nextEnemy;

        if (selectedEnemy == null)
        {
            selectedEnemy = nextEnemy; // Set the selected enemy if none is selected
            selectedEnemyOverlay.SetActive(true);
        }

        enemyCount++;
    }

    private void OnEnemyDestroyed(GameObject enemy)
    {
        if (!isActive) return;

        enemyCount--;
        var enemyIndex = System.Array.IndexOf(enemies, enemy);

        if (enemyIndex >= 0 && enemyIndex < enemies.Length)
        {
            enemyLocationsStatus[enemy.GetComponent<EnemyBase>().CurrentLocation] = null; // Clear the enemy's location
            enemies[enemyIndex] = null; // Mark the enemy as destroyed
        }
        else
        {
            Debug.LogError("Enemy not found in the list: " + enemy.name);
        }

        if (enemyCount <= 0 && spawnedEnemyCount >= enemies.Length)
        {
            Debug.Log("All enemies defeated.");
            OnEncounterEnd();
        }

        if (enemy == selectedEnemy)
        {
            selectedEnemy = enemyLocationsStatus.FirstOrDefault(e => e.Value != null && e.Value.activeInHierarchy).Value;
        }
    }

    private void OnPlayerDeath()
    {
        isActive = false;
        onGameOver.RaiseEvent(gameOverSceneName.Value);
    }

    private void SpawnBreaker()
    {
        var breaker = breakerQueue.Dequeue();
        Instantiate(breaker, breakerSpawn.position, Quaternion.identity);
        onBreakerQueueUpdate.RaiseEvent();
    }

    private void DiscardBreaker()
    {
        var discardedBreaker = FindAnyObjectByType<BreakerBase>();
        Destroy(discardedBreaker.gameObject);

        discardCount--;
        onBreakerDiscardUpdate.RaiseEvent();

        if (breakerQueue.Count > 0)
        {
            SpawnBreaker();
        }
        else
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    public void OnFire()
    {
        if (IsPointerOverUIButton(Input.mousePosition, canvas))
        {
            return;
        }

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        for (int i = 0; i < enemyLocations.Length; i++)
        {
            if (enemyLocationsStatus[i] == null) continue;

            var spriteRenderer = enemyLocations[i].GetComponent<SpriteRenderer>();
            if (BoundsContainsPoint(spriteRenderer.bounds, mousePosition))
            {
                selectedEnemy = enemyLocationsStatus[i];
                return;
            }
        }

        if (isPlayerTurn && !breakerShot && !reloading && !breakerDiscarded)
        {
            Vector2 direction = cursor.transform.position - breakerSpawn.transform.position;
            onBreakerFire.RaiseEvent(direction);
            breakerShot = true;

            paddleToggleEvent.RaiseEvent(true);
            cursor.SetActive(false);
        }
    }

    public void OnDiscard()
    {
        // Debug Cheat --- Remove mid-flight destruction for gameplay
        if (isPlayerTurn && breakerShot)
        {
            var breaker = FindAnyObjectByType<BreakerBase>();
            if (breaker != null)
            {
                Destroy(breaker.gameObject);
            }
        }
        else if (isPlayerTurn && !breakerShot && !reloading && discardCount > 0)
        {
            StartCoroutine(DiscardCoroutine());
        }
    }

    private void Attack(GameObject breakerObj, int damage)
    {
        paddleToggleEvent.RaiseEvent(false);

        var breaker = breakerObj.GetComponent<BreakerBase>();
        List<GameObject> hitEnemies = new List<GameObject>();

        GameObject targetEnemy = null;

        if (breaker.AttackType == BreakerBase.BreakerType.Projectile || breaker.AttackType == BreakerBase.BreakerType.Piercing)
        {
            targetEnemy = enemyLocationsStatus.FirstOrDefault(e => e.Value != null && e.Value.activeInHierarchy).Value;
        }
        else
        {
            targetEnemy = selectedEnemy;
        }

        if (targetEnemy == null)
        {
            Debug.LogWarning("No target enemy found for the breaker attack.");
            return;
        }

        hitEnemies.Add(targetEnemy);
        switch (breaker.AttackType)
        {
            case BreakerBase.BreakerType.Piercing:
                hitEnemies.Add(targetEnemy);
                int targetCount = 0;
                int currentLocation = targetEnemy.GetComponent<EnemyBase>().CurrentLocation;
                while (targetCount < breaker.AdjacencyCount && currentLocation + 1 < enemyLocations.Length)
                {
                    if (enemyLocationsStatus[++currentLocation] != null && enemyLocationsStatus[currentLocation].activeInHierarchy)
                    {
                        hitEnemies.Add(enemyLocationsStatus[currentLocation]);
                        targetCount++;
                    }
                }
                break;
            case BreakerBase.BreakerType.AreaOfEffect:
                hitEnemies.Add(targetEnemy);
                int TilesHit = 0;
                int locationLeft = targetEnemy.GetComponent<EnemyBase>().CurrentLocation;
                int locationRight = locationLeft;
                while (TilesHit < breaker.AdjacencyCount && locationLeft - 1 >= 0 && locationRight + 1 < enemyLocations.Length)
                {
                    if (enemyLocationsStatus[--locationLeft] != null && enemyLocationsStatus[locationLeft].activeInHierarchy)
                    {
                        hitEnemies.Add(enemyLocationsStatus[locationLeft]);
                    }
                    if (enemyLocationsStatus[++locationRight] != null && enemyLocationsStatus[locationRight].activeInHierarchy)
                    {
                        hitEnemies.Add(enemyLocationsStatus[locationRight]);
                    }
                    TilesHit++;
                }
                break;
        }

        foreach (var enemy in hitEnemies)
        {
            onEnemyDamage.RaiseEvent(enemy, damage);
        }

        StartCoroutine(ChangeTurnCoroutine());
    }

    private void Reload(bool updateUI = true)
    {
        // Shuffle the breaker inventory before refilling the queue  
        System.Random random = new System.Random();
        GameObject[] shuffledInventory = breakerInventory.Value.ToArray();
        for (int i = shuffledInventory.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            var temp = shuffledInventory[i];
            shuffledInventory[i] = shuffledInventory[j];
            shuffledInventory[j] = temp;
        }

        // Refill the breaker queue from the shuffled inventory  
        foreach (var breaker in shuffledInventory)
        {
            breakerQueue.Enqueue(breaker);
        }

        discardCount = maxBreakerDiscard;

        if (updateUI) onBreakerQueueUpdate.RaiseEvent();
        onBreakerDiscardUpdate.RaiseEvent();
    }

    private IEnumerator ChangeTurnCoroutine()
    {
        yield return new WaitForSeconds(0.25f); // Simulate turn change delay
        ChangeTurn();
    }

    private IEnumerator ReloadCoroutine()
    {
        reloading = true;

        Reload();

        yield return new WaitForSeconds(2f); // Simulate reload time  
        reloading = false;

        StartCoroutine(ChangeTurnCoroutine());
    }

    private IEnumerator DiscardCoroutine()
    {
        breakerDiscarded = true;
        DiscardBreaker();

        yield return new WaitForSeconds(0.5f); // Simulate discard time
        breakerDiscarded = false;
    }

    private bool BoundsContainsPoint(Bounds bounds, Vector2 point)
    {
        return (bounds.min.x < point.x && bounds.min.y < point.y &&
                bounds.max.x > point.x && bounds.max.y > point.y);
    }

    public bool IsPointerOverUIButton(Vector2 screenPosition, Canvas canvas)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
                return true;
        }
        return false;
    }
}
