using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningWeapon : Weapon
{
    public GameObject lightningParticleSystemObject;
    Dictionary<uint, ProjectileData> particleData = new Dictionary<uint, ProjectileData>();
    private LightningParticleSystem lightningParticleSystem;
    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

    public override void Start()
    {
        lightningParticleSystemObject = Instantiate(lightningParticleSystemObject, transform);
        lightningParticleSystem = lightningParticleSystemObject.GetComponent<LightningParticleSystem>();
        lightningParticleSystem.onTriggerAction = onLightningWeaponParticleTriggerEnter;
        base.Start();
    }

    public void onLightningWeaponParticleTriggerEnter()
    {
        int numEnter = lightningParticleSystem.ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter, out ParticleSystem.ColliderData data);
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle particle = enter[i];

            Enemy e = data.GetCollider(i, 0).GetComponent<Enemy>();
            if (particleData.ContainsKey(particle.randomSeed))
            {
                ProjectileData projectile = particleData[particle.randomSeed];
                projectile.velocity = particle.velocity;
                e.onProjectileCollision(projectile, particle.position);
                particleData.Remove(particle.randomSeed);
            }
        }
    }

    public override void shoot(int projectileMask)
    {
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        uint randomSeed = 0;
        do
        {
            randomSeed = (uint)Random.Range(0, int.MaxValue);

        } while (particleData.ContainsKey(randomSeed));
        emitParams.position = transform.position;
        emitParams.randomSeed = randomSeed;
        emitParams.velocity = weaponTransform.up * 50 + Random.insideUnitSphere * 10;
        ProjectileData p = new ProjectileData();
        particleData[randomSeed] = p;
        p.damage = entity.getMagnitude();
        lightningParticleSystem.ps.Emit(emitParams, 1);
    }
}
