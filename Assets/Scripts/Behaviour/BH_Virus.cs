using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BH_Virus : MonoBehaviour
{
    public SparkModel virus;
    Transform target;
    float currentLife = 0;
    //float currentSpeed;
    // Start is called before the first frame update
    void Start()
    {
        SearchSparck();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLife > virus.life)
        {
            Destroy(gameObject);
        }
        else
        {
            if (target != null)
            {
                float speed = target.tag == "Player" ? 0.05f : 0.5f;
                transform.position = Vector3.Lerp(transform.position, target.position, speed);
                //currentSpeed += Time.deltaTime;
            }
            else
                SearchSparck();
            currentLife += Time.deltaTime;
        }
    }

    private void SearchSparck()
    {
        //currentSpeed = 0.1f;
        GameObject[] sparks = GameObject.FindGameObjectsWithTag("Spark");

        float mini = 99999;
        foreach (GameObject spark in sparks)
        {
            Rigidbody sparkRigidbody = spark.GetComponent<Rigidbody>();
            if (sparkRigidbody != null && !sparkRigidbody.useGravity && spark.GetComponent<NotSparkBehaviour>() == null)
            {
                float distance = Vector3.Distance(transform.position, spark.transform.position);
                if (distance < mini)
                {
                    mini = distance;
                    target = spark.transform;
                }
            }
        }
        if (target == null)
        {
            target = GamePlayManager.obj.player;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Spark")
        {
            other.gameObject.AddComponent<NotSparkBehaviour>().ChangeColor();
            other.gameObject.GetComponent<SparkBehaviour>().sparkModel = virus;
            other.gameObject.GetComponent<PathBehaviour>().spawnSpark.SetInfectedColors();
        }
        else if (other.tag == "Player")
        {
            GamePlayManager.obj.Damage(1, GamePlayManager.obj.playerModel.damage);
        }
        Destroy(gameObject);
    }

}
