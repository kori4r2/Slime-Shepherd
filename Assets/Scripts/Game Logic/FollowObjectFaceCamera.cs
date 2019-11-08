using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObjectFaceCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0,1,0);
    protected Transform cameraTransform;
    protected Transform myTransform;

    void Awake()
    {
        myTransform = transform;
        myTransform.SetParent(null);
    }
    

    void Update()
    {
        if(cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        if(cameraTransform != null)
        {
            myTransform.LookAt(cameraTransform,Vector3.up);
        }

        if(target != null)
        {
            myTransform.position = target.position + offset;
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
