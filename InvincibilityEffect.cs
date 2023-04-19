using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityEffect : Effect
{
    public float duration;
    private Entity curEntity;
    public override IEnumerator applyEffect(Entity effected, Entity effector)
    {
        if (curEntity != null)
        {
            Debug.LogError("Invincibility applied twice");
        }
        curEntity = effected;
        float timer = 0;
        effected.col.enabled = false;
        while (timer < duration)
        {
            float flashTime = .1f;
            effected.flash(Color.white, flashTime);
            timer += flashTime * 2;
            yield return new WaitForSeconds(flashTime * 2);
        }
        StartCoroutine(removeEffect(effected, effector));
    }

    public override IEnumerator removeEffect(Entity effected, Entity effector)
    {
        curEntity = null;
        effected.col.enabled = true;
        yield break;
    }
}
