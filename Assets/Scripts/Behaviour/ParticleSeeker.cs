using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSeeker : MonoBehaviour
{
    public Transform target;
    public float force = 100.0f;
    public ParticleSystem _particleSystem;
    public int amount = 1;
    public bool isBad = true;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystem.Stop();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[_particleSystem.particleCount];
        _particleSystem.GetParticles(particles);
        for (int i=0; i < particles.Length; i++)
        {
            ParticleSystem.Particle p = particles[i];
            Vector3 direction = (target.position - p.position).normalized;
            Vector3 forceVector = direction * force * Time.deltaTime;
            p.velocity = forceVector;

            particles[i] = p;
        }

        _particleSystem.SetParticles(particles, particles.Length);
    }
    
    public void SetTarget(Transform target)
    {
        this.target = target;
        _particleSystem.trigger.SetCollider(0,target.GetComponent<Collider>());
    }
    private void OnParticleCollision(GameObject other)
    {
        if (!isBad)
        {
            int count = _particleSystem.GetCollisionEvents(other,new List<ParticleCollisionEvent>());
            GameManager.obj.playerHealth += count;
        }
    }

}
