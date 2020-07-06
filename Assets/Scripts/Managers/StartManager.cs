using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [SerializeField]
    GameObject warning;
    [SerializeField]
    GameObject LevelsPanel;
    public int testLevel;
    public Text scoreBoard;
    private void Start()
    {
        for (int i = 1; i <= 10; i++) { 
            string score = PlayerPrefs.GetString("Score_" + i, "");
            if (score.Length > 0)
            {
                scoreBoard.text += score + "\n";
            }
        }
        //Invoke("Test", 5);
        //OVRInput.GetConnectedControllers();
        //Debug.Log("Connected controllers: " + OVRInput.GetConnectedControllers().ToString());

        LevelsPanel.SetActive(true);
    }
    private void Update()
    {
        /*if (OVRInput.IsControllerConnected(OVRInput.Controller.RTouch) && OVRInput.IsControllerConnected(OVRInput.Controller.LHand))
        {
            warning.SetActive(false);
            LevelsPanel.SetActive(true);
        }
        else
        {
            warning.SetActive(true);
            LevelsPanel.SetActive(false);
        }*/
    }
    private void Test()
    {
        OnLevelEnter(4);
    }
    public void OnLevelEnter(int level)
    {
        PlayerPrefs.SetInt("Level", level);
        SceneManager.LoadScene("Level1");
    }
    public void LoadCalibrate()
    {
        SceneManager.LoadScene("Calibrate");
    }
}
