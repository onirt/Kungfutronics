using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
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
