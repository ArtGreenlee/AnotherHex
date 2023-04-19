using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class DamageTextObjectPool : MonoBehaviour
{
    public static DamageTextObjectPool damageTextObjectPool;
    private void Awake()
    {
        if (damageTextObjectPool == null)
        {
            damageTextObjectPool = this;
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
    public IObjectPool<DamageText> damageTextPool;
    public Transform canvasTransform;

    public GameObject damageTextObject;

    public IObjectPool<DamageText> Pool
    {
        get
        {
            if (damageTextPool == null)
            {
                if (poolType == PoolType.Stack)
                    damageTextPool = new ObjectPool<DamageText>(CreatePooledDamageText, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
                else
                    damageTextPool = new LinkedPool<DamageText>(CreatePooledDamageText, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, maxPoolSize);
            }
            return damageTextPool;
        }
    }

    DamageText CreatePooledDamageText()
    {
        GameObject newDamageText = Instantiate(damageTextObject, canvasTransform);
        return newDamageText.GetComponent<DamageText>();
    }

    void OnReturnedToPool(DamageText damageText)
    {
        damageText.gameObject.SetActive(false);
    }

    void OnTakeFromPool(DamageText damageText)
    {
        damageText.gameObject.SetActive(true);
    }

    void OnDestroyPoolObject(DamageText damageText)
    {
        Destroy(damageText.gameObject);
    }
}
