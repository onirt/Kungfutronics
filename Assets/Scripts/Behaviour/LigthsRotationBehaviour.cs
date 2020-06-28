using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LigthsRotationBehaviour : MonoBehaviour
{
    [SerializeField]
    AudioSpectrumColorModel audioSpectrum;
    Quaternion intensityRot;
    float speed;

    void Start()
    {
        audioSpectrum.intensityAction += SetIntentity;
    }
    private void OnDestroy()
    {
        audioSpectrum.intensityAction -= SetIntentity;
    }

    void Update()
    {

        transform.rotation = Quaternion.Lerp(transform.rotation, intensityRot, Time.time * speed);
    }

    public void SetIntentity(float intensity)
    {
        intensity = intensity * 100;
        intensityRot = Quaternion.Euler(intensity, intensity, intensity);
        speed = intensity * 10;
    }
}
