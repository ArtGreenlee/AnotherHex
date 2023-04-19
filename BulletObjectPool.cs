using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class BulletObjectPool : MonoBehaviour
{
    public static BulletObjectPool bulletObjectPool;
    private void Awake()
    {
        if (bulletObjectPool == null)
        {
            bulletObjectPool = this;
        }
    }

    public enum PoolType
    {
        Stack,  
        LinkedList
    }

    PoolType poolType = PoolType.Stack;
    public bool collectionChecks = true;
    public int maxPoolSize = 10;
    public IObjectPool<Bullet> bulletPool;  

    public GameObject bulletObject;

    public IObjectPool<Bullet> Pool
    {
        get
        {
            if (bulletPool == null)
            {
                if (poolType == PoolType.Stack)
                    bulletPool = new ObjectPool<Bullet>(CreatePooledBullet, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
                else
                    bulletPool = new LinkedPool<Bullet>(CreatePooledBullet, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, maxPoolSize);
            }
            return bulletPool;
        }
    }

    Bullet CreatePooledBullet()
    {
        GameObject newBullet = Instantiate(bulletObject, transform);
        return newBullet.GetComponent<Bullet>();
    }
    
    void OnReturnedToPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    void OnTakeFromPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    void OnDestroyPoolObject(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }
}
