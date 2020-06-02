﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBehaviour : MonoBehaviour
{
    [SerializeField]
    private float speed;
    public int power;
    ParticleSystem ps;
    List<ParticleCollisionEvent> collisionEvents;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        collisionEvents = new List<ParticleCollisionEvent>();
        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.xMultiplier += (GameManager.obj.level - 1) * 5;
        velocityOverLifetime.yMultiplier += (GameManager.obj.level - 1) * 5;
        velocityOverLifetime.zMultiplier += (GameManager.obj.level - 1) * 5;
    }

    void Update()
    {
        if (speed > 0)
            transform.position += transform.forward * Time.deltaTime * speed;

    }
    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyBehaviour>().SetDamage(power);
        }
        else
        {
            GameManager.obj.SetPoints(ps.GetCollisionEvents(other, collisionEvents));
            power++;

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        /*if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyBehaviour>().SetDamage(power);
        }*/
        Destroy(gameObject);
    }
}
