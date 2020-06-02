using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioSpectrumModel : ScriptableObject
{
    
    public float samplePercentage;
    public float emphasisMultiplier;
    public float scaleFactor;
    public float maximunIntensity;
    public float retractionSpeed;
    private float sampleRate;

    // Start is called before the first frame update
    private void OnEnable() {
        sampleRate = AudioSettings.outputSampleRate;
    }
    public abstract void Initiate();

    // Update is called once per frame
    public void UpdateSpectumIntensity(float[] spectrum, int samples)
    {
        float[] intensities = GetIntensities();
        int iteration = 0;
        int indexOnSpectrum = 0;
        int length = GetLength();
        int averagevalue = (int)(Mathf.Abs(samples * samplePercentage) / length);
        if (averagevalue < 1)
        {
            averagevalue = 1;
        }
        while (iteration < length)
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
        isDone();
    }
    protected abstract void isDone();
    public abstract void ReadyForNext();
    public abstract bool Already();
    protected abstract int GetLength();
    public abstract void UpdateSpectrumVisualizer();
    public abstract float[] GetIntensities();
    public abstract void Finish();
    
}
