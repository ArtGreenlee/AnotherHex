using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class Weapon : MonoBehaviour
{
    public Transform weaponTransform;
    float transformOffset;
    protected float projectileOffset;
    public List<OnHitEffect> onHitEffects = new List<OnHitEffect>();
    public List<OnShootEffect> onShootEffects = new List<OnShootEffect>();
    public float shootTimer = 0;
    public Entity entity;
    public RangeHelper rangeHelper;
    private void Awake()
    {
        rangeHelper = GetComponent<RangeHelper>();
        entity = GetComponentInParent<Entity>();
        if (entity == null)
        {
            entity = GetComponent<Entity>();
        }
        if (entity == null)
        {
            //Debug.LogError("what");
            Destroy(gameObject);
        }
    }
    public virtual void Start()
    {
        transformOffset = weaponTransform.localPosition.y;
        projectileOffset = transformOffset + weaponTransform.localScale.y / 2;
        foreach (OnHitEffect effect in GetComponents<OnHitEffect>())
        {
            onHitEffects.Add(effect);
        }
        foreach (OnShootEffect onShootEffect in GetComponents<OnShootEffect>())
        {   
            onShootEffects.Add(onShootEffect);
        }
    }
    public abstract void shoot(int projectileMask);
    
    protected void assignEffectsToProjectile(Projectile p)
    {
        foreach (OnShootEffect effect in onShootEffects)
        {
            effect.applyOnShootEffect(p, this);
        }
        p.pData.onHitEffects = onHitEffects;
    }
    public void rotateWeaponTransform(Vector3 direction)
    {
        Vector3 weaponEulerAngles = weaponTransform.eulerAngles;
        weaponEulerAngles.z = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) - 90;
        weaponTransform.localEulerAngles = weaponEulerAngles;
        weaponTransform.localPosition = direction.normalized * transformOffset;
    }
}
