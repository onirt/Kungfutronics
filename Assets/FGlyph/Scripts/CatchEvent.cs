using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchEvent : MonoBehaviour {


    private void OnEnable()
    {
        GlyphInput.OnMatchResult += InterpretResult;
    }
    private void OnDisable()
    {
        GlyphInput.OnMatchResult -= InterpretResult;
    }

    public void InterpretResult(string result, float match, float ms)
    {
        Debug.Log("Caught: " + result + " " + match + " ");
    }
}
