using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalEffect : OnShootEffect
{
    public float criticalPercentage;
    public float critDamage;
    public Vector3 sizeIncrease;

    public override IEnumerator applyOnShootEffect(Projectile projectile, Weapon weapon)
    {
        if (Random.value < criticalPercentage)
        {
            projectile.transform.localScale += sizeIncrease;
            projectile.pData.damage *= critDamage;
        }
        yield break;
    }
}
