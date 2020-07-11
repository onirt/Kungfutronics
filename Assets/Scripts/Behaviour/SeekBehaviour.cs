using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekBehaviour : MonoBehaviour
{
    public Vector3 offset;

    public Transform target;
    public int mode = 0;
    private Rigidbody _rigidbody;
    [SerializeField]
    private float sensitivity = 100f;
    private Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        if (mode == 3)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

    }

    private void FixedUpdate()
    {
        if (mode == 3)
        {
            Vector3 destination = target.transform.position;
            _rigidbody.transform.rotation = transform.rotation;

            velocity = (destination - _rigidbody.transform.position) * sensitivity;

            _rigidbody.velocity = velocity;
            transform.rotation = target.transform.rotation;
        }
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
