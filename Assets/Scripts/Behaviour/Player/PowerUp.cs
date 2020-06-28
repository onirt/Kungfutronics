using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    MeshRenderer render;
    ParticleSystem ps;
    private void Start()
    {
        render = GetComponent<MeshRenderer>();
        ps = GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        Transform player = GamePlayManager.obj.player;
        transform.LookAt(new Vector3(player.position.x,transform.position.y,player.position.z));
    }
    private void OnParticleCollision(GameObject other)
    {
        render.material.color = Color.green;
        render.material.SetColor("_EmissionColor", Color.green);
        ps.Emit(3);
        StartCoroutine(ChangeColor());
    }
    private void OnParticleTrigger()
    {

        Debug.Log("paricle trugger");
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if ((prev != null && !prev.ready) ||(handTracking.Length > 0 && !other.name.Equals(handTracking)))
        {
            ready = false;
            handTracking = "";
            return;
        }
        if (next != null) {
            next.handTracking = handTracking;
        }
        else
        {
            other.GetComponent<HandBehaviour>().SendPower();
        }
        ready = true;
        handTracking = "";
        render.material.color = Color.green;
        render.material.SetColor("_EmissionColor", Color.green);
        StartCoroutine(ChangeColor());
    }*/
    private IEnumerator ChangeColor()
    {
        yield return new WaitForSeconds(1);
        render.material.color = Color.white;
        render.material.SetColor("_EmissionColor", Color.white);
        transform.position = new Vector3(Random.Range(25,-25), Random.Range(2,33), Random.Range(25, -25));
    }
}
