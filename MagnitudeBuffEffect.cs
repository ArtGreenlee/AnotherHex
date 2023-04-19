using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagnitudeBuffEffect : Effect
{
    public override IEnumerator applyEffect(Entity effected, Entity effector)
    {
        if (!effected.currentEffects.Contains(this))
        {
            effected.changeMagnitudeScaling(effector.getMagnitude());
            effected.currentEffects.Add(this);
        }
        yield break;
    }

    public override IEnumerator removeEffect(Entity effected, Entity effector)
    {
        if (effected.currentEffects.Contains(this))
        {
            effected.changeMagnitudeScaling(-effector.getMagnitude());
            effected.currentEffects.Remove(this);
        }
        yield break;
    }
}
