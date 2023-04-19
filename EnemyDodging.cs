using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDodging : MonoBehaviour
{
    private Movement movement;
    public float dodgeCheckCooldown;
    private float dodgeTimer = 0;
    public float dodgeCheckRadius;
    public float numConsecutiveDodges;
    private Rigidbody2D rb;
    public float dodgeSpeed;
    public float dodgeStutterTime;
    public float dodgeCooldown;
    public bool dodging = false;
    public LayerMask bulletMask;
    public LayerMask enemyMask;
    private Color ogColor;
    public float dodgeCount = 0;
    public float dodgeCountDecaySpeed;
    private float dodgeCountDecayTimer = 0;
    public float vulnerableTime;
    public float movementWaitTime;
    public SpriteRenderer sr;
    private bool vulnerable = false;
    public float dodgeScoreConfirmThreshold;
    private TextMeshProUGUI dodgeCounter;
    private float colliderRadius;

    private void Awake()
    {
        colliderRadius = GetComponent<CircleCollider2D>().radius;
        dodgeCounter = GetComponentInChildren<TextMeshProUGUI>();
        dodgeCounter.text = numConsecutiveDodges.ToString();
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
        }
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
        ogColor = sr.color;
    }

    private void Update()
    {
        if (!dodging && Time.time > dodgeTimer)
        {
            dodgeTimer = Time.time + dodgeCheckCooldown;
            checkForDodges();
        }
        if (!vulnerable)
        {
            dodgeCounter.text = (numConsecutiveDodges - dodgeCount + 1).ToString();
        }
        if (dodgeCount > 0 && Time.time > dodgeCountDecayTimer)
        {
            dodgeCountDecayTimer = Time.time + dodgeCountDecaySpeed;
            dodgeCount--;
        }
        if (!vulnerable && dodgeCount == 0)
        {
            movement.enableMovement();
        }
    }
    

    public void checkForDodges()
    {
        Collider2D[] bulletColliders = Physics2D.OverlapCircleAll(transform.position, dodgeCheckRadius, bulletMask);
        float rand = Random.value;
        Vector2 dodgeDirection = Vector2.zero;
        bool confirm = false;
        //get the closest and fastest bullet

        float dodgeScore = float.MinValue;
        Vector2 bulletToDodgeVelocity = Vector2.zero;
        foreach (var collider in bulletColliders)
        {
            Vector2 bulletVelocity = collider.attachedRigidbody.velocity;
            Vector3 bulletPosition = collider.transform.position;
            float bulletRadius = collider.gameObject.GetComponent<CircleCollider2D>().radius;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(bulletPosition, bulletRadius, bulletVelocity, 2f, enemyMask);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    /*Debug.Log("position score");
                    Debug.Log(20 / (transform.position - bulletPosition).sqrMagnitude);
                    Debug.Log("velocity score");
                    Debug.Log(bulletVelocity.sqrMagnitude * .05f);*/
                    float dodgeScoreCurrent = 20 / (transform.position - bulletPosition).sqrMagnitude + bulletVelocity.sqrMagnitude * .05f;
                    if (dodgeScoreCurrent > dodgeScoreConfirmThreshold && dodgeScoreCurrent > dodgeScore)
                    {
                        confirm = true;
                        dodgeScore = dodgeScoreCurrent;
                        bulletToDodgeVelocity = bulletVelocity;
                    }
                }
            }
            
        }
        if (confirm)
        {
            if (rand >= .5f)
            {
                dodgeDirection = bulletToDodgeVelocity.Perpendicular1();
            }
            else
            {
                dodgeDirection = -bulletToDodgeVelocity.Perpendicular1();
            }
            StartCoroutine(dodge(dodgeDirection));
        }
    }

    public IEnumerator dodge(Vector3 direction)
    {
        if (dodging)
        {
            yield break;
        }
        bool vulnerableTrigger = false;
        dodgeCount++;
        if (dodgeCount > numConsecutiveDodges)
        {
            vulnerableTrigger = true;
        }
        StartCoroutine(Helper.flash(sr, Color.red, ogColor, .1f));
        dodging = true;
        movement.disableMovement();
        float dodgeSpeedTemp = dodgeSpeed;
        yield return new WaitForSeconds(dodgeStutterTime);
        if (Physics2D.CircleCast(transform.position, colliderRadius * 4, direction, dodgeSpeed * 3, bulletMask))
        {
            direction *= -1;
            if (Physics2D.CircleCast(transform.position, colliderRadius * 4, direction, dodgeSpeed * 3, bulletMask))
            {
                dodgeSpeedTemp *= 1.5f;
            }
        }
        rb.velocity = direction.normalized * dodgeSpeedTemp;
        yield return new WaitForSeconds(dodgeCooldown);
        rb.velocity = Vector2.zero;
        
        if (vulnerableTrigger)
        {
            dodgeCounter.text = "!";
            vulnerable = true;
            dodgeCount = 0;
            int ticks = Mathf.RoundToInt(vulnerableTime / .5f);
            for (int i = 0; i < ticks; i++)
            {
                StartCoroutine(Helper.flash(sr, Color.blue, ogColor, .25f));
                yield return new WaitForSeconds(.5f);
            }
            vulnerable = false;
        }
        dodging = false;
        yield return new WaitForSeconds(movementWaitTime);
        
    }
}
