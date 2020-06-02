using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public int testLevel;
    private void Start()
    {
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
}
