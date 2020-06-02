using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekBehaviour : MonoBehaviour
{
    public Vector3 offset;

    public Transform target;
    public int mode = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
            return;
        //transform.LookAt(target.position);
        switch (mode) {
            case 0:
                transform.forward = target.forward;
                transform.position = target.position + transform.right * offset.x + transform.up * offset.y + transform.forward * offset.z;
                break;
            case 1:
                transform.position = target.position + transform.right * offset.x + transform.up * offset.y + transform.forward * offset.z;
                break;
            case 2:
                transform.forward = new Vector3(target.forward.x , 0, target.forward.z).normalized;
                transform.position = new Vector3(target.position.x, transform.position.y, target.position.x) + transform.right * offset.x + transform.up * offset.y + transform.forward * offset.z;
                break;
        }
        //transform.Rotate(0,180,0);
    }
}
