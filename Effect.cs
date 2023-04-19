using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public abstract IEnumerator applyEffect(Entity effected, Entity effector);

    public abstract IEnumerator removeEffect(Entity effected, Entity effector);
}
