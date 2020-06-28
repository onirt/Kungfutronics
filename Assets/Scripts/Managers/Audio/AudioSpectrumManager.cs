using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpectrumManager : MonoBehaviour
{
    delegate void ReadyForNext();
    ReadyForNext readyNext;
    public AudioSpectrumModel[] audioModels;
    private float[] spectrum;
    public int bufferSampleSize;
    public AudioSource audioSource;
    [SerializeField]
    private bool delayed;

    private void Configure()
    {

        audioSource = GetComponent<AudioSource>();
        spectrum = new float[bufferSampleSize];
        foreach (AudioSpectrumModel audioModel in audioModels)
        {
            readyNext += audioModel.ReadyForNext;
            if (audioModel.GetType() == typeof(AudioSpectrumColorModel))
            {
                audioModel.Initiate();
            }
        }

    }
    private void Start()
    {
        Configure();
        GamePlayManager.obj.gameEnded += GameEnded;
        GamePlayManager.obj.gameStarted += GameStarted;
    }
    private void OnDestroy()
    {
        GamePlayManager.obj.gameEnded -= GameEnded;
        GamePlayManager.obj.gameStarted -= GameStarted;
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            return;
        }
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        foreach (AudioSpectrumModel audioModel in audioModels)
        {
            if (!audioModel.Already())
            {
                audioModel.UpdateSpectumIntensity(spectrum, bufferSampleSize);
            }
            audioModel.UpdateSpectrumVisualizer();
        }
        readyNext();
    }
    IEnumerator WaitFor()
    {
        yield return new WaitForSeconds(GamePlayManager.obj.startDelay);
        audioSource.Play();
    }
    public void GameStarted()
    {

        foreach (AudioSpectrumModel audioModel in audioModels)
        {
            audioModel.Initiate();

        }
        if (!audioSource.isPlaying)
        {
            if (PlayerPrefs.GetInt(GamePlayManager.GetLevelTag("MusicSyc"), 1) == 1)
            {
                if (delayed)
                {
                    if (GamePlayManager.obj.startDelay > 0)
                    {
                        StartCoroutine(WaitFor());
                        return;
                    }
                }
            }
            audioSource.Play();
            return;
        }

    }
    public void GameEnded()
    {
        audioSource.Stop();
        foreach (AudioSpectrumModel audioModel in audioModels){
            audioModel.Finish();
        }
    }
}
