using UnityEngine;

[CreateAssetMenu(menuName = "Data/Encounter/Combat Encounter Data")]
public class CombatEncounterData : EncounterData
{
    [SerializeField] public GameObject[] enemies;
    [SerializeField] public int startingEnemyCount = 1;
    [SerializeField] public int enemySpawnTimer = 2;

    [SerializeField] public GameObject[] brickTypes;
    [SerializeField] public Vector3[] brickLocations;

    [SerializeField] public GameObject[] SpecialEffects;
    [SerializeField] public Vector3[] specialEffectLocations;

    [SerializeField] public Vector3 breakerSpawn;

    [SerializeField] public bool isPlayerTurn = true;

}
