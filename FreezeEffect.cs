using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeEffect : OnHitEffect
{
    public float freezeDuration;
    public override IEnumerator applyOnHitEffect(ProjectileData projectile, Entity effected)
    {
        effected.movement.disableMovement();
        yield return new WaitForSeconds(freezeDuration);
        effected.movement.enableMovement();
    }
}
