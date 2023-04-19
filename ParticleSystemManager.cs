using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
    /*private ParticleSystem ps;
    private ParticleSystem.TriggerModule triggerModule;
    public static ParticleSystemManager instance;
    private ParticleSystem.ExternalForcesModule externalForcesModule;
    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
    Dictionary<uint, ProjectileData> particleData = new Dictionary<uint, ProjectileData>();
    public HashSet<Entity> registeredEntities;


    public void registerEntity(Entity e)
    {
        Collider2D col = e.col;
        Debug.Log("registering trigger of gameobject: " + col.gameObject.name);
        triggerModule.AddCollider(col);
        externalForcesModule.AddInfluence(e.psff);
    }

    public void unregisterTrigger(Entity e)
    {
        triggerModule.RemoveCollider(e.col) ;
        externalForcesModule.RemoveInfluence(e.psff);
    }

    private void OnParticleTrigger()
    {
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter, out ParticleSystem.ColliderData data);

        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle particle = enter[i];
            
            Entity e = data.GetCollider(i, 0).GetComponent<Entity>();
            if (particleData.ContainsKey(particle.randomSeed))
            {
                ProjectileData projectile = particleData[particle.randomSeed];
                projectile.velocity = particle.velocity;
                e.onProjectileCollision(projectile, particle.position);
                particleData.Remove(particle.randomSeed);
            }
             
        }
    }
   

    private void Awake()
    {
        instance = this;
        ps = GetComponent<ParticleSystem>();
        externalForcesModule = ps.externalForces;
        triggerModule = ps.trigger;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < 20; i++)
            {
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                uint randomSeed = 0;
                do
                {
                    randomSeed = (uint)Random.Range(0, int.MaxValue);

                } while (particleData.ContainsKey(randomSeed));
                emitParams.position = Helper.mousePosition;
                emitParams.randomSeed = randomSeed;
                ProjectileData p = new ProjectileData();
                particleData[randomSeed] = p;
                p.damage = 1;
                ps.Emit(emitParams, 1);
            }
        }
    }*/
}
