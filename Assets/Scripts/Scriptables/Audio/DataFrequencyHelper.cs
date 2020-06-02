using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataHelper", menuName = "Spectrums/DataHelper", order = 3)]
public class DataFrequencyHelper : ScriptableObject
{
    [Range(0,1)]
    public float thresholdPorcent = 0.1f;
    [Range(0,1)]
    public float suggestedThresholdPorcent = 0.3f;
    public float threshold;
    public float suggestedThreshold;
    
    public float offset;
    public float rms;
    public float accum;
    public float max;
    public int maxSeeker = 0;
    public int loop = 0;
    public bool reset;
    public bool disable;

    public void Init()
    {
        if (disable) return;
        if (reset)
        {
            loop = 0;
            rms = 0;
            accum = 0;
            max = 0;
            offset = 0;
            maxSeeker = 0;
            threshold = 0;
            suggestedThreshold = 0;
        
        }
    }
    public void Finish()
    {
        if (disable) return;
        rms = accum / loop;
        threshold = rms + rms * thresholdPorcent;
        offset = max - rms;
        suggestedThreshold = offset * suggestedThresholdPorcent + rms;
    }

    public void Process(float intensity)
    {
        if (disable) return;
        accum += intensity;
        loop++;
        if (intensity > max)
        {
            max = intensity;
            maxSeeker++;
        }
    }

}
