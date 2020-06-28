using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkBehaviour : MonoBehaviour
{
    private PathBehaviour pathBehaviour;
    public SparkModel sparkModel;

    // Start is called before the first frame update
    void Start()
    {
        GamePlayManager.obj.gameEnded += DestroyObject;
        pathBehaviour = GetComponent<PathBehaviour>();
        pathBehaviour.reached += AddSparkForce;
    }
    private void OnDestroy()
    {
        GamePlayManager.obj.gameEnded -= DestroyObject;
        pathBehaviour.reached -= AddSparkForce;
    }
    public void AddSparkForce()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce((Vector3.up * 10 - transform.position).normalized * sparkModel.force);
        rigidbody.useGravity = true;
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DestroyObject();
        }
        else  if (other.tag == "Hand")
        {
            Debug.Log("Hand collision detected...................");
            pathBehaviour.enabled = false;

            //Vector3 dir = -(other.GetContact(0).point - transform.position).normalized;
            //Rigidbody r = GetComponent<Rigidbody>();
            //r.AddForce((r.velocity + Vector3.forward * 0.1f) * 1000);
            //Invoke("DestroyObject", 2);
        }
    }

    /*private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "Hand")
        {
            Debug.Log("Hand collision detected...................");
            pathBehaviour.enabled = false;

            //Vector3 dir = -(other.GetContact(0).point - transform.position).normalized;
            //Rigidbody r = GetComponent<Rigidbody>();
            //r.AddForce((r.velocity + Vector3.forward * 0.1f) * 1000);
            //Invoke("DestroyObject", 2);
        }
    }*/

}
