using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shepherd : MonoBehaviour
{
    public static Shepherd instance;
    [SerializeField] private Launcher launcher;
    [SerializeField] private float slimeCallRadius = 10f;
    [SerializeField] private GameObject slimeCallParticles = null;
    private Animator animator;
    private Movable movable;
    // Start is called before the first frame update
    void Awake()
    {
        movable = GetComponent<Movable>();
        animator = GetComponent<Animator>();
        instance = this;
    }

    public void UnlockMovement(){
        movable.CanMove = true;
    }

    public void Lost(){
        if(launcher.Charging){
            launcher.CancelCharge();
        }
        animator.SetTrigger("Lost");
        movable.CanMove = false;
    }

    public void Stop(){
        if(launcher.Charging){
            animator.SetTrigger("AimCancel");
            launcher.CancelCharge();
        }
        movable.CanMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            if(movable.CanMove && launcher.ChargeShot()){
                animator.SetTrigger("Aim");
                movable.CanMove = false;
            }
        }else if(Input.GetMouseButtonDown(1)){
            if(launcher.Charging){
                animator.SetTrigger("AimCancel");
                launcher.CancelCharge();
            }else if(movable.CanMove){
                // Raycast to the ground and callback slimes
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] objectsHit = Physics.RaycastAll(mouseRay, CameraFollow.raycastDistance, LayerMask.GetMask("Ground"));
                if(objectsHit.Length > 0){
                    Vector3 capsuleBottom = objectsHit[0].point;
                    GameObject obj = Instantiate(slimeCallParticles, capsuleBottom, Quaternion.identity);
                    obj.transform.localScale = new Vector3(slimeCallRadius, 1.0f, slimeCallRadius);
                    Vector3 capsuleTop = new Vector3(capsuleBottom.x, capsuleBottom.y + 1, capsuleBottom.z);
                    foreach(Collider col in Physics.OverlapCapsule(capsuleBottom, capsuleTop, slimeCallRadius, LayerMask.GetMask("Slime"))){
                        Slime slime = col.GetComponent<Slime>();
                        if(slime.CurrentState == Slime.SlimeState.Idle){
                            slime.SetState(Slime.SlimeState.Returning);
                        }   
                    }
                    
                    animator.SetTrigger("Call");
                    movable.CanMove = false;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(capsuleBottom-transform.position, transform.up)), 1);
                }
            }
        }else if(Input.GetMouseButtonUp(0)){
            if(launcher.Charging){
                animator.SetTrigger("AimConfirm");
                launcher.Shoot();
            }
        }

        if(launcher.Charging){
            // Raycast to the ground and callback slimes
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] objectsHit = Physics.RaycastAll(mouseRay, CameraFollow.raycastDistance, LayerMask.GetMask("Ground"));
            if(objectsHit.Length > 0){
                Vector3 capsuleBottom = objectsHit[0].point;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(capsuleBottom-transform.position, transform.up)), movable.TurnRate);
            }
        }
    }
}
