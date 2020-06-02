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

        GameManager.DebugApp("MUSIC", "Audio Spectrum Awake: " + name);
    }
    private void Start()
    {
        Configure();
        GameManager.obj.gameEnded += GameEnded;
        GameManager.obj.gameStarted += GameStarted;
    }
    private void OnDestroy()
    {
        GameManager.obj.gameEnded -= GameEnded;
        GameManager.obj.gameStarted -= GameStarted;
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
        yield return new WaitForSeconds(GameManager.obj.startDelay);
        GameManager.DebugApp("MUSIC", "Delayed Music Played From: " + name);
        audioSource.Play();
    }
    public void GameStarted()
    {

        GameManager.DebugApp("MUSIC", "Starting Audio Spectrum: " + name);
        foreach (AudioSpectrumModel audioModel in audioModels)
        {
            audioModel.Initiate();

            GameManager.DebugApp("MUSIC", "audioModel Initiate: " + audioModel.name);
        }
        if (!audioSource.isPlaying)
        {
            GameManager.DebugApp("MUSIC", "Not PLaygin yet: " + name);
            if (PlayerPrefs.GetInt(GameManager.GetLevelTag("MusicSyc"), 1) == 1)
            {
                GameManager.DebugApp("MUSIC", "Music Sync enabled!!!!");
                if (delayed)
                {
                    GameManager.DebugApp("MUSIC", "Music Delayed");
                    if (GameManager.obj.startDelay > 0)
                    {
                        GameManager.DebugApp("MUSIC", "Start Music Delay" + name);
                        StartCoroutine(WaitFor());
                        return;
                    }
                }
            }
            GameManager.DebugApp("MUSIC", "Music Played From: " + name);
            audioSource.Play();
            return;
        }

        GameManager.DebugApp("MUSIC", "Already Playgin: " + name);
    }
    public void GameEnded()
    {
        GameManager.DebugApp("MUSIC", "Music Stop");
        audioSource.Stop();
        foreach (AudioSpectrumModel audioModel in audioModels){
            audioModel.Finish();
        }
    }
}
