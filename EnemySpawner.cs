using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyObject;
    public GameObject spawnIndicator;
    public List<float> spawnTimes;
    public List<int> counts;
    private int wave = 0;
    private float timer;
    Transform playerTransform;
    public float minSpawnDistance;
    private float minSpawnDistanceSqrd;
    public float spawnDelay;
    public Field field;
    private void Start()
    {
        minSpawnDistanceSqrd = minSpawnDistance * minSpawnDistance;
        field = Field.field;
        playerTransform = Player.player.transform;  
        StartCoroutine(enemySpawnerRoutine());
    }

    private IEnumerator enemySpawnerRoutine()
    {
        for (int i = 0; i < spawnTimes.Count; i++)
        {
            yield return new WaitForSeconds(spawnTimes[i]);
            spawnEnemies(counts[i]);
            
        }
    }

    private bool pointInsideRangeOfList(List<Vector3> list, Vector3 point, float range)
    {
        float rangeSqrd = range * range;
        foreach (Vector3 test in list)
        {
            if ((test - point).sqrMagnitude < rangeSqrd)
            {
                return true;
            }
        }
        return false;
    }


    public virtual void spawnEnemies(int enemyCount)
    {
        Debug.Log("spawning: " + enemyCount.ToString() + " enemies");
        List<Vector3> spawnPositions = new List<Vector3>();
        for (int i = 0; i < enemyCount; i++)
        {
            //get random point around player;
            Vector3 playerPosition = playerTransform.position;
            Vector3 spawnPosition;
            int attempts = 0;
            do
            {
                attempts++;
                spawnPosition = new Vector3(Random.Range(field.x_min + 2, field.x_max - 2),
                    Random.Range(field.y_min + 2, field.y_max - 2), 0);
            } while (((playerPosition - spawnPosition).sqrMagnitude < minSpawnDistanceSqrd ||
            pointInsideRangeOfList(spawnPositions, spawnPosition, 1)) && attempts < 10);

            StartCoroutine(spawnEnemy(spawnPosition));
            spawnPositions.Add(spawnPosition);
        }
    }

    public virtual IEnumerator spawnEnemy(Vector3 spawnPosition)
    {
        GameObject s = Instantiate(spawnIndicator, spawnPosition, Quaternion.identity);
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(enemyObject, spawnPosition, Quaternion.identity);
        Destroy(s);
    }


}
