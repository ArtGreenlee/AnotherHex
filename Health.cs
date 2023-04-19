using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    private HealthBar healthBar;
    public bool flashOnHit;
    public float maxHealth;
    public float curHealth;
    public bool damageTextOnHit;
    private DamageTextObjectPool damageTextObjectPool; 
    public List<OnDamageEffect> onDamageEffects;
    private Entity entity;
    void Awake()
    {
        entity = GetComponent<Entity>();
        if (entity == null)
        {
            Debug.LogError("OBJECT WITH HEALTH DOES NOT HAVE ENTITY");
        }
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<HealthBar>();
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        damageTextObjectPool = DamageTextObjectPool.damageTextObjectPool;
    }

    private void OnEnable()
    {
        if (healthBar != null)
        {
            healthBar.registerChange(curHealth / maxHealth);
        }
    }

    public void applyOnHitEffects(ProjectileData pData)
    {
        foreach (OnHitEffect effect in pData.onHitEffects)
        {
            StartCoroutine(effect.applyOnHitEffect(pData, entity));
        }
    }

    public void changeHealth(float amount, bool showDamageText, Color color)
    {
        if (amount > 0 && curHealth < maxHealth)
        {
            curHealth += amount;
            
            if (curHealth > maxHealth)
            {
                curHealth = maxHealth;
            }
            entity.flash(Color.green, .03f);
        }
        else if (amount < 0) 
        {
            curHealth += amount;
            entity.flash(Color.white, .03f);
            foreach (OnDamageEffect onDamageEffect in onDamageEffects)
            {
                StartCoroutine(onDamageEffect.applyOnDamageEffect(amount, entity));
            }
        }
         if (amount != 0 && curHealth != maxHealth && damageTextOnHit)
        {
            DamageText damageText = damageTextObjectPool.Pool.Get();
            damageText.showDamageText(transform.position, amount);
        }
        if (curHealth <= 0)
        {
            entity.OnDeath();
        }
        if (healthBar != null)
        {
            healthBar.registerChange(curHealth / maxHealth);
        }
    }

    public bool changeHealth(float amount)
    {
       
        if (amount > 0 && curHealth < maxHealth)
        {
            curHealth += amount;

            if (curHealth > maxHealth)
            {
                curHealth = maxHealth;
            }
            entity.flash(Color.green, .03f);
        }
        else if (amount < 0)
        {
            curHealth += amount;
            entity.flash(Color.white, .03f);
            foreach (OnDamageEffect onDamageEffect in onDamageEffects)
            {
                StartCoroutine(onDamageEffect.applyOnDamageEffect(amount, entity));
            }
        }
        if (amount != 0 && curHealth != maxHealth)
        {
            DamageText damageText = damageTextObjectPool.Pool.Get();
            damageText.showDamageText(transform.position, amount);
        }
        if (healthBar != null)
        {
            healthBar.registerChange(curHealth / maxHealth);
        }
        if (curHealth <= 0.0001f)
        {
            return false;
        }
        return true;
    }
}
