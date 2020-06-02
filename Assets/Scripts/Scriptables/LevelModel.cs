using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Game/Level", order = 1)]
public class LevelModel : ScriptableObject
{
    /*[SerializeField]
    private Material skybox;*/
    [SerializeField]
    private Color plataformColor;
    [SerializeField]
    private Color fogColor;
    [SerializeField]
    private Color skyColor;
    [SerializeField]
    private SparkDataModel sparkDataModel;
    [SerializeField]
    private float delay;
    [SerializeField]
    private float offset;
    [SerializeField]
    [Range(0, 1)]
    private float threshold;
    [SerializeField]
    [Range(0, 1)]
    private float scale;
    [SerializeField]
    [Range(0, 10)]
    private int enemies;
    [SerializeField]
    private bool MusicSyc;
    [SerializeField]
    private int level;

    /*public Material getSkybox (){
        return skybox;
    }*/
    public Color getFogColor()
    {
        return fogColor;
    }
    public Color getSkyColor()
    {
        return skyColor;
    }
    public Color getPlataformColor()
    {
        return plataformColor;
    }

    public void SetUpLevel()
    {
        foreach (SparkModel model in sparkDataModel.models)
        {
            string tag = model.type.ToString();
            PlayerPrefs.SetFloat(getTag(tag + "_Th"), getLife(model));
            PlayerPrefs.SetFloat(getTag(tag), getScale(model));
        }
        Debug.Log("Setting Enemies: " + getTag("Enemies") + " enemies: " + enemies);
        PlayerPrefs.SetInt(getTag("Enemies"), enemies);
        PlayerPrefs.SetFloat(getTag("Offset"), offset);
        PlayerPrefs.SetInt(getTag("MusicSyc"), MusicSyc ? 1 : 0);
        PlayerPrefs.SetFloat(getTag("Delay"), delay);
        
    }
    private float getScale(SparkModel model)
    {
        //model.scale = model.defaultScale = model.minScale + (model.maxScale - model.minScale) * scale;

        float s;
        switch (model.type)
        {
            default:
                s = model.minScale + (model.maxScale - model.minScale) * scale;
                break;
        }
        return s;
    }
    private float getLife(SparkModel model)
    {
        model.life = model.defaultLife = model.minThreshold + (model.maxThreshold - model.minThreshold) * threshold;
        float t = model.minThreshold + (model.maxThreshold - model.minThreshold) * threshold;
        return model.life;
    }
    public string getTag(string tag)
    {
        return "L" + level + "_" + tag;
    }

}
