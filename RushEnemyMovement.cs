using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushEnemyMovement : Movement
{
    public float rushDelay;
    public float rushMagnitude;
    public float rushDistanceTriggerThreshold;
    public float movespeed;
    private float rushDistanceTriggerSqrd;
    Transform playerTransform;
    private Transform target;
    Rigidbody2D rb;
    private bool rushing = false;
    public float rushCooldown;
    private float rushCooldownTimer = 0;
    private LineRendererObjectPool lineRendererPool;
    LineRenderer rushDirectionIndicator;
    private float checkTimer;
    private RangeHelper rangeHelper;
    private Enemy enemy;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        rangeHelper = GetComponent<RangeHelper>();
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rushDistanceTriggerSqrd = rushDistanceTriggerThreshold * rushDistanceTriggerThreshold;
        lineRendererPool = LineRendererObjectPool.lineRendererObjectPool;
        playerTransform = Player.player.transform;
    }

    public override void move()
    {
        if (Time.time > checkTimer)
        {
            checkTimer = Time.time + 1;
            target = rangeHelper.getNearestTransform();     
        }
        if (!rushing && target != null)
        {
            Vector3 diff = target.position - transform.position;
            rb.velocity = diff.normalized * movespeed * movespeedScaler;
            if (Time.time > rushCooldownTimer && diff.sqrMagnitude <= rushDistanceTriggerSqrd)
            {
                rushing = true;
                StartCoroutine(rush(diff.normalized));
            }
        }
    }

    private void OnDestroy()
    {
        if (rushDirectionIndicator != null)
        {
            lineRendererPool.Pool.Release(rushDirectionIndicator);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (rushing && collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent<Health>(out Health h))
        {
            Debug.Log("rush enemy knockback");
            StopAllCoroutines();
            if (rushDirectionIndicator != null)
            {
                lineRendererPool.Pool.Release(rushDirectionIndicator);
            }
            rushCooldownTimer = Time.time + rushCooldown;
            rb.AddForce((transform.position - collision.transform.position).normalized * 15, ForceMode2D.Impulse);
            rushing = false;
        }
    }

    public IEnumerator rush(Vector3 direction)
    {
        rushing = true;
        rb.velocity = Vector3.zero;
        rushDirectionIndicator = lineRendererPool.Pool.Get();
        rushDirectionIndicator.positionCount = 2;
        rushDirectionIndicator.startColor = Color.red;
        rushDirectionIndicator.endColor = Color.red;
        rushDirectionIndicator.SetPosition(0, transform.position);
        rushDirectionIndicator.SetPosition(1, transform.position + direction.normalized * 4);
        yield return new WaitForSeconds(rushDelay);
        lineRendererPool.Pool.Release(rushDirectionIndicator);
        rushDirectionIndicator = null;
        rb.AddForce(direction.normalized * rushMagnitude, ForceMode2D.Impulse);
        yield return new WaitForSeconds(rushCooldown);
        rushing = false;
    }
}
