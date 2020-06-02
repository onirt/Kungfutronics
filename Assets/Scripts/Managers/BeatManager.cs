using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public OnEventBeat onBeat;
    public int ringBufferSize = 120;
    public int bufferSize;
    public int samplingRate = 44100;
    public bool limitBeats;
    public int limitedAmount;
    public float beatIndicationThreshold;
    private const int bands = 12;
    private const int maximumLag = 100;
    private const float smoothDecay = 0.997f;
    private AudioSource audioSource;
    private AudioData audioData;
    private int frameSinceBeat;
    private float framePeriod;
    private int currentRingBufferPosition;
    private float[] spectrum;
    private float[] previousSpectrum;
    private float[] averagePowerPerband;
    private float[] onsets;
    private float[] notations;
    // Start is called before the first frame update

    private void Awake()
    {
        onsets = new float[ringBufferSize];
        notations = new float[ringBufferSize];
        spectrum = new float[bufferSize];
        averagePowerPerband = new float[bands];

        audioSource = GetComponent<AudioSource>();
        samplingRate = audioSource.clip.frequency;
        framePeriod = (float)bufferSize / samplingRate;
        previousSpectrum = new float[bands];
        for (int i = 0; i < bands; i++)
        {
            previousSpectrum[i] = 100f;
        }
        audioData = new AudioData(maximumLag, smoothDecay, framePeriod, BandWidth() * 2);
    }
    private float BandWidth()
    {
        return (2f / bufferSize) * (samplingRate / 2f) * .5f;
    }
    private void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        for (int i = 0; i < bands; i++)
        {
            float averagePower = 0;
            float samplingFactor = samplingRate * .5f;
            int lowFrequencyIndex = (i == 0) ? 0 : Mathf.RoundToInt(samplingFactor / Mathf.Pow(2, bands - i));
            int hightFrequencyIndex = Mathf.RoundToInt(samplingFactor / Mathf.Pow(2, bands - 1 - i));

            int lowBound = FrequencyByIndex(lowFrequencyIndex, samplingFactor);
            int hightBound = FrequencyByIndex(hightFrequencyIndex, samplingFactor);

            for (int j=0; j <= hightBound; j++){
                averagePower += spectrum[j];
            }
            averagePower /= (hightBound - lowBound + 1);
            averagePowerPerband[i] = averagePower;
        }
        float onset = 0;
        for (int i = 0; i < bands; i++)
        {
            float spectrumValue = Mathf.Max(-100f, 20f * Mathf.Log10(averagePowerPerband[i] + 160f));
            spectrumValue *= 0.025f;
            float dbIncrement = spectrumValue - previousSpectrum[i];
            previousSpectrum[i] = spectrumValue;
            onset += dbIncrement;
        }
        onsets[currentRingBufferPosition] = onset;
        audioData.UpdateAudioData(onset);
        float maxDelay = 0f;
        int tempo = 0;

        for (int i=0; i<maximumLag; i++){
            float delayVal = Mathf.Sqrt(audioData.DelayAtIndex(i));
            if (delayVal > maxDelay){
                maxDelay = delayVal;
                tempo = i;
            }
        }
        float maximumNotation = -9999;
        int maximumNotationIndex = 0;

        for (int i=Mathf.RoundToInt(tempo * .5f); i < Mathf.Min(ringBufferSize, tempo * 2); i++){
            float notatoinValue = onset + notations[(currentRingBufferPosition - i + ringBufferSize) % ringBufferSize] - (beatIndicationThreshold * 100f) * Mathf.Pow(Mathf.Log(i/tempo), 2);
            if (notatoinValue > maximumNotation){
                maximumNotation = notatoinValue;
                maximumNotationIndex = i;
            }
        }
        notations[currentRingBufferPosition] = maximumNotation;
        float minimumNotation = notations[0];

        for (int i=0; i < ringBufferSize; i++){
            if (notations[i] < minimumNotation){
                minimumNotation = notations[i];
            }
        }
        for (int i=0; i < ringBufferSize; i++){
            notations[i] -= minimumNotation;
        }
        frameSinceBeat++;
        if (maximumNotationIndex == currentRingBufferPosition){
            if (limitBeats){
                if (frameSinceBeat > tempo / limitedAmount){
                    onBeat.Invoke();
                    frameSinceBeat = 0;
                }
            }else{
                onBeat.Invoke();
            }
        }
        currentRingBufferPosition++;
        if (currentRingBufferPosition > bufferSize){
            currentRingBufferPosition = 0;
        }
    }

    public int FrequencyByIndex(int frequencyIndex, float samplingFactor ){
        float badnWdth = BandWidth();
        if (frequencyIndex < badnWdth){
            return 0;
        }
        if (frequencyIndex > samplingFactor - badnWdth){
            return bufferSize / 2;
        }
        float fraction = frequencyIndex / samplingRate;
        return Mathf.RoundToInt(bufferSize * fraction);
    }

}
public class AudioData
{
    private int index;
    private int delayLength;
    private float decay;
    private float[] delays;
    private float[] outputValues;
    private float[] weights;
    private float[] bpms;
    private float octaveWidth;
    private float framePeriod;

    public AudioData(int delayLength, float decay, float framePeriod, float octaveWidth)
    {
        this.octaveWidth = octaveWidth;
        this.decay = decay;
        this.delayLength = delayLength;
        this.framePeriod = framePeriod;
        delays = new float[delayLength];
        outputValues = new float[delayLength];
        bpms = new float[delayLength];
        weights = new float[delayLength];
        index = 0;
        AddWeights();
    }
    private void AddWeights()
    {
        for (int i = 0; i < delayLength; i++)
        {
            bpms[i] = 60f / framePeriod * i;
            weights[i] = Mathf.Exp(-.5f * Mathf.Pow(Mathf.Log(bpms[i] / 120f) / Mathf.Log(2f / octaveWidth), 2f));
        }
    }
    public void UpdateAudioData(float updatedOnset)
    {
        delays[index] = updatedOnset;

        for (int i = 0; i < delayLength; i++)
        {
            int delayIndex = (index - i + delayLength) % delayLength;
            outputValues[i] += (1 - decay) * (delays[index] * delays[delayIndex] - outputValues[i]);
        }
        index++;
        if (index >= delayLength)
        {
            index = 0;
        }
    }
    public float DelayAtIndex(int delayIndex)
    {
        return weights[delayIndex] * outputValues[delayIndex];
    }
}
[System.Serializable]
public class OnEventBeat : UnityEvent
{

}
