using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemyPrefab;
        public float weight;
    }
    
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float spawnInterval;
        public float enemySpeedMultiplier;
        public float enemyHealthMultiplier;
        public List<EnemyType> enemyTypes;
    }

    [SerializeField] private List<Wave> waves;
    [SerializeField] private float timeBetweenWaves = 5f;

    private List<Vector3[]> pathPositions;
    private int currentWave = 0;
    private int enemiesSpawned = 0;
    [SerializeField] private Testing testingScript;
    private bool isInitialized = false;

    private void Start()
    {
        testingScript = FindObjectOfType<Testing>();
        //pathPositions = testingScript.GetPathPositions();
        //StartCoroutine(SpawnWaves());
    }
    public void Initialize(List<Vector3[]> path)
    {
        pathPositions = path;
        testingScript = FindObjectOfType<Testing>();
        isInitialized = true;
        StartCoroutine(SpawnWaves());
    }
    private IEnumerator SpawnWaves()
    {
        if (!isInitialized)
        {
            Debug.LogError("EnemySpawner is not initialized. Call Initialize() before spawning waves.");
            yield break;
        }

        while (currentWave < waves.Count)
        {
            yield return StartCoroutine(SpawnWave(waves[currentWave]));
            yield return new WaitForSeconds(timeBetweenWaves);
            currentWave++;
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private void SpawnEnemy(Wave wave)
    {
        System.Random random = new System.Random();
        if (wave.enemyTypes.Count > 0 && pathPositions.Count > 0)
        {
            int randomIndex = random.Next(pathPositions.Count);
            GameObject enemyPrefab = ChooseEnemyType(wave.enemyTypes);
            Enemy enemy = Enemy.Create(pathPositions[randomIndex][0], enemyPrefab.name);
            Debug.Log(enemyPrefab.name);
            if (enemy != null)
            {
                enemy.SetPath(pathPositions[randomIndex]);
                enemy.SetProperties(wave.enemySpeedMultiplier, wave.enemyHealthMultiplier);
                enemiesSpawned++;
            }
        }
    }
    private GameObject ChooseEnemyType(List<EnemyType> enemyTypes)
    {
        float totalWeight = 0f;
        foreach (EnemyType enemyType in enemyTypes)
        {
            totalWeight += enemyType.weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float weightSum = 0f;

        foreach (EnemyType enemyType in enemyTypes)
        {
            weightSum += enemyType.weight;
            if (randomValue <= weightSum)
            {
                return enemyType.enemyPrefab;
            }
        }

        return enemyTypes[0].enemyPrefab; // Fallback to first enemy type
    }
    public int GetEnemiesSpawned()
    {
        return enemiesSpawned;
    }
}