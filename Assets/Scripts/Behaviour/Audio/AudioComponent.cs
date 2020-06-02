using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioComponent : MonoBehaviour
{
    delegate void ReadyForNext();
    ReadyForNext readyNext;
    public AudioSource audioSource;
    
    public AudioSpectrumModel[] audioSpectrumModel;
    // Start is called before the first frame update
    private void Awake() {
        
        audioSource = GetComponent<AudioSource>();
        foreach (AudioSpectrumModel audioSpectrumModel in audioSpectrumModel){
            readyNext += audioSpectrumModel.ReadyForNext;
        }
    }

    void UpdateSpectrum(float[] spectrum, int bufferSampleSize)
    {
        foreach (AudioSpectrumModel audioSpectrumModel in audioSpectrumModel){
            if (!audioSpectrumModel.Already()){
                audioSpectrumModel.UpdateSpectumIntensity(spectrum, bufferSampleSize);
            }
            audioSpectrumModel.UpdateSpectrumVisualizer();
        }
        readyNext();
    }
}
