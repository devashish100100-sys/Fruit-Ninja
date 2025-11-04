using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Object Pooler")]
    [SerializeField] ObjectPooler targetsPooler;

    [Header("Spawn Option")]
    [SerializeField] SpawnOption spawnOption = SpawnOption.RandomWave;
    [Space]
    [SerializeField, Min(0)] int minTargetsInAllAtOnce = 3;
    [SerializeField, Min(0)] int maxTargetsInAllAtOnce = 6;
    [Space]
    [SerializeField, Min(0)] int minTargetsInOneByOne = 3;
    [SerializeField, Min(0)] int maxTargetsInOneByOne = 8;

    [Header("Spawn Rate In Seconds")]
    [SerializeField, Min(0)] float minSpawnRateInAllAtOnce = 1f;
    [SerializeField, Min(0)] float maxSpawnRateInAllAtOnce = 4f;
    [Space]
    [SerializeField, Min(0)] float minSpawnRateInOneByOne = 0.05f;
    [SerializeField, Min(0)] float maxSpawnRateInOneByOne = 1.5f;
    [Space]
    [SerializeField, Min(0)] float minSpawnRateInOneAtOnce = 2f;
    [SerializeField, Min(0)] float maxSpawnRateInOneAtOnce = 8f;

    [Header("Forces Values Applied to Objects at Spawn")]
    [SerializeField, Min(0)] float maxVerticalForce = 14f;
    [SerializeField, Min(0)] float minVerticalForce = 8f;
    [Space]
    [SerializeField, Min(0)] float maxHorizontalForce = 5f;
    [Space]
    [SerializeField, Min(0)] float maxTorque = 10f;

    GameManager gameManager;

    // ✅ Cached list of indices that are allowed to spawn
    List<int> allowedPoolIndices = new List<int>();

    void Start()
    {
        gameManager = GameManager.Instance;
        CacheAllowedPools(); // ✅ Cache the prefabs marked as CanSpawn = true
        StartCoroutine(SelectSpawnOption());
    }

    // ✅ Build a list of all pool indices that can actually spawn
    void CacheAllowedPools()
    {
        allowedPoolIndices.Clear();

        for (int i = 0; i < targetsPooler.PoolSize(); i++)
        {
            var pooledObj = targetsPooler.GetPooledObjectAt(i);
            if (pooledObj.CanSpawn)
                allowedPoolIndices.Add(i);
        }

        if (allowedPoolIndices.Count == 0)
        {
            Debug.LogWarning("No prefabs marked as spawnable in ObjectPooler!");
        }
    }

    IEnumerator SelectSpawnOption()
    {
        while (true)
        {
            SpawnOption randomSpawnOption = spawnOption;
            if (spawnOption == SpawnOption.RandomWave)
                randomSpawnOption = spawnOption.RandowmWave();

            Coroutine selectedCoroutine;

            if (randomSpawnOption == SpawnOption.WaveAllAtOnce)
                selectedCoroutine = StartCoroutine(SpawnWaveAllAtOnce());
            else if (randomSpawnOption == SpawnOption.WaveOneByOne)
                selectedCoroutine = StartCoroutine(SpawnWaveOneByOne());
            else
                selectedCoroutine = StartCoroutine(SpawnOneAtOnce());

            yield return selectedCoroutine; // Wait for chosen coroutine to finish
        }
    }

    IEnumerator SpawnWaveAllAtOnce()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnRateInAllAtOnce, maxSpawnRateInAllAtOnce));

        if (gameManager.State == State.Playing && allowedPoolIndices.Count > 0)
        {
            int numberOfTargetsToSpawn = Random.Range(minTargetsInAllAtOnce, maxTargetsInAllAtOnce);
            Vector3 randomVerticalForce = RandomVerticalForce();

            for (int i = 0; i < numberOfTargetsToSpawn; i++)
            {
                int poolIndex = allowedPoolIndices[Random.Range(0, allowedPoolIndices.Count)];
                Target newTarget = targetsPooler.GetFromPool(poolIndex);

                newTarget.ResetForces();
                newTarget.AddForce(randomVerticalForce);
                newTarget.AddRandomHorizontalForce(maxHorizontalForce);
                newTarget.AddRandomTorque(maxTorque);
            }
        }
    }

    IEnumerator SpawnWaveOneByOne()
    {
        int numberOfTargetsToSpawn = Random.Range(minTargetsInOneByOne, maxTargetsInOneByOne);

        for (int i = 0; i < numberOfTargetsToSpawn; i++)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnRateInOneByOne, maxSpawnRateInOneByOne));

            if (gameManager.State == State.Playing && allowedPoolIndices.Count > 0)
            {
                int poolIndex = allowedPoolIndices[Random.Range(0, allowedPoolIndices.Count)];
                Target newTarget = targetsPooler.GetFromPool(poolIndex);

                newTarget.ResetForces();
                newTarget.AddRandomForce(minVerticalForce, maxVerticalForce, maxHorizontalForce);
                newTarget.AddRandomTorque(maxTorque);
            }
        }
    }

    IEnumerator SpawnOneAtOnce()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnRateInOneAtOnce, maxSpawnRateInOneAtOnce));

        if (gameManager.State == State.Playing && allowedPoolIndices.Count > 0)
        {
            int poolIndex = allowedPoolIndices[Random.Range(0, allowedPoolIndices.Count)];
            Target newTarget = targetsPooler.GetFromPool(poolIndex);

            newTarget.ResetForces();
            newTarget.AddRandomForce(minVerticalForce, maxVerticalForce, maxHorizontalForce);
            newTarget.AddRandomTorque(maxTorque);
        }
    }

    Vector3 RandomVerticalForce() => Vector3.up * Random.Range(minVerticalForce, maxVerticalForce);
}
