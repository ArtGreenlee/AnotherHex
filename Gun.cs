using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    BulletObjectPool bulletPool;
    public float bulletSpeed;
    public bool pierceAll;
    public int numPierce; 
    public Color bulletColor;
    public Vector3 bulletScale;
    public Vector3 bulletSpawnPosition;

    public override void Start()
    {
        bulletPool = BulletObjectPool.bulletObjectPool;
        base.Start(); 
    }

    public void shootBullet(Bullet bullet, Transform weaponTransform, Vector3 direction)
    {
        bullet.transform.rotation = weaponTransform.rotation;
        bullet.transform.position = weaponTransform.transform.position;//Player.player.transform.TransformPoint(weaponTransform.localPosition);// + weaponTransform.up * weaponTransform.localScale.y);
        Vector3 velocity = weaponTransform.up * bulletSpeed;
        bullet.pData.velocity = velocity;
        bullet.rb.velocity = velocity;
        bullet.sr.color = bulletColor;
        bullet.numPierce = numPierce;
        bullet.pierceAll = pierceAll;
        bullet.pData.damage = entity.getMagnitude();
        bullet.transform.localScale = bulletScale;
        bullet.lifeTimeTimer = Time.time + entity.getRange() / bulletSpeed;
        assignEffectsToProjectile(bullet);
    }

    public override void shoot(int projectileLayer)
    {
        //Debug.Log("shoot");
        Bullet b = bulletPool.Pool.Get();
        Vector3 bulletSpawnPosition = transform.position;
        //Debug.Log("call Shoot bullet");
        shootBullet(b, weaponTransform, Vector3.up);
        b.gameObject.layer = projectileLayer;
    }
}
