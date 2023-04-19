using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningParticleSystem : MonoBehaviour
{
    LightningParticleSystemManager manager;
    public ParticleSystem ps;
    public ParticleSystem.TriggerModule psTrigger;
    public Action onTriggerAction;
    
    private void Awake()
    {
        manager = LightningParticleSystemManager.instance;
        ps = GetComponent<ParticleSystem>();
        psTrigger = ps.trigger;
    }

    private void Start()
    {
        manager = LightningParticleSystemManager.instance;
        manager.registerLightningParticleSystem(this);
    }

    private void OnDestroy()
    {
        manager.unregisterLightningParticleSystem(this);
    }

    private void OnParticleTrigger()
    {
        onTriggerAction();    
    }
}
