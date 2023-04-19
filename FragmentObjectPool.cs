using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FragmentObjectPool : MonoBehaviour
{
    public static FragmentObjectPool fragmentObjectPool;
    private void Awake()
    {
        if (fragmentObjectPool == null)
        {
            fragmentObjectPool = this;
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
    public IObjectPool<Fragment> fragmentPool;

    public GameObject fragmentObject;

    public IObjectPool<Fragment> Pool
    {
        get
        {
            if (fragmentPool == null)
            {
                if (poolType == PoolType.Stack)
                    fragmentPool = new ObjectPool<Fragment>(CreatePooledFragment, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
                else
                    fragmentPool = new LinkedPool<Fragment>(CreatePooledFragment, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, maxPoolSize);
            }
            return fragmentPool;
        }
    }

    Fragment CreatePooledFragment()
    {
        GameObject newFragment = Instantiate(fragmentObject, transform);
        return newFragment.GetComponent<Fragment>();
    }

    void OnReturnedToPool(Fragment fragment)
    {
        fragment.gameObject.SetActive(false);
    }

    void OnTakeFromPool(Fragment fragment)
    {
        fragment.gameObject.SetActive(true);
    }

    void OnDestroyPoolObject(Fragment fragment)
    {
        Destroy(fragment.gameObject);
    }
}
