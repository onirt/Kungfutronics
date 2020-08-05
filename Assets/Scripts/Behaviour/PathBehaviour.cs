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

    private Vector3 currentDirection;
    void Start()
    {
        currentPoint = 1;
        currentDirection = (pathModel.points[currentPoint] - transform.position).normalized;
    }
    private void OnDestroy()
    {
        pathModel = null;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //if (stop) return;
        if ((GamePlayManager.isPlaying() || haveFreePass) && !stop)
        {
            if (pathModel == null) return;
            //distanceTravelled += speed * Time.deltaTime;
            transform.Translate(currentDirection * speed * Time.deltaTime);
            float distance = Vector3.Distance(transform.position, pathModel.points[currentPoint]);
            //GameManager._obj.Print("PATH", "distance: " + distance + " name: " + name);
            if (distance < 0.1f)
            {
                currentPoint++;
                if (currentPoint >= pathModel.points.Length)
                {
                    spawnSpark.RestoreColors();
                    stop = true;
                    reached?.Invoke();
                }
                else
                {
                    currentDirection = (pathModel.points[currentPoint] - transform.position).normalized;
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
