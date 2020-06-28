using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VisualizationMode { Ring, RingWithBeat }
public class AudioVisualizationManager : MonoBehaviour
{
    
    public int bufferSampleSize;
    public float samplePercentage;
    public float emphasisMultiplier;
    public int amountOfSegments;
    public float radius;
    public float bufferSizeArea;
    public float maximunExtendLength;
    public float retractionSpeed;
    public GameObject lineRenderPrefab;
    public Material lineRenderMaterial;
    public VisualizationMode visualizationMode;
    public Gradient colorGradientA = new Gradient();
    public Gradient colorGradientB = new Gradient();
    private Gradient currentColor = new Gradient();
    private float sampleRate;
    private float[] samples;
    private float[] spectrum;
    private float[] extendLengths;
    private LineRenderer[] lineRenderers;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sampleRate = AudioSettings.outputSampleRate;
        samples = new float[bufferSampleSize];
        spectrum = new float[bufferSampleSize];
        switch (visualizationMode)
        {
            case VisualizationMode.Ring:
                InitiateRing();
                break;
            case VisualizationMode.RingWithBeat:
                break;
        }
    }
    private void InitiateRing (){
        extendLengths = new float[amountOfSegments + 1];
        lineRenderers = new LineRenderer[extendLengths.Length];

        for (int i=0; i < lineRenderers.Length; i++){
            GameObject go = Instantiate(lineRenderPrefab);
            LineRenderer lineRenderer = go.GetComponent<LineRenderer>();
            lineRenderer.sharedMaterial = lineRenderMaterial;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderers[i] = lineRenderer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.GetSpectrumData(spectrum,0,FFTWindow.BlackmanHarris);
        UpdateExtends();
        if (visualizationMode == VisualizationMode.Ring){
            UpdateRing();
        }
    }
    private void UpdateExtends(){
        int iteration = 0;
        int indexOnSpectrum = 0;
        int averagevalue = (int) (Mathf.Abs(samples.Length * samplePercentage)/amountOfSegments);
        if (averagevalue < 1){
            averagevalue = 1;
        }
        while (iteration < amountOfSegments){
            int iterationIndex = 0;
            float sumValueY = 0;
            while (iterationIndex < averagevalue){
                sumValueY += spectrum[indexOnSpectrum];
                indexOnSpectrum++;
                iterationIndex++;
            }
            float y = sumValueY / averagevalue * emphasisMultiplier;
            extendLengths[iteration] -= retractionSpeed * Time.deltaTime; 
            Debug.Log(iteration + "extendLengths: " + extendLengths[iteration] + " y: " + y);
            if (extendLengths[iteration] < y){
                extendLengths[iteration] = y;
            }
            if (extendLengths[iteration] > maximunExtendLength){
                extendLengths[iteration] = maximunExtendLength;
            }
            iteration++;
        }
    }
    private void UpdateRing(){
        for (int i=0; i < lineRenderers.Length; i++){
            float t = i / (lineRenderers.Length - 2f);
            float a = t * Mathf.PI * 2f;
            Vector2 direction = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            float maximumRadius = (radius + bufferSizeArea + extendLengths[i]);
            Debug.Log(i + " maximumRadius: " + maximumRadius);
            lineRenderers[i].SetPosition(0, direction * radius);
            lineRenderers[i].SetPosition(1, direction * maximumRadius);
            lineRenderers[i].startWidth = Spacing(radius);
            lineRenderers[i].endWidth = Spacing(maximumRadius);
            lineRenderers[i].startColor = colorGradientA.Evaluate(0);
            lineRenderers[i].endColor = colorGradientA.Evaluate((extendLengths[i] - 1) / (maximunExtendLength - 1f));

        }
    }
    private float Spacing(float radius){
        float c = 2f * Mathf.PI * radius;
        float n = lineRenderers.Length;
        return c / n;
    }
    
}
