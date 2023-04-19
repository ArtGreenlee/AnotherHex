using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOnHitEffect : OnHitEffect
{
    public float totalDamage;
    public float duration;
    public float tickDuration;
    public override IEnumerator applyOnHitEffect(ProjectileData projectile, Entity effected)
    {
        Health health = effected.health;
        int numTicks = Mathf.RoundToInt(duration / tickDuration);
        float tickDamage = totalDamage / numTicks;
        for (int i = 0; i < numTicks; i++)
        {
            if (health.gameObject.activeInHierarchy)
            {
                health.changeHealth(-tickDamage, false, Color.red);
            }
            else
            {
                yield break;
            }
            yield return new WaitForSeconds(tickDuration);
        }
    }
}
