using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class Player : Entity
{
    public static Player player;
    private Rigidbody2D rb;
    public float knockbackForceMagnitude;
    public float invincibilityTime;
    public float collisionRadius;
    public float xpMax;
    private float xpCur;
    public HealthBar xpBar;
    private InvincibilityEffect invincibleEffect;
    //public float fragmentPickupRadius;
    //float fragmentDestroyRadiusSqrd;
    public void addXP(float xpAmount)
    {
        xpCur += xpAmount;
        if (xpCur > xpMax)
        {
            health.changeHealth(health.maxHealth - health.curHealth);
            xpCur = 0;
        }
        xpBar.registerChange(xpCur / xpMax);
    }
    public override void Awake()
    {
        helper = Helper.helper;
        base.Awake();
        player = this;
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        invincibleEffect = gameObject.AddComponent<InvincibilityEffect>();  
        invincibleEffect.duration = invincibilityTime;
    }

    public override void Start()
    {
        //float colliderRadius = GetComponent<CircleCollider2D>().radius;
        //fragmentDestroyRadiusSqrd = colliderRadius * colliderRadius;
        base.Start();
        //rangeHelper.setRange(fragmentPickupRadius);
        //rangeHelper.setEnterCallbackFunction(pickupFragment);
    }

    /*public void pickupFragment(Collider2D fragmentCollider)
    {
        if (fragmentCollider.TryGetComponent<Fragment>(out Fragment fragment))
        {
            IEnumerator newRoutine = collectFragmentRoutine(fragment);
            StartCoroutine(newRoutine);
        }
    }

    private void fragmentCollision(Fragment f)
    {
        f.rb.velocity = Vector2.zero;
        addXP(1);
        StartCoroutine(DeathFragmentation.instance.fadeAndRelease(f));
    }
    */

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.TryGetComponent<Fragment>(out Fragment fragment))
        {
            fragment.rb.velocity = Vector2.zero;
            fragment.col.enabled = false;
            addXP(1);
            StartCoroutine(DeathFragmentation.instance.fadeAndRelease(fragment));
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnitySceneManager.LoadScene(UnitySceneManager.GetActiveScene().name);
        }
    }

    /*private IEnumerator collectFragmentRoutine(Fragment fragment)
    {
        Rigidbody2D fragmentRb = fragment.rb;
        fragmentRb.velocity = Vector3.zero;
        Vector3 diff = fragment.transform.position - transform.position;
        fragmentRb.AddForce(diff.normalized * 3, ForceMode2D.Impulse);
        helper.flashColor(fragment.sr, Color.white, .1f);
        fragment.col.enabled = false;
        while (diff.sqrMagnitude > fragmentDestroyRadiusSqrd)
        {
            diff = fragment.transform.position - transform.position;
            fragmentRb.AddForce(-diff.normalized);
            yield return new WaitForEndOfFrame();
        }
        fragmentCollision(fragment);
    }
    */
    public override void onCollision(Entity other)
    {
        health.changeHealth(-other.getMagnitude());
        Vector3 forceDirection = transform.position - other.transform.position;
        rb.AddForce(forceDirection.normalized * knockbackForceMagnitude, ForceMode2D.Impulse);
        StartCoroutine(invincibleEffect.applyEffect(this, other));
    }
}
