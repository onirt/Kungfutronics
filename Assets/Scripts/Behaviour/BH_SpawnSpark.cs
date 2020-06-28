using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BH_SpawnSpark : MonoBehaviour
{

    private Color originalColor;
    public Renderer[] renderers;

    private void Start()
    {
        if (renderers.Length > 0)
        originalColor = renderers[0].material.GetColor("Color_4FB40C3");
    }
    public void RestoreColors()
    {
        SetColors(originalColor);
    }
    public void StartColors()
    {
        SetColors(Color.white);
    }
    public void SetInfectedColors()
    {
        SetColors(Color.red);
    }
    private void SetColors(Color color)
    {
        foreach (Renderer render in renderers)
        {
            render.material.SetColor("Color_4FB40C3", color);
        }
    }
}
