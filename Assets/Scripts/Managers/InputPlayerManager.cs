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
        if (IsRightie()) {
            //Debug.Log("Is Right Hand");
            if (OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Two) || OVRInput.Get(OVRInput.Button.Start))
            {
                //GameManager.obj.ui.ShowInterface();
                GamePlayManager.obj.ui.ReturnMenuDisplay(true);
            }
        }
        else
        {
            if (OVRInput.Get(OVRInput.RawButton.X) || OVRInput.Get(OVRInput.RawButton.Y) || OVRInput.Get(OVRInput.Button.Start))
            {
                //GameManager.obj.ui.ShowInterface();
                GamePlayManager.obj.ui.ReturnMenuDisplay(true);
            }
        }
    }
    public bool IsRightie()
    {
        OVRPlugin.Handedness handedness = OVRPlugin.GetDominantHand();
        if (handedness == OVRPlugin.Handedness.RightHanded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
