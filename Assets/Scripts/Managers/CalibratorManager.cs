using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalibratorManager : MonoBehaviour
{
    [SerializeField]
    private Text debug;
    [SerializeField]
    private OVRInput.Controller m_controller = OVRInput.Controller.None;
    [SerializeField]
    private Transform leftHand;
    [SerializeField]
    private Transform rightHand;
    [SerializeField]
    private Transform calibrateLeftRef;
    [SerializeField]
    private Transform calibrateRightRef;


    void Start()
    {
        Vector3 leftHandMax = new Vector3(PlayerPrefs.GetFloat("LeftHandX"), PlayerPrefs.GetFloat("LeftHandY"), PlayerPrefs.GetFloat("LeftHandZ"));
        Vector3 rightHandMax = new Vector3(PlayerPrefs.GetFloat("RightHandX"), PlayerPrefs.GetFloat("RightHandY"), PlayerPrefs.GetFloat("RightHandZ"));
        calibrateLeftRef.position = leftHandMax;
        calibrateRightRef.position = rightHandMax;
    }

    void Update()
    {

        float touch = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
        debug.text = "Touch: " + touch + ", " + calibrateLeftRef.position + ", " + calibrateRightRef.position;
        if (touch == 0)
        {
            return;
        }
        PlayerPrefs.SetFloat("LeftHandX", leftHand.position.x);
        PlayerPrefs.SetFloat("LeftHandY", leftHand.position.y);
        PlayerPrefs.SetFloat("LeftHandZ", leftHand.position.z);
        PlayerPrefs.SetFloat("RightHandX", rightHand.position.x);
        PlayerPrefs.SetFloat("RightHandY", rightHand.position.y);
        PlayerPrefs.SetFloat("RightHandZ", rightHand.position.z);
        calibrateLeftRef.position = leftHand.position;
        calibrateRightRef.position = rightHand.position;
    }
}
