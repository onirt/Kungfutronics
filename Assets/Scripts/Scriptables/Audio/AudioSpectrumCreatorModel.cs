using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpectrumCreator", menuName = "Spectrums/SpectrumCreator", order = 1)]
public class AudioSpectrumCreatorModel : AudioSpectrumModel
{
    public float deepPosition;
    public float scaleFactor2;
    public SparkDataModel sparkDataModel;
    public bool isUpdated;
    private float[] intensities;
    public float radius;
    public float instantiateDelay = 0.2f;
    private float activeTime;
    private bool delayActived;
    private bool musicSync = true;
    private float height;
    private float deltaAngle;

    public DataFrequencyHelper[] dataFrequencyHelpers;


    public override void Initiate()
    {
        intensities = new float[sparkDataModel.models.Length];
        deltaAngle = 360f / intensities.Length;
        musicSync = PlayerPrefs.GetInt(GamePlayManager.GetLevelTag("MusicSyc"), 1) == 1;
        radius = PlayerPrefs.GetFloat(GamePlayManager.GetLevelTag("Offset"), 0.05f);
        if (!musicSync)
        {
            instantiateDelay = PlayerPrefs.GetFloat(GamePlayManager.GetLevelTag("Delay"), 0.5f);
            deepPosition = 20f;
        }
        else
        {
            instantiateDelay = PlayerPrefs.GetFloat(GamePlayManager.GetLevelTag("Delay"), 0.2f);
            deepPosition = 10f;
        }
        sparkDataModel.InitData();
        foreach (DataFrequencyHelper data in dataFrequencyHelpers)
        {
            data.Init();
        }
        height = GamePlayManager.obj.player.position.y - 0.1f;
        Debug.Log(GamePlayManager.GetLevelTag("Offset") + ".radius: " +  radius);
    }

    protected override int GetLength()
    {
        return sparkDataModel.models.Length;
    }

    public override void UpdateSpectrumVisualizer()
    {
        if (!delayActived)
        {

            if (!musicSync)
            {
                InstantiateModel(Random.Range(0, sparkDataModel.models.Length));
                return;
            }
            for (int i = intensities.Length - 1; i >= 0; i--)
            {
                if (GamePlayManager.obj.test && i < dataFrequencyHelpers.Length)
                     dataFrequencyHelpers[i].Process(intensities[i]);
                if (intensities[i] > sparkDataModel.models[i].life)
                {
                    InstantiateModel(i);
                    return;
                }
            }
        }
        else
        {
            activeTime += Time.deltaTime;
            if (activeTime > instantiateDelay)
            {
                delayActived = false;
                activeTime = 0;
            }
        }

    }
    private void InstantiateModel(int i)
    {
        //int ii = (sparkDataModel.models[i].type == SparkModel.SparkType.Anomaly) ? Random.Range(0,intensities.Length):i;
        /*float t = ii * deltaAngle;
        float a = t * Mathf.PI * 2f / 360f;
        Debug.Log(i + " t: " + t + " a: " + a);
        Vector2 position = new Vector2(Mathf.Sin(a) * radius, Mathf.Cos(a) * radius * 0.35f);
        int j = (int) position.magnitude / sparkDataModel.pathModels.Length;*/
        sparkDataModel.InstantiatModel(i);
        delayActived = true;
    }
    public override bool Already()
    {
        return isUpdated;
    }
    protected override void isDone()
    {
        isUpdated = true;
    }
    public override void ReadyForNext()
    {
        isUpdated = false;
    }
    public override float[] GetIntensities()
    {
        return intensities;
    }
    public override void Finish()
    {

        foreach (DataFrequencyHelper data in dataFrequencyHelpers)
        {
            data.Finish();
        }
    }
}
