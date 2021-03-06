﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HandBehaviour : MonoBehaviour
{
    [SerializeField]
    public OVRInput.Controller m_controller = OVRInput.Controller.None;

    public float touch;
    public float touch2;

    [SerializeField]
    Renderer meshRenderer;
    [SerializeField]
    AudioSource audioSouce;
    PlayerBehaviour player;
    [SerializeField]
    ParticleSystem[] pss;
    [SerializeField]
    ParticleSystem powerRay;
    [SerializeField]
    ParticleSystem hitSparks;
    [SerializeField]
    ParticleSeeker particleSeeker;
    [SerializeField]
    Transform targeting;
    PowerBehaviour particlePower;
    //TrailRenderer trail;

    [SerializeField]
    int count;
    Vector3 lastPosition;
    int detectorIndex = 0;
    public bool tracking;
    bool isHit;
    int layerMask;
    float maxDistance = 300f;
    float size = 4f;
    RaycastHit hit;


    public static byte TwoHand;

    /*private void OnEnable()
    {
        VRGlyphInput.OnMatchResult += MatchGesture;
    }
    private void OnDisable()
    {

        VRGlyphInput.OnMatchResult -= MatchGesture;
    }*/
    public void MatchGesture(string result, float match, float ms)
    {
        if (count > 0)
        SendPower();
    }
    void Start()
    {
        powerRay.Stop();
        foreach (ParticleSystem ps in pss)
        {
            ps.Stop();
        }
        lastPosition = transform.position;
        meshRenderer.material.color = Color.black;
        meshRenderer.material.SetColor("_EmissionColor", Color.black);
        layerMask = LayerMask.GetMask("Enemy", "Fail");
        particlePower = particleSeeker.GetComponent<PowerBehaviour>();
        //trail = GetComponent<TrailRenderer>();
        //trail.endColor = new Color(0,0,0,0);
        if (tracking)
            StartCoroutine(PowerTest());
    }
    IEnumerator PowerTest()
    {
        int i = 0;
        do
        {
            StartParticles();
            SendPower();
            yield return new WaitForSeconds(1);
            i++;
        } while (i < 1000);
    }

    private void Update()
    {
        /*float touch = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
        if (touch > 0)
        {
            detectorIndex |= 4;
            return;
        }
        detectorIndex &= ~4;*/
        isHit = Physics.BoxCast(transform.position, new Vector3(size, size, size), transform.forward, out hit, transform.rotation, maxDistance, layerMask);
        if (isHit)
        {
            if (hit.collider.tag == "Enemy")
            {
                targeting.position = Vector3.Lerp(hit.collider.transform.position + Vector3.up * 2, targeting.position, Time.deltaTime);
                targeting.LookAt(GamePlayManager.obj.player.position);
                targeting.Rotate(0, 180, 0);
                targeting.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                targeting.position = hit.point;
            }
        }
        else
        {
            targeting.GetChild(0).gameObject.SetActive(false);
        }
    }
    void LateUpdate()
    {
        /*if (count == 0)
        {
            return;
        }*/

        touch = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller);
        touch2 = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
        //trail.enabled = touch > 0;
        /*if (touch2 == 0)
        {
            return;
        }*/
        //SendPower();
        //trail.startColor = GetColor();
        /* Vector3 deltaPosition = transform.position - lastPosition;
         if (Vector3.Dot(GameManager.obj.player.forward, deltaPosition) < 0)
         {
             if (tracking)
                GameManager.DebugApp("SYS", "magnitude detected: " + deltaPosition.magnitude + " amplifed: " + (deltaPosition.magnitude + 100000));
             detectorIndex |= 1;
             if (deltaPosition.magnitude > 0.07f)
             {
                 detectorIndex |= 2;
                 if (tracking)
                     GameManager.DebugApp("SYS", "Back detected");
             }
         }
         else if (detectorIndex == 3)
         {
             if (deltaPosition.magnitude > 0.07f)
             {
                 SendPower();
             }
         }
         else
         {
             detectorIndex &= ~3;
         }*/
         //lastPosition = transform.position;
    }
    /*private void FixedUpdate()
    {

        lastPosition = transform.position;
    }*/
    /*private void OnDrawGizmos()
    {
        Vector3 boxScale = new Vector3(size, size, size);
        if (isHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
            Gizmos.DrawCube(hit.transform.position, boxScale);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        }
    }*/
    /*private void OnTriggerStay(Collider other)
    {
        
        ProcessSparkContact(other.gameObject);
    }*/
    private void OnCollisionEnter(Collision collision)
    {
        ProcessSparkContact(collision);
    }
    private void ProcessSparkContact(Collision collision)
    {
        if (collision.rigidbody.tag == "Spark")
        {
            SparkBehaviour spark = collision.rigidbody.GetComponent<SparkBehaviour>();
            if (spark.hitted) return;
            if (touch2 == 0)
            {
                audioSouce.clip = spark.sparkModel.sound;
                audioSouce.Play();
                StartParticles();
                SetCount();
                GamePlayManager.obj.SetPoints(100);
                Destroy(collision.gameObject);
            }
            else
            {
                Pushhit(spark, collision.contacts[0].normal);
            }
            hitSparks.Emit(5);
        }
        else if (collision.rigidbody.tag == "Virus")
        {
            SparkBehaviour spark = collision.rigidbody.GetComponent<SparkBehaviour>();
            if (spark.hitted) return;
            if (touch2 == 0)
            {
                if (spark != null)
                {
                    audioSouce.clip = spark.sparkModel.sound;
                }
                else
                {
                    BH_Virus bh = collision.rigidbody.GetComponent<BH_Virus>();
                    if (bh == null) return;
                    audioSouce.clip = bh.virus.sound;
                }
                audioSouce.Play();
                Destroy(collision.gameObject);
            }
            else
            {
                Pushhit(spark, collision.contacts[0].normal);
            }
            hitSparks.Emit(5);
        }
    }
    private void Pushhit(SparkBehaviour spark, Vector3 contact)
    {
        //Vector3 direction = transform.position - lastPosition;
        Vector3 force =  -10000  * contact;
        //GameManager._obj.Print("FORCE", force.ToString() + " contact: " + contact + " direction: " + direction);
        spark.hitDirection = force;    
    }
    public void SendPower()
    {
        detectorIndex = 4;
        //bool isHit = Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, layerMask);
        //if (isHit)
        //{
        //if (hit.collider.tag == "Enemy"){
        //StartCoroutine(hit.transform.GetComponent<EnemyBehaviour>().SetDamageWithDelay(count));
        //}

        //powerRay.trigger.SetCollider(0, hit.transform);
        //Transform power = GameManager.obj.playerModel.InstantiatePowerPrefab(transform.position + transform.forward * 0.5f, transform.forward, count);
        particlePower.power = 10 * (count + 1);
        //particleSeeker.target = power;
        particleSeeker.enabled = true;

        powerRay.Emit(Math.Max(1,count - 3));
        Debug.Log("Emit: " + count);
        //}

        count = 0;
        StartCoroutine(ClearHandStatus());
    }
    private IEnumerator ClearHandStatus()
    {
        yield return new WaitForSeconds(1);
        count = 0;
        detectorIndex = 0;

        FinishParticles();
        particleSeeker.enabled = false;
        powerRay.trigger.SetCollider(0, null);
    }
    private void StartParticles()
    {
        foreach (ParticleSystem ps in pss)
            if (ps.isStopped)
                ps.Play();
    }
    private void FinishParticles()
    {
        meshRenderer.material.color = Color.black;
        meshRenderer.material.SetColor("_EmissionColor", Color.black);
        SetColor(Color.white);
        for (int i = 0; i < pss.Length; i++)
        {
            if (pss[i].isPlaying)
            {
                pss[i].Stop();
            }
        }
        if (powerRay.isPlaying)
            powerRay.Stop();
    }
    private void SetCount()
    {
        switch (count)
        {
            case 0:
                count = 3;
                meshRenderer.material.color = Color.white;
                meshRenderer.material.SetColor("_EmissionColor", Color.white);
                break;
            case 3:
                count = 5;
                meshRenderer.material.color = Color.blue;
                meshRenderer.material.SetColor("_EmissionColor", Color.blue);
                SetColor(Color.blue);
                break;
            case 5:
                count = 10;
                meshRenderer.material.color = Color.red;
                meshRenderer.material.SetColor("_EmissionColor", Color.red);
                SetColor(Color.red);
                break;
            case 10:
                count = 0;
                Explosion();
                TakeDamage(2);
                break;
        }
    }
    Color GetColor()
    {
        switch (count)
        {
            case 3:
                return Color.blue;
            case 5:
                return Color.red;
        }
        return Color.white;
    }
    private void SetColor(Color color)
    {
        foreach (ParticleSystem ps in pss)
        {
            var main = ps.main;
            main.startColor = color;
        }
        var trails = powerRay.trails;
        trails.colorOverTrail = color;
    }
    private void Explosion()
    {
        FinishParticles();

    }
    private void TakeDamage(int amount)
    {
        GamePlayManager.obj.Damage(amount, GamePlayManager.obj.playerModel.overload);
    }
}
