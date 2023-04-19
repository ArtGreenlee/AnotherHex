using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : Projectile
{
    BulletObjectPool bulletPool;
    public int numPierce;
    public bool pierceAll;
    public int pierceCounter;
    public float lifeTimeTimer; 

    private void Update()
    {
        if (Time.time > lifeTimeTimer)
        {
            bulletPool.Pool.Release(this);
        }
    }
    private void OnEnable()
    {
        pierceCounter = 0;
    }

    private void Start()
    {
        bulletPool = BulletObjectPool.bulletObjectPool;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.activeSelf && 
            (other.gameObject.CompareTag("Wall") ||
            (!pierceAll && pierceCounter == numPierce)))
        {
            bulletPool.Pool.Release(this);
        }
        if (!pierceAll)
        {
            pierceCounter++;
        }
    }
}
