using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Path", menuName = "Path")]
public class PathModel : ScriptableObject
{
    public bool update;
    public Vector3[] points;

}
