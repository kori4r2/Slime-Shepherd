using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shepherd : MonoBehaviour
{
    public static Shepherd instance;
    [SerializeField] private Launcher launcher;
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
        }else if(Input.GetMouseButtonDown(1)){
            launcher.CancelCharge();
        }else if(Input.GetMouseButtonUp(0)){
            launcher.Shoot();
        }
    }
}
