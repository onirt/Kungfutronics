using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SparkDataModel))]
public class PathsHeightControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SparkDataModel sparkDataModel = (SparkDataModel) target;
        DrawDefaultInspector();

        if (GUILayout.Button("Update Height"))
        {
            for (int i=0; i < sparkDataModel.pathModels.Length; i++)
            {
                for (int j=0; j < sparkDataModel.pathModels[i].points.Length;j++) {
                    sparkDataModel.pathModels[i].points[j] = new Vector3(sparkDataModel.pathModels[i].points[j].x, sparkDataModel.height, sparkDataModel.pathModels[i].points[j].z);
                }
            }
        }
    }
}
