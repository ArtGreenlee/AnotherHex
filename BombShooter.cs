using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombShooter : Weapon
{
    public GameObject bombObject;
    public List<float> damageRatioAtRadius;
    public Color bombExplosionColor;
    public Color bombColor;
    public float detonationVelocity;
    public override void shoot(int projectileMask)
    {
        Bomb bomb = Instantiate(bombObject, transform.position, Quaternion.identity).GetComponent<Bomb>();
        bomb.sr.color = bombColor;
        bomb.explosionColor = bombExplosionColor;
        bomb.rb.AddForce(weaponTransform.up * entity.getRange(), ForceMode2D.Impulse);
        bomb.rb.AddTorque(10, ForceMode2D.Impulse);
        bomb.pData.damage = entity.getMagnitude();
        bomb.damageRatioAtRadius = damageRatioAtRadius;
        bomb.detonationVelocity = detonationVelocity;
        assignEffectsToProjectile(bomb);
        bomb.gameObject.layer = projectileMask;
    }

}
