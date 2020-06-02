using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SparkData", menuName = "Spark/SparkData")]
public class SparkDataModel : ScriptableObject
{
    public PathModel[] pathModels;
    public SparkModel[] models;
    public float height = 0;

    public void InitData()
    {
        for (int i = 0; i < models.Length; i++)
        {
            SparkModel spark = models[i];
            string tag = spark.type.ToString();
            spark.scale = PlayerPrefs.GetFloat(GameManager.GetLevelTag(tag));
            spark.life = PlayerPrefs.GetFloat(GameManager.GetLevelTag(tag + "_Th"));
        }
    }
    public void InstantiatModel(int selectedModel)
    {
        int selectedPath = Random.Range(0, pathModels.Length);
        GameObject newObject = Instantiate(models[selectedModel].prefab, new Vector3(pathModels[selectedPath].points[0].x, height, pathModels[selectedPath].points[0].z), Quaternion.identity);
        newObject.GetComponent<SparkBehaviour>().sparkModel = models[selectedModel];
        newObject.GetComponent<PathBehaviour>().pathModel = pathModels[selectedPath];

        float scale = models[selectedModel].GetScale();
        newObject.transform.localScale = new Vector3(scale, scale, scale);

    }
}
