using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizationManager1 : MonoBehaviour
{

    public int bufferSampleSize;
    public float samplePercentage;
    public float emphasisMultiplier;
    public float bufferSizeArea;
    public float maximunIntensity;
    public float retractionSpeed;
    private float sampleRate;
    private float[] samples;
    private float[] spectrum;
    private float[] intensities;
    public float r;
    public float g;
    public float b;
    public Material[] materials;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sampleRate = AudioSettings.outputSampleRate;
        samples = new float[bufferSampleSize];
        spectrum = new float[bufferSampleSize];
        InitiateColor();

    }
    private void InitiateColor()
    {

        intensities = new float[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            //float colorFactor = (materials.Length / (i + 1f));
            Color newColor = new Color(r, g, b, 1.0f);
            materials[i].color = newColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        UpdateIntensity();
        UpdateColor();
    }
    private void UpdateIntensity()
    {
        int iteration = 0;
        int indexOnSpectrum = 0;
        int averagevalue = (int)(Mathf.Abs(samples.Length * samplePercentage) / materials.Length);
        if (averagevalue < 1)
        {
            averagevalue = 1;
        }
        while (iteration < materials.Length)
        {
            int iterationIndex = 0;
            float sumValueY = 0;
            while (iterationIndex < averagevalue)
            {
                sumValueY += spectrum[indexOnSpectrum];
                indexOnSpectrum++;
                iterationIndex++;
            }
            float y = sumValueY / averagevalue * emphasisMultiplier;
            intensities[iteration] -= retractionSpeed * Time.deltaTime;
            Debug.Log(iteration + " intensity: " + intensities[iteration] + " y: " + y);
            if (intensities[iteration] < y)
            {
                intensities[iteration] = y;
            }
            if (intensities[iteration] > maximunIntensity)
            {
                intensities[iteration] = maximunIntensity;
            }
            iteration++;
        }
    }
    private void UpdateColor()
    {
        for (int i = 0; i < materials.Length; i++)
        {

            Debug.Log(i + " final intensity: " + intensities[i]);
            Color colorInensity = new Color(intensities[i] * r, intensities[i] * g, intensities[i] * b);
            materials[i].SetColor("_EmissionColor", colorInensity);

        }
    }
    private float Spacing(float radius)
    {
        float c = 2f * Mathf.PI * radius;
        float n = materials.Length;
        return c / n;
    }
}
