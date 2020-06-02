using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class ConfigurationManager : MonoBehaviour
{
    public AudioSpectrumColorModel audioSpectrumColor;
    public AudioSpectrumCreatorModel audioSpectrumCreator;
    private GameObject currentModel;
    [SerializeField]
    private LevelModel[] levels;
    private int Version = 1;


    private void Start()
    {
        audioSpectrumColor.SetEmissionColor(Color.black);
        //PlayerPrefs.DeleteAll();
        if (PlayerPrefs.GetInt("Version") != Version)
        {
            for (int i = 0; i < levels.Length; i++)
            {

                levels[i].SetUpLevel();
            }

            PlayerPrefs.SetInt("Version", Version);
            //ConfigSave();
            PlayerPrefs.Save();
        }
        /*if (isSaved())
        {
            LoadFromFile();
        }*/
    }
    /*public Color SetEnviroment()
    {
        LevelModel level = levels[GameManager.obj.level];
        //RenderSettings.skybox = level.getSkybox();
        RenderSettings.fogColor = level.getFogColor();
        if (RenderSettings.skybox.HasProperty("_Tint"))
            RenderSettings.skybox.SetColor("_Tint", level.getSkyColor());
        else if (RenderSettings.skybox.HasProperty("_SkyTint"))
            RenderSettings.skybox.SetColor("_SkyTint", level.getSkyColor());
        return RenderSettings.fogColor;
    }*/

    /*public void SetScale(int index, float scale)
    {
        audioSpectrumCreator.sparkDataModel.models[index].scale = scale;
        PlayerPrefs.SetFloat(GameManager.GetLevelTag(audioSpectrumCreator.sparkDataModel.models[index].type.ToString()), scale);
        currentModel.transform.localScale = new Vector3(scale, scale, scale);
    }
    public void SetThreshold(int index, float threshold)
    {
        SparkModel model = audioSpectrumCreator.sparkDataModel.models[index];
        model.life = threshold;
        PlayerPrefs.SetFloat(GameManager.GetLevelTag(model.type.ToString() + "_Th"), threshold);
        GameManager.obj.ui.UIDebug("Setting Threshold: " + threshold + " Food: " + model.type.ToString());
    }
    public void SetRadiusOffset(float radius)
    {
        audioSpectrumCreator.radius = radius;
        PlayerPrefs.SetFloat(GameManager.GetLevelTag("Offset"), radius);
    }
    public SparkModel InstantiateModel(int index)
    {
        if (currentModel != null) Destroy(currentModel);
        SparkModel model = audioSpectrumCreator.sparkDataModel.models[index];
        currentModel = Instantiate(model.prefab, GameManager.obj.player.position + Vector3.forward, Quaternion.identity);
        currentModel.GetComponent<PathBehaviour>().enabled = false;
        currentModel.AddComponent<RoationShowBehaviour>();
        currentModel.transform.localScale = new Vector3(model.scale, model.scale, model.scale);
        return model;
    }
    public void SetMusicSync(int sync)
    {
        PlayerPrefs.SetInt(GameManager.GetLevelTag("MusicSyc"), sync);
    }
    public void SetSpawnTime(float time)
    {
        PlayerPrefs.SetFloat(GameManager.GetLevelTag("Delay"), time);
    }*/
    public void Restart()
    {
        levels[0].SetUpLevel();
        //ConfigSave();
        /*foreach (FoodModel food in audioSpectrumCreator.foodsModel.foods)
        {
            food.life = food.defaultLife;
            food.life = food.defaultScale;
        }
        audioSpectrumCreator.radius = 0.05f;*/
    }
    public LevelModel GetLevel(int i)
    {
        return levels[i];
    }
    public SparkModel GetFoodModel(int index)
    {
        return audioSpectrumCreator.sparkDataModel.models[index];
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    /*
    public void ConfigSave()
    {
        PlayerPrefs.Save();
        SaveToFile();

        GameManager.obj.ui.UIDebug("Saved Finish.");
    }
    public bool isSaved()
    {
        return File.Exists(Application.persistentDataPath + "/levelInfo.dat");
    }
    
    public void SaveToFile()
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.OpenOrCreate);
        LevelData data = new LevelData();
        data.level = GameManager.obj.level;
        data.delay = PlayerPrefs.GetFloat(GameManager.GetLevelTag("Delay"));
        data.offset = PlayerPrefs.GetFloat(GameManager.GetLevelTag("Offset"));
        data.musicsync = PlayerPrefs.GetInt(GameManager.GetLevelTag("MusicSyc"));
        data.sparks = new Spark[audioSpectrumCreator.sparkDataModel.models.Length];
        for (int i = 0; i < data.sparks.Length; i++)
        {
            SparkModel model = audioSpectrumCreator.sparkDataModel.models[i];
            data.sparks[i] = new Spark(model.type, model.points,
                                        PlayerPrefs.GetFloat(GameManager.GetLevelTag(model.type.ToString() + "_Th")),
                                        PlayerPrefs.GetFloat(GameManager.GetLevelTag(model.type.ToString())));

        //    GameManager.obj.ui.UIDebug("Saved: " + GameManager.GetLevelTag(data.foods[i].type.ToString()));
        //    GameManager.obj.ui.UIDebug("    life: " + GameManager.GetLevelTag(data.foods[i].life + ""));
        //    GameManager.obj.ui.UIDebug("    scale: " + GameManager.GetLevelTag(data.foods[i].scale + ""));
        }
        binary.Serialize(file, data);
        file.Close();
    }
    public void LoadFromFile()
    {
        if (isSaved())
        {
            BinaryFormatter binary = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.Open);
            LevelData data = (LevelData)binary.Deserialize(file);
            file.Close();
            GameManager.obj.level = data.level;
            PlayerPrefs.SetFloat(GameManager.GetLevelTag("Delay"), data.delay);
            PlayerPrefs.SetFloat(GameManager.GetLevelTag("Offset"), data.offset);
            PlayerPrefs.SetFloat(GameManager.GetLevelTag("MusicSyc"), data.musicsync);
            for (int i = 0; i < data.sparks.Length; i++)
            {
                PlayerPrefs.SetFloat(GameManager.GetLevelTag(data.sparks[i].type.ToString() + "_Th"), data.sparks[i].life);
                PlayerPrefs.SetFloat(GameManager.GetLevelTag(data.sparks[i].type.ToString()), data.sparks[i].scale);
                //GameManager.obj.ui.UIDebug("Loaded: " + GameManager.GetLevelTag(data.foods[i].type.ToString()));
                //GameManager.obj.ui.UIDebug("    life: " + GameManager.GetLevelTag(data.foods[i].life + ""));
                //GameManager.obj.ui.UIDebug("    scale: " + GameManager.GetLevelTag(data.foods[i].scale + ""));
            }
        }
    }
    public void DsiplayData()
    {
        GameManager.obj.ui.UIDebug("------------------------------------");
        GameManager.obj.ui.UIDebug(Time.time + " Level: " + GameManager.obj.level);
        for (int i = 0; i < audioSpectrumCreator.sparkDataModel.models.Length; i++)
        {
            SparkModel model = audioSpectrumCreator.sparkDataModel.models[i];

            GameManager.obj.ui.UIDebug("    Preferfs: " + GameManager.GetLevelTag(model.type.ToString()));
            GameManager.obj.ui.UIDebug("        life: " + PlayerPrefs.GetFloat(GameManager.GetLevelTag(model.type.ToString() + "_Th")));
            GameManager.obj.ui.UIDebug("        scale: " + PlayerPrefs.GetFloat(GameManager.GetLevelTag(model.type.ToString())));
        }
    }*/
}
[Serializable]
class LevelData
{
    public Spark[] sparks;
    public int level;
    public float delay;
    public float offset;
    public int musicsync;


}
