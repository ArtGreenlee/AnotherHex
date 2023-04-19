using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GridBrushBase;

public class EnemyCirclingMovement : Movement
{
    public float enemySearchCooldown;
    public float rotateDistance;
    public float speed;
    private float rotationDirection = 1;
    public Transform rotateTransform;
    public float enemySearchRadius;
    private float rotateDistanceSqrd;
    RangeHelper rangeHelper;

    public override void Awake()
    {
        base.Awake();
        rangeHelper = GetComponent<RangeHelper>();
    }
    private void Start()
    {
        rangeHelper.setRange(enemySearchRadius);
        rotateDistanceSqrd = rotateDistance * rotateDistance;
        rotateTransform = Player.player.transform;
    }

    public override void move()
    {
        rotateTransform = rangeHelper.getNearestTransform();
        Vector3 diff = transform.position - rotateTransform.position;
        float distSqrd = diff.sqrMagnitude;
        if (distSqrd > rotateDistanceSqrd + 2)
        {
            transform.position -= diff.normalized * Time.deltaTime * distSqrd * 2 / rotateDistanceSqrd;
        }
        else if (distSqrd < rotateDistanceSqrd - 2)
        {
            transform.position += diff.normalized * Time.deltaTime * rotateDistanceSqrd / distSqrd;
        }
        else
        {
            var q = transform.rotation;
            transform.RotateAround(rotateTransform.position, Vector3.forward, (rotationDirection * speed * movespeedScaler / diff.magnitude) * Time.deltaTime);
            transform.rotation = q;
        }
    }
}
