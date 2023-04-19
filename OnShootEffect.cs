using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnShootEffect : MonoBehaviour
{
    public abstract IEnumerator applyOnShootEffect(Projectile projectile, Weapon weapon);
}
