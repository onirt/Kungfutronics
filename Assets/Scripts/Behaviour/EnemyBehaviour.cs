using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    bool passive;
    [SerializeField]
    SparkModel virus;
    Animator animator;
    float timer = 0;
    [SerializeField]
    Renderer _render;
    [SerializeField]
    float threshold;
    ParticleSeeker particleSeeker; 
    Transform closest = null;

    public int healt = 100;
    bool busy;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play("Idle", 0, randomIdleStart);
        timer = Random.Range(0,threshold);
        particleSeeker = GetComponentInChildren<ParticleSeeker>();
        //Invoke("TestSpawn", 2);
    }

    void TestSpawn()
    {
        SetDamage(100);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.obj.gameStatus != GameManager.GameStatus.Started || passive)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer > threshold)
        {
            if (!busy) {
                animator.applyRootMotion = false;
                GameObject[] sparks = GameObject.FindGameObjectsWithTag("Spark");

                float mini = 99999;
                foreach (GameObject spark in sparks)
                {
                    Rigidbody sparkRigidbody = spark.GetComponent<Rigidbody>();
                    if (sparkRigidbody != null && !sparkRigidbody.useGravity && spark.GetComponent<NotSparkBehaviour>() == null) {
                        float distance = Vector3.Distance(transform.position, spark.transform.position);
                        if (distance < mini)
                        {
                            mini = distance;
                            closest = spark.transform;
                        }
                    }
                }
                if (closest != null)
                {
                    particleSeeker.target = closest;
                    transform.LookAt(new Vector3(closest.position.x, transform.position.y, closest.position.z));
                    animator.SetTrigger("Cast");
                    busy = true;
                    closest.gameObject.AddComponent<NotSparkBehaviour>().ChangeColor();
                    closest.gameObject.GetComponent<SparkBehaviour>().sparkModel = virus;

                }
            }
            else if (closest != null)
            {
                transform.LookAt(new Vector3(closest.position.x, transform.position.y, closest.position.z));
            }
            
        }
    }
    private void OnParticleTrigger()
    {
        //SetDamage();
    }
    public IEnumerator SetDamageWithDelay(int amount)
    {
        yield return new WaitForSeconds(1);
        SetDamage(amount);
    }
    public void SetDamage(int amount)
    {
        healt-=amount;
        animator.applyRootMotion = true;
        if (healt <= 0)
        {
            animator.SetTrigger("Death");
            GameManager.obj.SetPoints(1000);
            GameManager.obj.SpawnNewEnemy(name);
            Destroy(gameObject, 4);
        }
        else
        {
            animator.SetTrigger("Damage");
        }
        Color newcolor = Color.Lerp(Color.white, Color.red, healt / 10f);
        _render.material.color = newcolor;
        _render.material.SetColor("_MyColor", newcolor);
    }
    /*private void OnCollisionEnter(Collision collision)
    {
        healt-=2;
        if (healt>0)
        {
            animator.SetTrigger("Damage");
        }
        else
        {
            animator.SetTrigger("Death");
            Destroy(gameObject, 5);
        }
    }*/

    public void AttackEvent()
    { 
        particleSeeker._particleSystem.Play();
    }

    public void ReadyEvent()
    {
        animator.applyRootMotion = true;
        busy = false;
        timer = 0;
        threshold = Random.Range(threshold, threshold*2);
        particleSeeker._particleSystem.Stop();

    }
}
