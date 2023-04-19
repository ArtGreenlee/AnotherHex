using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BoidEnemyMovement : Movement
{
    public Transform bodyTransform;
    private Transform playerTransform;
    public float turningSpeed;
    Vector3 rotatedVectorToTarget;
    public Vector2 direction;
    Quaternion targetRotation;
    Quaternion orientation;
    public float moveSpeed;
    public LayerMask enemyMask;
    public int bufferSize;
    private Collider2D col;
    private Collider2D[] nearbyBuffer;
    public float nearbyBoidSearchRadius = 5;
    private Vector2 nearbyBoidCenter;
    private Vector2 nearbyBoidVelocity;
    private Vector2 avoidanceDirection;
    public float avoidanceWeight;
    public float cohesionWeight;
    public float alignmentWeight;
    public float seekPlayerWeight;
    public float raycastDistance;
    public LayerMask raycastMask;
    private float circleCastRadius;
    private float currentCircleCastRadius;
    int numNearby;
    RaycastHit2D hit;
    bool avoidanceMode = false;
    float flip;

    private List<Vector2> raycastDirections = new List<Vector2>();


    private IEnumerator disableBoidMovement(float seconds)
    {
        float temp = moveSpeed;
        moveSpeed = 0;
        yield return new WaitForSeconds(seconds);
        moveSpeed = temp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            direction *= -1;
            bodyTransform.rotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: (Vector2)transform.position - collision.ClosestPoint(transform.position));

        }
    }

    public override void movementOnCollision()
    {
        
    }

    private void Start()
    {
        if (Random.value > .5f)
        {
            flip = 1;
        }
        else
        {
            flip = -1;
        }
        for (float angle = 0; angle < 360; angle += 20)
        {
            Vector2 pos = Vector2.zero;
            pos.x = Mathf.Cos(angle / (180f / Mathf.PI));
            pos.y = Mathf.Sin(angle / (180f / Mathf.PI));
            raycastDirections.Add(pos);
        }
        
        circleCastRadius = GetComponent<CircleCollider2D>().radius;
        col = GetComponent<Collider2D>();
        nearbyBuffer = new Collider2D[bufferSize];
        orientation = Quaternion.Euler(0, 0, 0);
        playerTransform = Player.player.transform;
    }

    public override void move()
    {
        numNearby = Physics2D.OverlapCircleNonAlloc(transform.position, nearbyBoidSearchRadius, nearbyBuffer, enemyMask);
        nearbyBoidCenter = Vector3.zero;
        nearbyBoidVelocity = Vector2.zero;

        for (int i = 0; i < numNearby; i++)
        {
            if (nearbyBuffer[i] != col && Vector3.Dot(rb.velocity, (nearbyBuffer[i].transform.position - transform.position).normalized) > 0)
            {
                nearbyBoidVelocity += nearbyBuffer[i].GetComponent<Rigidbody2D>().velocity;
                nearbyBoidCenter += (Vector2)nearbyBuffer[i].transform.position;
                avoidanceDirection += (Vector2)(transform.position - nearbyBuffer[i].transform.position);
            }
        }

        nearbyBoidCenter /= numNearby;
        nearbyBoidVelocity /= numNearby;
        avoidanceDirection /= numNearby;

        direction = (Vector2)(playerTransform.position - transform.position).normalized * seekPlayerWeight +
            avoidanceDirection.normalized * avoidanceWeight + 
            nearbyBoidCenter.normalized * cohesionWeight + 
            nearbyBoidVelocity.normalized * alignmentWeight;


        hit = Physics2D.CircleCast(transform.position, circleCastRadius * 2, rb.velocity, raycastDistance, raycastMask);
        RaycastHit2D hit2 = Physics2D.CircleCast(transform.position, circleCastRadius * 2, direction, raycastDistance, raycastMask);
        int counter = 0;
        
        if (hit || hit2)
        {
            Vector2 avoidance = Vector2.zero;
            avoidance = rb.velocity;
            while (hit)
            {
                hit = Physics2D.CircleCast(transform.position, circleCastRadius * 2, avoidance, raycastDistance, raycastMask);
                counter++;
                avoidance = Quaternion.Euler(0, 0, 10 * counter * flip) * avoidance;
            }
            //Debug.DrawRay(transform.position, avoidance, Color.red, 1);
            direction += avoidance.normalized * 100;
            /*direction = rb.velocity;
            foreach (Vector2 dir in raycastDirections) 
            {
                direction = bodyTransform.TransformDirection(dir);
                hit = Physics2D.CircleCast(transform.position, circleCastRadius, direction, raycastDistance, raycastMask);
                //.DrawRay(transform.position, direction * 5, Color.red, 10);

            }
            direction += (hit.point - (Vector2)transform.position).normalized;*/
        }

        rotatedVectorToTarget = orientation * direction;
        targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);
        bodyTransform.rotation = Quaternion.RotateTowards(bodyTransform.rotation, targetRotation, turningSpeed * Time.deltaTime);
        rb.velocity = bodyTransform.up * movespeedScaler * moveSpeed;
        
    }
}
