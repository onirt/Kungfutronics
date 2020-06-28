using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBehaviour : MonoBehaviour
{
    public delegate void Reached();
    public Reached reached;

    public PathModel pathModel;


    [SerializeField]
    private bool haveFreePass = false;
    [SerializeField]
    private float speed = 1f;

    //private float distanceTravelled;
    private int currentPoint;
    private bool stop;

    public BH_SpawnSpark spawnSpark;

    void Start()
    {
        currentPoint = 1;
    }
    private void OnDestroy()
    {
        pathModel = null;
    }

    // Update is called once per frame
    void Update()
    {
        //if (stop) return;
        if ((GamePlayManager.isPlaying() || haveFreePass) && !stop)
        {
            if (pathModel == null) return;
            //distanceTravelled += speed * Time.deltaTime;
            transform.Translate((pathModel.points[currentPoint] - transform.position).normalized * speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, pathModel.points[currentPoint]) < 0.05f)
            {
                currentPoint++;
                if (currentPoint >= pathModel.points.Length)
                {
                    spawnSpark.RestoreColors();
                    stop = true;
                    reached?.Invoke();
                }
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Respawn")
        {
            spawnSpark = other.GetComponent<BH_SpawnSpark>();
            spawnSpark.StartColors();
        }
    }
}
