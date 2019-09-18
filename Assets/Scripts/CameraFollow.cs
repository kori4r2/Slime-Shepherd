using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    private new Camera camera;
    [SerializeField] private Transform target;
    private Vector3 distanceToTarget;

    void Awake(){
        camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        target = Shepherd.instance.transform;
        distanceToTarget = transform.position - target.position;
    }

    void LateUpdate(){
        // To do: Colocar uma liberdade na camera, ou um lookahead
        transform.position = target.position + distanceToTarget;
    }
}
