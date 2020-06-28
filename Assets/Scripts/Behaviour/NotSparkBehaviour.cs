using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotSparkBehaviour : MonoBehaviour
{

    void Start()
    {
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Electrocute();
        }
    }

    private void Electrocute()
    {
        GamePlayManager.obj.Damage(GetComponent<SparkBehaviour>().sparkModel.points, GamePlayManager.obj.playerModel.damage);
        Destroy(gameObject);
    }
    public void ChangeColor()
    {
        tag = "Virus";
        for (int i=0; i < transform.childCount; i++) {
            if (i == 3) {
                Material material = transform.GetChild(i).GetComponent<Renderer>().material;
                if (material == null) return;
                material.color = Color.red;
                material.SetColor("_EmissionColor", Color.red);
            }
            else
            {
                ParticleSystem particleSystem = transform.GetChild(i).GetComponent<ParticleSystem>();
                if (particleSystem == null) return;
                ParticleSystem.MainModule mainModule = particleSystem.main;
                mainModule.startColor = Color.red;
            }
        }
    }
}
