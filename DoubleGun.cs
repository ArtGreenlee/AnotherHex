using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleGun : Gun
{
    BulletObjectPool bulletPool;
    public Transform weaponTransform2;
    public Transform weaponTransform1;

    public override void Start()
    {
        bulletPool = BulletObjectPool.bulletObjectPool;
        base.Start();
    }

    public override void shoot(int projectileLayer)
    {
        Bullet b1 = bulletPool.Pool.Get();
        Bullet b2 = bulletPool.Pool.Get();
        shootBullet(b1, weaponTransform1, Vector3.up);
        shootBullet(b2, weaponTransform2, Vector3.up);
        b1.gameObject.layer = projectileLayer;
        b2.gameObject.layer = projectileLayer;
    }
}
