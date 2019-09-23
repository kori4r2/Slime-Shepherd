using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteAlways]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Range(0f, 100)] private float distanceToTarget = 15f;
    [SerializeField, Range(0f, 90f)] private float viewingAngle = 60f;
    [SerializeField] private Transform ground;
    public static float raycastDistance { get; private set; }
    public static float baseDistance;
    private static CameraFollow instance = null;

    // Singleton em cada cena
    void Awake(){
        if(!instance){
            instance = this;
        }else{
            Destroy(this);
        }
    }

    void OnDestroy(){
        if(instance == this){
            instance = null;
        }
    }

    void Start()
    {
        transform.eulerAngles = new Vector3(viewingAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        transform.position = target.position - (distanceToTarget * transform.forward);
        baseDistance = transform.position.y - ((ground != null)? ground.position.x : 0f);
        float baseAngle = 90f - viewingAngle;
        float angle = baseAngle + (GetComponent<Camera>().fieldOfView / 2);
        angle = Mathf.Clamp(angle, 0f, 90f - float.Epsilon);
        raycastDistance = baseDistance / Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad));
        transform.position = target.position - (distanceToTarget * transform.forward);
    }

    void LateUpdate(){
#if UNITY_EDITOR
        transform.eulerAngles = new Vector3(viewingAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        baseDistance = transform.position.y - ((ground != null)? ground.position.x : 0f);
        float baseAngle = 90f - viewingAngle;
        float angle = baseAngle + (GetComponent<Camera>().fieldOfView / 2);
        angle = Mathf.Clamp(angle, 0f, 90f - float.Epsilon);
        raycastDistance = baseDistance / Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad));
#endif
        transform.position = target.position - (distanceToTarget * transform.forward);
    }
}
