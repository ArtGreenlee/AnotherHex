using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawnHandler : MonoBehaviour
{
    public static EnemySpawnHandler enemySpawnHandler;
    public enum spawnInfoType { maxAlive, minAlive, spawnDistanceSqrd }
    private Dictionary<Helper.enemyType, Dictionary<spawnInfoType, int>> spawnInfoDict = new Dictionary<Helper.enemyType, Dictionary<spawnInfoType, int>>()
    {
        { 
            Helper.enemyType.shield, new Dictionary<spawnInfoType, int>() {
                { spawnInfoType.maxAlive, 5 },
                { spawnInfoType.minAlive, 
                    0  
                },
            }
        },
        {
            Helper.enemyType.swarm, new Dictionary<spawnInfoType, int>() {
                { spawnInfoType.maxAlive, 10 },
                { spawnInfoType.minAlive,
                    0 
                },
            }
        },
        {
            Helper.enemyType.rush, new Dictionary<spawnInfoType, int>() {
                { spawnInfoType.maxAlive, 10 },
                { spawnInfoType.minAlive, 
                    0 
                },
            }
        },
        {
            Helper.enemyType.heal, new Dictionary<spawnInfoType, int>() {
                { spawnInfoType.maxAlive, 10 },
                { spawnInfoType.minAlive, 
                    0
                },
            }
        },
        {
            Helper.enemyType.dodge, new Dictionary<spawnInfoType, int>() {
                { spawnInfoType.maxAlive, 10 },
                { spawnInfoType.minAlive,
                    0 
                },
            }
        },
        {
            Helper.enemyType.gun, new Dictionary<spawnInfoType, int>() {
                { spawnInfoType.maxAlive, 10 },
                { spawnInfoType.minAlive, 
                    0 
                },
            }
        }
    };

    public GameObject spawnIndicatorObject;
    public Dictionary<Helper.enemyType, GameObject> enemyTypeObjects;
    private Field field;
    public float spawnDelay;
    Transform playerTransform;
    public List<GameObject> enemyObjects;
    private Dictionary<Helper.enemyType, int> numAlive;
    public float enemyCheckCooldown;
    private float enemyCheckCooldownTimer = 0;
    public int maximumSpawnsPerCheck;
    public float enemyMinSpawnDistanceSqrd;
    private HashSet<RangeHelper> rangeHelpers = new HashSet<RangeHelper>();

    public void registerRangeHelper(RangeHelper rangeHelper)
    {
        rangeHelpers.Add(rangeHelper);
    }

    public void unregisterRangeHelper(RangeHelper rangeHelper)
    {
        rangeHelpers.Remove(rangeHelper);
    }

    public void registerEnemyDeath(Helper.enemyType type, Enemy enemy)
    {
        List<RangeHelper> toRemove = null;
        foreach (RangeHelper r in rangeHelpers)
        {
            if (r == null)
            {
                if (toRemove == null)
                {
                    toRemove = new List<RangeHelper>();
                }
                toRemove.Add(r);
                continue;
            }
            if (r.objects.Contains(enemy.col))
            {
                r.objects.Remove(enemy.col);
            }
            if (r.storeEntities && r.entitiesInRange.Contains(enemy)) 
            {
                r.entitiesInRange.Remove(enemy);
            }
            if (r.storeTransforms && r.transformsInRange.Contains(enemy.transform))
            {
                r.transformsInRange.Remove(enemy.transform);
            }
        }
        if (toRemove != null)
        {
            foreach (RangeHelper r in toRemove)
            {
                rangeHelpers.Remove(r);
            }
        }
        numAlive[type]--;
    }
    private void Awake()
    {
        enemySpawnHandler = this;
    }

    void Start()
    {
        field = Field.field;
        playerTransform = Player.player.transform;
        numAlive = new Dictionary<Helper.enemyType, int>();
        enemyTypeObjects = new Dictionary<Helper.enemyType, GameObject>();
        foreach (Helper.enemyType type in Enum.GetValues(typeof(Helper.enemyType)))
        {
            numAlive[type] = 0;
        }
        foreach (GameObject enemyObject in enemyObjects)
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            enemyTypeObjects[enemy.enemyType] = enemyObject;
        }
    }

    private void Update()
    {
        if (Time.time > enemyCheckCooldownTimer)
        {
            enemyCheckCooldownTimer = Time.time + enemyCheckCooldown;
            foreach (Helper.enemyType type in Enum.GetValues(typeof(Helper.enemyType)))
            {
                Dictionary<spawnInfoType, int> spawnInfo = spawnInfoDict[type];
                int minAlive = spawnInfo[spawnInfoType.minAlive];
                int maxAlive = spawnInfo[spawnInfoType.maxAlive];
                if (numAlive[type] < minAlive)
                {
                    StartCoroutine(spawnEnemies(type, minAlive - numAlive[type]));
                }
            }
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

    private IEnumerator spawnEnemies(Helper.enemyType enemyType, int enemyCount)
    {
        numAlive[enemyType] += enemyCount;
        List<Vector3> spawnPositions = new List<Vector3>();
        List<GameObject> spawnIndicators = new List<GameObject>();
        for (int i = 0; i < enemyCount; i++)
        {
            //get random point around player;
            Vector3 playerPosition = playerTransform.position;
            Vector3 spawnPosition;
            do
            {
                spawnPosition = new Vector3(Random.Range(field.x_min + 2, field.x_max - 2),
                    Random.Range(field.y_min + 2, field.y_max - 2), 0);
            } while ((playerPosition - spawnPosition).sqrMagnitude < enemyMinSpawnDistanceSqrd ||
            pointInsideRangeOfList(spawnPositions, spawnPosition, 1));

            GameObject indicator = Instantiate(spawnIndicatorObject, spawnPosition, Quaternion.identity);
            spawnIndicators.Add(indicator);
            spawnPositions.Add(spawnPosition);
        }

        yield return new WaitForSeconds(spawnDelay);
        
        foreach (GameObject indicator in spawnIndicators)
        {
            Destroy(indicator);
        }

        foreach (Vector3 spawnPoint in spawnPositions)
        {
            Instantiate(enemyTypeObjects[enemyType], spawnPoint, Quaternion.identity);
        }
    }

    // Start is called before the first frame update
    
}
