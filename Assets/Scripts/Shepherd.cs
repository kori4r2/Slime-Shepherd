using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shepherd : MonoBehaviour
{
    public static Shepherd instance;
    [SerializeField] private Launcher launcher;
    [SerializeField] private float slimeCallRadius = 10f;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            launcher.ChargeShot();
            GetComponent<Movable>().CanMove = false;
        }else if(Input.GetMouseButtonDown(1)){
            if(launcher.Charging)
                launcher.CancelCharge();
            else{
                // Raycast to the ground and callback slimes
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] objectsHit = Physics.RaycastAll(mouseRay, CameraFollow.raycastDistance, LayerMask.GetMask("Ground"));
                if(objectsHit.Length > 0){
                    Vector3 capsuleBottom = objectsHit[0].point;
                    Vector3 capsuleTop = new Vector3(capsuleBottom.x, capsuleBottom.y + 1, capsuleBottom.z);
                    foreach(Collider col in Physics.OverlapCapsule(capsuleBottom, capsuleTop, slimeCallRadius, LayerMask.GetMask("Slime"))){
                        Slime slime = col.GetComponent<Slime>();
                        if(slime.CurrentState == Slime.SlimeState.Idle/* ||  slime.CurrentState == Slime.SlimeState.Attacking*/){
                            slime.SetState(Slime.SlimeState.Returning);
                        }   
                    }
                }
            }
        }else if(Input.GetMouseButtonUp(0)){
            launcher.Shoot();
            GetComponent<Movable>().CanMove = true;
        }
    }
}
