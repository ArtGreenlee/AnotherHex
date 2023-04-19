using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class LightningParticleSystemManager : MonoBehaviour
{
    public static LightningParticleSystemManager instance;
    private HashSet<LightningParticleSystem> lightningParticleSystems = new HashSet<LightningParticleSystem>();
    private HashSet<Enemy> register = new HashSet<Enemy>();

    private void Awake()
    {
        instance = this;
    }

    public void registerEnemy(Enemy e)
    {
        if (register.Contains(e)) return;
        register.Add(e);
        foreach (LightningParticleSystem lps in lightningParticleSystems)
        {
            lps.psTrigger.AddCollider(e.col);
        }
    }

    public void registerLightningParticleSystem(LightningParticleSystem lps)
    {
        ParticleSystem.TriggerModule lpsTrigger = lps.psTrigger;
        foreach (Enemy e in register)
        {
            lpsTrigger.AddCollider(e.col);
        }
        lightningParticleSystems.Add(lps);
    }
    
    public void unregisterLightningParticleSystem(LightningParticleSystem lps)
    {
        lightningParticleSystems.Remove(lps);
    }

    public void unregisterEnemy(Enemy e)
    {
        if (!register.Contains(e)) return;
        register.Remove(e);
        foreach (LightningParticleSystem lps in lightningParticleSystems)
        {
            lps.psTrigger.RemoveCollider(e.col);
        }
    }
}
