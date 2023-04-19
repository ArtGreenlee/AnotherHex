using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Collider2D col;
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public ProjectileData pData;
    public virtual void Awake()
    {
        pData = new ProjectileData();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
}

public class ProjectileData
{
    public float damage;
    public List<OnHitEffect> onHitEffects = new List<OnHitEffect>();
    public bool crit;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public Vector3 velocity;

}
