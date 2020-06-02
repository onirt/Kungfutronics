using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SparkModel", menuName = "Spark/SparkModel")]
public class SparkModel : ScriptableObject
{

    public enum SparkType
    {
        Normal,
        Special,
        Anomaly
    };
    public SparkType type;
    public float life;
    public float defaultLife;
    public float minThreshold;
    public float maxThreshold;
    public float scale;
    public float defaultScale;
    public float minScale;
    public float maxScale;
    public int points;
    public float force;
    public GameObject prefab;
    public bool debug;
    public AudioClip sound; 

    public float GetScale()
    {
        if (debug)
            Debug.Log(type + " scale:" + scale);
        return scale;
    }
}
