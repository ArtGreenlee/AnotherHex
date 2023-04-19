using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircularEnemyMovement : Movement
{
    public float switchCooldownMax;
    public float switchCooldownMin;
    public float rotationDistanceMin;
    private float rotationDistanceMinSqrd;
    private float switchTimer = 0;
    public float circlingSpeed;
    private float rotationDirection = 1;
    private Vector2 straightDirection = Vector2.zero;
    private RangeHelper rangeHelper;
    private Transform target;
    public override void Awake()
    {
        rotationDistanceMinSqrd = rotationDistanceMin * rotationDistanceMin;
        base.Awake();
        rangeHelper = GetComponent<RangeHelper>();  
        
    }
    private void Start()
    {
        switchTimer = Time.time + Random.Range(1, 10);
        rangeHelper.onNewNearestActions.Add(newNearestTransformAction);
        if (Random.value > .5f)
        {
            rotationDirection *= -1;
        }
    }

    public void newNearestTransformAction(Transform newNearestTransform)
    {
        //Debug.Log(gameObject.name + "Recieved new nearest transform: ");
        //Debug.Log(newNearestTransform.gameObject.name);
        target = newNearestTransform;
    }


    public override void move()
    {
        if (target != null)
        {
            Vector2 diff = transform.position - target.position;
            if (diff.sqrMagnitude < rotationDistanceMinSqrd)
            {
                rb.AddForce(diff.normalized * .2f);
            }
            rb.AddForce(Vector2.Perpendicular(target.position - transform.position).normalized * circlingSpeed * rotationDirection * movespeedScaler);
        }
        if (Time.time > switchTimer)
        {
            rotationDirection *= -1;
            switchTimer = Time.time + Random.Range(switchCooldownMin, switchCooldownMax);
        }
        
    }
}
