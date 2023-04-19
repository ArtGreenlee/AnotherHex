using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRegisterDamage : OnDamageEffect
{
    public Shield shield;
    public override IEnumerator applyOnDamageEffect(float damage, Entity damaged)
    {
        shield.changeShieldSize();
        yield break;
    }
}

public class Shield : MonoBehaviour
{
    ShieldController controller;
    Health health;
    float startScaleX;
    ShieldRegisterDamage damageListener;
    private void Start()
    {
        damageListener = gameObject.AddComponent<ShieldRegisterDamage>();
        damageListener.shield = this;
        startScaleX = transform.localScale.x;
        health = GetComponent<Health>();
        controller = GetComponentInParent<ShieldController>();
        health.onDamageEffects.Add(damageListener);
    }

    public void changeShieldSize()
    {
        Vector3 localScale = transform.localScale;
        localScale.x = startScaleX * health.curHealth / health.maxHealth;
        transform.localScale = localScale;
    }

    private void OnDestroy()
    {
        health.onDamageEffects.Remove(damageListener);
        Destroy(controller);
    }
}
