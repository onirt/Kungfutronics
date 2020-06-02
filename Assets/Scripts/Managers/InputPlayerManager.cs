using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputPlayerManager : MonoBehaviour
{
    /*[SerializeField]
    private OVRInput.Controller m_controller;

    [SerializeField]
    private LaserPointer laserPointer;
    private float last_flex;*/
    [SerializeField]
    Transform tracker;
    private void Start()
    {
        /*Vector3 leftHandMax = new Vector3(PlayerPrefs.GetFloat("LeftHandX"), PlayerPrefs.GetFloat("LeftHandY"), PlayerPrefs.GetFloat("LeftHandZ"));
        Vector3 rightHandMax = new Vector3(PlayerPrefs.GetFloat("RightHandX"), PlayerPrefs.GetFloat("RightHandY"), PlayerPrefs.GetFloat("RightHandZ"));
        tracker.GetChild(0).position = leftHandMax;
        tracker.GetChild(1).position = rightHandMax;*/
    }
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One))
        {
            //GameManager.obj.ui.ShowInterface();
            SceneManager.LoadScene("Main");
        }
    }
}
