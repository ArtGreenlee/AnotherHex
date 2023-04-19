using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnDamageEffect : MonoBehaviour
{
    public abstract IEnumerator applyOnDamageEffect(float damage, Entity damaged);
}
