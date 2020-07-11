using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkBehaviour : MonoBehaviour
{
    public SparkModel sparkModel;
    private PathBehaviour pathBehaviour;
    private Rigidbody _rigidbody;
    private bool _hitted;
    public bool hitted
    {
        get
        {
            return _hitted;
        }
    }
    private float speed;


    float currentLife = 0;

    public Vector3 hitDirection
    {
        set
        {
            _hitDirection = value.normalized;
            _hitted = true;
            _rigidbody.useGravity = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            UpdateHitForce();
        }
    }
    private Vector3 _hitDirection;
    [SerializeField]
    private bool isTest;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (isTest) return;
        GamePlayManager.obj.gameEnded += DestroyObject;
        pathBehaviour = GetComponent<PathBehaviour>();
        pathBehaviour.reached += AddSparkForce;
        
    }
    private void OnDestroy()
    {
        if (isTest) return;
        GamePlayManager.obj.gameEnded -= DestroyObject;
        pathBehaviour.reached -= AddSparkForce;
    }

    private void FixedUpdate()
    {
        if (hitted)
        {
            UpdateHitForce();
            if (currentLife > sparkModel.defaultLife)
            {
                Destroy(gameObject);
            }
            currentLife += Time.deltaTime;
        }
    }
    public void AddSparkForce()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce((Vector3.up * 10 - transform.position).normalized * sparkModel.force);
        rigidbody.useGravity = true;
        pathBehaviour.enabled = false;
    }
    private void UpdateHitForce()
    {

        float moveInfluence = sparkModel.multiplier * Time.deltaTime;
        _rigidbody.MovePosition(transform.position + _hitDirection * moveInfluence);
        _rigidbody.AddForce(_hitDirection * moveInfluence * 0.1f);
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
        /*else  if (other.tag == "Hand")
        {
            pathBehaviour.enabled = false;

            //Vector3 dir = -(other.GetContact(0).point - transform.position).normalized;
            //Rigidbody r = GetComponent<Rigidbody>();
            //r.AddForce((r.velocity + Vector3.forward * 0.1f) * 1000);
            //Invoke("DestroyObject", 2);
        }*/
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
