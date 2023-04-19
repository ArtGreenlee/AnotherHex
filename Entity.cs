using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
public class Entity : MonoBehaviour
{
    public bool playerEntity;
    public string entityName;
    public Health health;
    public HealthBar healthBar;
    public float magnitudeMin;
    public float magnitudeMax;
    private float magnitudeScaling = 1;
    private float speedScaling = 1;
    private float rangeScaling = 1;
    public Movement movement;
    [SerializeField] protected float speed;
    [SerializeField] protected float range;
    public List<Effect> currentEffects = new List<Effect>();
    public Color originalColor;
    public RangeHelper rangeHelper;
    public string info;
    public Collider2D col;
    public List<Action> onRangeChange = new List<Action>();
    public List<SpriteRenderer> spriteRenderers;
    protected Helper helper;
    public bool fragmentOnDeath;
    bool alive = true;
    public virtual void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        movement = GetComponent<Movement>();
        health = GetComponent<Health>();
        rangeHelper = GetComponent<RangeHelper>();
        col = GetComponent<Collider2D>();
    }

    public virtual void Start()
    {
        if (playerEntity)
        {
            PlayerEntityManager.instance.entityMasterList.Add(this);
        }
        helper = Helper.helper;
        if (rangeHelper != null)
        {
            rangeHelper.setRange(getRange());
        }
    }

    public virtual void onOtherEntityDestroy(Entity other)
    {

    }

    public void flash(Color color, float duration)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            helper.flashColor(sr, color, duration);
        }
    }

    public virtual void OnDeath(Vector3 projectileVelocity, Vector3 projetilePosition)
    {
        alive = false;
        if (fragmentOnDeath)
        {
            DeathFragmentation.instance.fragmentEntity(this, projectileVelocity, projetilePosition);
            fragmentOnDeath = false;
        }
        Destroy(gameObject);
    }

    public virtual void OnDeath()
    {
        alive = false;
        if (fragmentOnDeath)
        {
            DeathFragmentation.instance.fragmentEntity(this);
            fragmentOnDeath = false;
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (playerEntity)
        {
            foreach (Entity e in PlayerEntityManager.instance.entityMasterList)
            {
                e.onOtherEntityDestroy(this);
            }
            PlayerEntityManager.instance.entityMasterList.Remove(this);
        }
    }

    public virtual void onProjectileCollision(ProjectileData pData, Vector3 position)
    {
        if (alive)
        {
            foreach (OnHitEffect effect in pData.onHitEffects)
            {
                StartCoroutine(effect.applyOnHitEffect(pData, this));
            }
            if (!health.changeHealth(-pData.damage))
            {
                OnDeath(pData.velocity, position);
            }
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Projectile>(out Projectile p))
        {
            onProjectileCollision(p.pData, other.transform.position);
        }
        else if (other.gameObject.TryGetComponent<Entity>(out Entity entity))
        {
            onCollision(entity);
        }
        else if (other.gameObject.CompareTag("Wall") && movement != null)
        {
            movement.movementOnCollision();
        }
    }


    public void changeRangeScaling(float change)
    {
        foreach (Action action in onRangeChange)
        {
            action();
        }
        rangeScaling += change;
        rangeHelper.setRange(getRange());
    }

    public float getRange()
    {
        return range * rangeScaling;
    }

    public void changeMagnitudeScaling(float change)
    {
        magnitudeScaling += change;
    }

    public float getMagnitude()
    {
        return Mathf.Round(Random.Range(magnitudeMin, magnitudeMax)) * magnitudeScaling;
    }

    public void changeSpeedScaling(float change)
    {
        speedScaling += change;
    }
    public float getSpeed()
    {
        return speed * speedScaling;
    }

    public virtual void onCollision(Entity other)
    {
        if (movement != null)
        {
            movement.movementOnCollision();
        }
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            Debug.Log("collided with enemy");
            health.changeHealth(-enemy.getCollisionDamage());
        }
    }

}
