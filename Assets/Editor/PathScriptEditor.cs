using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PosistionExtractor))]
public class PathScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PosistionExtractor posistionExtractor = (PosistionExtractor)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Create Path")) {
            posistionExtractor.pathModel.points = new Vector3[posistionExtractor.path.childCount];
            for (int i=0; i < posistionExtractor.path.childCount; i++)
            {
                Vector3 position = posistionExtractor.path.GetChild(i).position;
                posistionExtractor.pathModel.points[i] = new Vector3(position.x, position.y, position.z);
            }
        }
}

}
