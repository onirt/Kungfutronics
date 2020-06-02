using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleBehaviour : MonoBehaviour
{
    private Vector3 targetScale;
    private Vector3 minScale;
    private Vector3 maxScale;
    private static Vector3 speed = new Vector3(0.01f, 0.01f, 0.01f);
    // Start is called before the first frame update
    void Start()
    {
        maxScale = transform.localScale;
        minScale = maxScale * 0.8f;
        targetScale = minScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x <= targetScale.x)
        {
            targetScale = maxScale;
            transform.localScale += speed;
        }
        else
        {
            targetScale = minScale;
            transform.localScale -= speed;
        }
    }
}
