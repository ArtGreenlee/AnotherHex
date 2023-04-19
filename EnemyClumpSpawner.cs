using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClumpSpawner : EnemySpawner
{
    public float clumpRadius;
    public override void spawnEnemies(int enemyCount)
    {
        Vector2 spawnPoint = field.getRandomFreeSpaceOnField(clumpRadius);
        for (int i = 0; i < enemyCount; i++)
        {
            StartCoroutine(spawnEnemy(spawnPoint + Random.insideUnitCircle * clumpRadius));
        }
    }
}
