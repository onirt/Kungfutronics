using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishBehaviour : MonoBehaviour
{
    private AudioSource audioSource;
    bool finished;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        
            if (finished){

            }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.obj.points > 0)
            audioSource.Play();
        finished = true;;
    }
}
