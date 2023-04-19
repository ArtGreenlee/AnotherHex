using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyMovement : Movement
{
    public Transform aggroIndicatorTransform;
    private SpriteRenderer aggroIndicatorSpriteRenderer;
    public Color deAggroColor;
    public Color aggroColor;
    public float randomMovementThreshold;
    private float randomMovementThresholdSqrd;
    public float randomMovementSpeedMin, randomMovementSpeedMax;
    public float randomMovementWaitMin, randomMovementWaitMax;
    public float towardsPlayerThreshold;
    private float towardsPlayerThresholdSqrd;
    public float towardsPlayerSpeed;
    private Transform playerTransform;
    private float playerDistanceSqrd;
    private float changeCheckTimer = 0;
    private Field field;
    public int maxDirectionCheckCount;
    private int checkCount;
    public LayerMask directionCheckLayerMask;
    private RaycastHit2D hit;
    public float directionCheckDistance;
    private Vector3 prevRandomDirection;
    private Vector3 nextRandomDirection;
    private float circleCastRadius;
    public float newAngleDifferenceThreshold;
    public float towardsPlayerCheckTimerMin, towardsPlayerCheckTimerMax;
    public float changeCheckTimerCooldownResting;
    private Vector3 playerDirection;
    private float directionSwitch = 1;

    public override void Awake()
    {
        base.Awake();
        randomMovementThresholdSqrd = randomMovementThreshold * randomMovementThreshold;
        towardsPlayerThresholdSqrd = towardsPlayerThreshold * towardsPlayerThreshold;
    }

    private void Start()
    {
        //aggroIndicatorSpriteRenderer = aggroIndicatorTransform.GetComponent<SpriteRenderer>();
        //aggroIndicatorSpriteRenderer.color = deAggroColor;
        playerTransform = Player.player.transform;
        circleCastRadius = transform.localScale.x;
    }

    /*private bool obstacleInDirection(Vector3 direction)
    {
        hit = Physics2D.CircleCast(transform.position, circleCastRadius, playerDirection, directionCheckDistance, directionCheckLayerMask);
        if (hit.collider.gameObject.)
    }*/

    public override void movementOnCollision()
    {
        base.movementOnCollision();
        directionSwitch *= -1;
        StartCoroutine(unswitchDirection(Random.Range(3, 7)));
    }

    private IEnumerator unswitchDirection(float delay)
    {
        yield return new WaitForSeconds(delay);
        directionSwitch = 1;
    }

    public override void move()
    {
        if (Time.time < changeCheckTimer)
        {
            return;   
        }
        playerDirection = playerTransform.position - transform.position;
        playerDistanceSqrd = playerDirection.sqrMagnitude;
        //aggroIndicatorSpriteRenderer.color = deAggroColor;
        if (playerDistanceSqrd > randomMovementThresholdSqrd)
        {
            rb.velocity = Vector3.zero;
            changeCheckTimer = Time.time + changeCheckTimerCooldownResting;
            return;
        }
        if (playerDistanceSqrd > towardsPlayerThresholdSqrd)
        {
            checkCount = 0;
            while (checkCount < maxDirectionCheckCount)
            {
                nextRandomDirection = Random.insideUnitCircle.normalized;
                hit = Physics2D.CircleCast(transform.position, circleCastRadius, nextRandomDirection, directionCheckDistance, directionCheckLayerMask);
                if (!hit)
                {
                    rb.AddForce(nextRandomDirection * Random.Range(randomMovementSpeedMin, randomMovementSpeedMax) * directionSwitch * movespeedScaler);
                    changeCheckTimer = Time.time + Random.Range(randomMovementWaitMin, randomMovementWaitMax);
                    return;
                }
                checkCount++;
            }
            Debug.LogError("golly gee fuck what do i do.");
        }
        //USE THE FIELD
        //aggroIndicatorSpriteRenderer.color = aggroColor;
        //aggroIndicatorTransform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(playerTransform.position.y, playerTransform.position.x));
        changeCheckTimer = Time.time + Random.Range(towardsPlayerCheckTimerMin, towardsPlayerCheckTimerMax);
        rb.AddForce(playerDirection.normalized * towardsPlayerSpeed * directionSwitch * movespeedScaler);
    }

}
