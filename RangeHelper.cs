using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
public class RangeHelperHelper : MonoBehaviour
{
    public RangeHelper rangeHelper;
    public Action<Collider2D> triggerEnterHandler;
    public Action<Collider2D> triggerExitHandler;
    private Entity e;
    private void Update()
    {
        transform.position = rangeHelper.transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        rangeHelper.debugList.Add(collision.gameObject);
        if (!rangeHelper.objects.Contains(collision))
        {
            if (triggerEnterHandler != null)
            {
                triggerEnterHandler(collision);
            }
            if (rangeHelper.storeEntities &&
                collision.TryGetComponent<Entity>(out e))
            {
                rangeHelper.entitiesInRange.Add(e);
            }
            if (rangeHelper.storeTransforms)
            {
                rangeHelper.setNearestTransform();
                rangeHelper.transformsInRange.Add(collision.transform);
            }
            rangeHelper.objects.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        rangeHelper.debugList.Remove(collision.gameObject);
        if (rangeHelper.objects.Contains(collision))
        {
            if (triggerExitHandler != null)
            {
                triggerExitHandler(collision);
            }
            if (rangeHelper.storeEntities &&
                collision.TryGetComponent<Entity>(out e))
            {
                rangeHelper.entitiesInRange.Remove(e);
            }
            if (rangeHelper.storeTransforms)
            {
                rangeHelper.setNearestTransform();
                rangeHelper.transformsInRange.Remove(collision.transform);
            }
            rangeHelper.objects.Remove(collision);
        }
    }
}
public class RangeHelper : MonoBehaviour
{
    public List<GameObject> debugList = new List<GameObject>();

    public HashSet<Collider2D> objects = new HashSet<Collider2D>();
    public HashSet<Entity> entitiesInRange = new HashSet<Entity>();
    public HashSet<Transform> transformsInRange = new HashSet<Transform>();
    private Entity nearestEntity;
    CircleCollider2D circleCollider;
    private GameObject helperObject;
    public string layerMask;
    private Transform nearestTransform;
    public bool storeEntities;
    public bool storeTransforms;
    public bool consistentNearestEntity;
    public bool consistentNearestTransform;
    public float nearestCheckCooldown;
    private float nearestCheckTimer = 0;
    private float curDistance;
    private float minDistance;
    public List<Action<Transform>> onNewNearestActions = new List<Action<Transform>>();

    private void Awake()
    {
        helperObject = new GameObject(gameObject.name + "\'s RangeHelper");
        helperObject.transform.localPosition = Vector3.zero;
        helperObject.transform.rotation = Quaternion.identity;
        helperObject.AddComponent<CircleCollider2D>();
        helperObject.layer = LayerMask.NameToLayer(layerMask);
        circleCollider = helperObject.GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        helperObject.AddComponent<RangeHelperHelper>().rangeHelper = this;
    }

    private void Update()
    {
        if (consistentNearestEntity || consistentNearestTransform)
        {
            if (Time.time > nearestCheckTimer)
            {
                nearestCheckTimer = Time.time + nearestCheckCooldown;
                if (consistentNearestEntity)
                {
                    setNearestEntity();
                }
                else
                {
                    setNearestTransform();
                }
            }
        }
    }

    public void setEnterCallbackFunction(Action<Collider2D> enterCallback)
    {
        helperObject.GetComponent<RangeHelperHelper>().triggerEnterHandler = enterCallback;
    }

    public void setExitCallbackFunction(Action<Collider2D> exitCallback)
    {
        helperObject.GetComponent<RangeHelperHelper>().triggerExitHandler = exitCallback;
    }

    public void setNearestEntity()
    {
        if (storeTransforms && !storeEntities && consistentNearestTransform)
        {
            nearestTransform = nearestEntity.transform;
        }
        minDistance = float.MaxValue;
        nearestEntity = null;
        foreach (Entity e in entitiesInRange)
        {
            curDistance = (e.transform.position - transform.position).sqrMagnitude;
            if (curDistance < minDistance)
            {
                minDistance = curDistance;
                nearestEntity = e;
            }
        }
        
    }

    public void setNearestTransform()
    {
        Transform temp = nearestTransform;
        if (!storeTransforms && storeEntities && consistentNearestEntity)
        {
            nearestTransform = nearestEntity.transform;
            return;
        }
        minDistance = float.MaxValue;
        nearestTransform = null;
        foreach (Transform t in transformsInRange)
        {
            if (t == null) continue;
            curDistance = (t.position - transform.position).sqrMagnitude;
            if (curDistance < minDistance)
            {
                minDistance = curDistance;
                nearestTransform = t;
            }
        }
        if (nearestTransform != temp && nearestTransform != null)
        {
            foreach (Action<Transform> onNewNearestAction in onNewNearestActions)
            {
                //Debug.Log(gameObject.name + "calling new nearest Transform:" + nearestTransform.gameObject.name);
                onNewNearestAction(nearestTransform);
            }
        }
    }

    private void OnDestroy()
    {
        if (helperObject != null)
        {
            Destroy(helperObject);
        }
    }

    public Transform getNearestTransform()
    {
        if (consistentNearestTransform && storeTransforms)
        {
            return nearestTransform;
        }
        setNearestTransform();
        return nearestTransform;
    }

    public Entity getNearestEntity()
    {
        if (consistentNearestEntity && storeEntities)
        {
            return nearestEntity;
        }
        setNearestEntity();
        return nearestEntity;
    }

    public void setRange(float range)
    {
        circleCollider.radius = range;
    }
}
