using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LineRendererObjectPool : MonoBehaviour
{
    public static LineRendererObjectPool lineRendererObjectPool;
    private void Awake()
    {
        if (lineRendererObjectPool == null)
        {
            lineRendererObjectPool = this;
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
    public IObjectPool<LineRenderer> lineRendererPool;

    public GameObject lineRendererObject;

    public IObjectPool<LineRenderer> Pool
    {
        get
        {
            if (lineRendererPool == null)
            {
                if (poolType == PoolType.Stack)
                    lineRendererPool = new ObjectPool<LineRenderer>(CreatePooledLineRenderer, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
                else
                    lineRendererPool = new LinkedPool<LineRenderer>(CreatePooledLineRenderer, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, maxPoolSize);
            }
            return lineRendererPool;
        }
    }

    LineRenderer CreatePooledLineRenderer()
    {
        GameObject newLineRenderer = Instantiate(lineRendererObject, transform);
        return newLineRenderer.GetComponent<LineRenderer>();
    }

    void OnReturnedToPool(LineRenderer lineRenderer)
    {
        lineRenderer.gameObject.SetActive(false);
    }

    void OnTakeFromPool(LineRenderer lineRenderer)
    {
        lineRenderer.gameObject.SetActive(true);
    }

    void OnDestroyPoolObject(LineRenderer lineRenderer)
    {
        Destroy(lineRenderer.gameObject);
    }
}
