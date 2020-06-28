using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LipsInputManager : MonoBehaviour
{
    [SerializeField]
    Text isOn;
    // smoothing amount
    [Range(1, 100)]
    [Tooltip("Smoothing of 1 will yield only the current predicted viseme, 100 will yield an extremely smooth viseme response.")]
    public int smoothAmount = 70;

    [Range(0.0f, 1.0f)]
    [Tooltip("Laughter probability threshold above which the laughter blendshape will be activated")]
    public float laughterThreshold = 0.5f;

    [Range(0.0f, 3.0f)]
    [Tooltip("Laughter animation linear multiplier, the final output will be clamped to 1.0")]
    public float laughterMultiplier = 1.5f;

    [SerializeField]
    OVRLipSyncContextBase lipsyncContext;
    public bool EatOn;
    public float activeTime = 0f;
    private float threshold = 0.7f;
    private ParticleSystem m_particleSystem;
    [SerializeField]
    AudioSource explosion;
    [SerializeField]
    Material bridge;
    [SerializeField]
    float r;
    [SerializeField]
    float g;
    [SerializeField]
    float b;
    // Start is called before the first frame update
    void Start()
    {
        lipsyncContext = GetComponent<OVRLipSyncContextBase>();
        lipsyncContext.Smoothing = smoothAmount;
        //StartCoroutine(forTest());
    }
    IEnumerator forTest()
    {
        yield return new WaitForSeconds(4);
        Eated();
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePlayManager.obj.gameStatus != GamePlayManager.GameStatus.Started) return;
        if ((lipsyncContext != null))
        {
            // get the current viseme frame
            OVRLipSync.Frame frame = lipsyncContext.GetCurrentPhonemeFrame();
            if (frame != null)
            {

                SetLaughterToMorphTarget(frame);
            }

            // Update smoothing value
            if (smoothAmount != lipsyncContext.Smoothing)
            {
                lipsyncContext.Smoothing = smoothAmount;
            }
        }
        if (EatOn)
        {
            activeTime += Time.deltaTime;
            float intensity = (threshold - activeTime) / threshold;
            Color colorInensity = new Color(intensity * r, intensity * g, intensity * b);
            bridge.SetColor("_EmissionColor", colorInensity);
            if (activeTime > threshold)
            {
                EatOn = false;
            }
        }
        if (m_particleSystem.isPlaying)
        {
            if (m_particleSystem.particleCount == 0)
            {
                if (!explosion.isPlaying)
                    explosion.Play();
            }
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Food")
        {
            if (EatOn)
            {
                GamePlayManager.obj.SetPoints(other.gameObject.GetComponent<SparkBehaviour>().sparkModel.points);
                Destroy(other.gameObject);
                m_particleSystem.Play();
            }
        }
    }

    /// <summary>
    /// Sets the laughter to morph target.
    /// </summary>
    void SetLaughterToMorphTarget(OVRLipSync.Frame frame)
    {
        // Laughter score will be raw classifier output in [0,1]
        float laughterScore = frame.laughterScore;

        // Threshold then re-map to [0,1]
        laughterScore = laughterScore < laughterThreshold ? 0.0f : laughterScore - laughterThreshold;
        laughterScore = Mathf.Min(laughterScore * laughterMultiplier, 1.0f);
        laughterScore *= 1.0f / laughterThreshold;

        int max = -1;
        float last = 0;
        for (int i = 0; i < frame.Visemes.Length; i++)
        {
            if (frame.Visemes[i] > last)
            {
                max = i;
                last = frame.Visemes[i];
            }
        }
        if (max > 0)
        //if (max == 10)
        //if (max != -1)
        {
            r = (max & 1) == 1 ? 0.5f : 0;
            g = (max & 2) == 2 ? 0.5f : 0;
            b = ((max & 4) == 4 && (r == 0 || g == 0)) || (r != 0 && g == 0) ? 0.5f : 0;
            if (isOn != null)
            {
                //isOn.text = "Is On! " + max;
                isOn.color = Color.green;
            }
            //GameManager.obj.play.EatIt();
            Eated();
        }
        else if (isOn != null)
        {
            //isOn.text = (max != -1) ? "Is On! " + max : "Is Off";
            isOn.color = Color.red;
        }
    }
    private void Eated()
    {

        activeTime = 0;
        EatOn = true;
    }
}
