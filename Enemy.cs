using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity 
{
    [SerializeField] float collisionDamage;
    public EnemySpawnHandler spawnHandler;
    public Helper.enemyType enemyType;  
    public List<GameObject> drops;
    LightningParticleSystemManager lightningParticleSystemManager;
    
    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        lightningParticleSystemManager = LightningParticleSystemManager.instance;
        spawnHandler = EnemySpawnHandler.enemySpawnHandler;
        lightningParticleSystemManager.registerEnemy(this);
        base.Start();
    }
    public float getCollisionDamage()
    {
        return collisionDamage;
    }

    private void OnDestroy()
    {
        if (spawnHandler != null)
        {
            spawnHandler.registerEnemyDeath(enemyType, this);
        }
    }

    public override void OnDeath()
    {
        foreach (GameObject drop in drops)
        {
            if (drop.GetComponent<Rigidbody2D>() != null)
            {
                Vector3 randomOffset = Random.insideUnitSphere / 2;
                randomOffset.z = 0;
                GameObject newDrop = Instantiate(drop, transform.position + randomOffset, Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 180f))));
                Rigidbody2D rb = newDrop.GetComponent<Rigidbody2D>();
                rb.AddForce(randomOffset * 4, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-500, 500));
            }
            else
            {
                Instantiate(drop, transform.position, Quaternion.identity);
            }   
        }
        lightningParticleSystemManager.unregisterEnemy(this);
        base.OnDeath();
    }
}
