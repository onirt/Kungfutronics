using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAnimStartBehaviour : MonoBehaviour
{
    float threshold;
    float currentTime;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        threshold = Random.Range(1, 7);
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > threshold)
        {
            animator.SetTrigger("Start");
            threshold = Random.Range(1, 7);
            currentTime = 0;
        }
    }

    public void AnimationEvent()
    {

    }

}
