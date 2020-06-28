using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpectrumColor", menuName = "Spectrums/SpectrumColor", order = 2)]
public class AudioSpectrumColorModel : AudioSpectrumModel
{
    public delegate void Action(float intensity);
    public Action intensityAction;
    private float r;
    private float g;
    private float b;
    public float _r;
    public float _g;
    public float _b;
    private Transform lights;
    public Material[] materials;
    private static float[] intensities;
    public static bool isUpdated;
    public int selectedIntensity;
    public override void Initiate()
    {
        SetOriginal();
        GamePlayManager.obj.takedDamage += TakedDamage;

        if (intensities == null)
        intensities = new float[materials.Length];
        SetEmissionColor(Color.black);
        lights = GameObject.FindGameObjectWithTag("Lights").transform;
        for (int i =0; i < lights.childCount; i++)
        {
            MeshRenderer renderer = lights.GetChild(i).GetComponent<MeshRenderer>();
            renderer.material = materials[Random.Range(0, materials.Length)];
        }
    }
    public void SetEmissionColor(Color color)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            //float colorFactor = (materials.Length / (i + 1f));
            Color newColor = new Color(r, g, b, 1.0f);
            //materials[i].color = newColor;
            materials[i].SetColor("Color_DADA8D09", color);
        }
    }
    protected override int GetLength()
    {
        return materials.Length;
    }

    public override void UpdateSpectrumVisualizer()
    {

        for (int i = 0; i < materials.Length; i++)
        {
            scaleFactor = i * i * 10f + 1;
            Color colorInensity = new Color(intensities[i] * r * scaleFactor, intensities[i] * g * scaleFactor, intensities[i] * b * scaleFactor);
            //Debug.Log(i + ".- " + materials[i].name +  " intensity: " + intensities[i] + " colorInensity: " + colorInensity.ToString());
            materials[i].SetColor("Color_DADA8D09", colorInensity);
            if (selectedIntensity == i)
            {
                intensityAction?.Invoke(intensities[i]);
            }
        }
    }
    private void SetOriginal()
    {

        r = _r;
        g = _g;
        b = _b;
    }
    public void TakedDamage(bool taked)
    {
        if (taked) {
            r = 1;
            g = 0;
            b = 0;
        }
        else
        {
            r = _r;
            g = _g;
            b = _b;
        }

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

    public override void Finish(){

        GamePlayManager.obj.takedDamage -= TakedDamage;

    }
}
