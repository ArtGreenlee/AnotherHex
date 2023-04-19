using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealer : Enemy
{
    LineRendererObjectPool lineRendererObjectPool;
    public float healingTickDuration;
    public int maxEnemies;
    public Color healingIndicatorColor;
    private float enemySearchTimer = 0;
    HashSet<Health> healingEnemies;
    public float healDuration;
    private EnemyCirclingMovement movement;
    private HashSet<LineRenderer> healingIndicators = new HashSet<LineRenderer>();
    public override void Awake()
    {
        base.Awake();
        movement = GetComponent<EnemyCirclingMovement>();
        health = GetComponent<Health>();
        healingEnemies = new HashSet<Health>();
    }
    // Start is called before the first frame update
    public override void Start()
    {
        lineRendererObjectPool = LineRendererObjectPool.lineRendererObjectPool;
    }

    private void OnDestroy()
    {
        foreach (LineRenderer lr in healingIndicators)
        {
            lineRendererObjectPool.Pool.Release(lr);
        }
    }

    // Update is called once per frame
    void Update()
    {
        enemySearchTimer = Time.time + getSpeed();
        Health enemyHealth = rangeHelper.getNearestEntity().health;
        if (enemyHealth != null)
        {
            movement.rotateTransform = enemyHealth.transform;
            healingEnemies.Add(enemyHealth);
            StartCoroutine(healEnemy(enemyHealth, healDuration, getMagnitude()));
        }
    }

    private IEnumerator healEnemy(Health enemyHealth, float duration, float amount)
    {
        LineRenderer healingIndicator = lineRendererObjectPool.Pool.Get();
        healingIndicators.Add(healingIndicator);
        healingIndicator.startColor = healingIndicatorColor;
        healingIndicator.endColor = healingIndicatorColor;
        healingIndicator.positionCount = 2;
        Transform enemyTransform = enemyHealth.transform;
        float numTicks = duration / healingTickDuration;
        float tickAmount = amount / numTicks;
        float healingRadiusSqrd = getRange() * getRange();
        for (int i = 0; i < numTicks; i++)
        {
            if (enemyHealth == null)
            {
                break;
            }
            enemyHealth.changeHealth(tickAmount);
            float timer = 0;
            while (timer < healingTickDuration && enemyTransform != null)
            {
                timer += Time.deltaTime;
                healingIndicator.SetPosition(0, transform.position);
                healingIndicator.SetPosition(1, enemyTransform.position);
                yield return new WaitForEndOfFrame();
            }
            if (gameObject.activeSelf && 
                (enemyHealth.transform.position - transform.position).sqrMagnitude < healingRadiusSqrd)
            {
                break;
            }
        }
        healingIndicators.Remove(healingIndicator);
        lineRendererObjectPool.Pool.Release(healingIndicator);
        healingEnemies.Remove(enemyHealth);
    }
}
