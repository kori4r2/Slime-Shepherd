using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 50.0f;
    [SerializeField] private GameObject Projectile;
    [SerializeField] private float projectileSpeed = 10.0f;
    [SerializeField] private float heightOffset = 0.3f;
    [SerializeField] private float maxDistance = 50.0f;
    [SerializeField] private float chargeTime = 0.5f;
    [SerializeField] private int startingCharge = 5;
    private int chargeLevel = 0;
    private int ChargeLevel{
        get => chargeLevel;
        set{
            chargeLevel = Mathf.Clamp(value, 0, ammo);
        }
    }
    public bool Charging { get; private set; }
    private float timer = 0f;
    private int ammo = 0;
    public int Ammo {
        get => ammo;
        private set{
            ammo = Mathf.Clamp(value, 0, int.MaxValue);
        }
    }

    public void ChargeShot(){
        if(ammo <= 0)
            return;

        ChargeLevel = 1;
        timer = 0f;
        Charging = true;
        GetComponent<Slime>()?.SetState(Slime.SlimeState.Charging);
        transform.Find("Arrow").gameObject.SetActive(true);
    }

    public void CancelCharge(){
        Charging = false;
        GetComponent<Slime>()?.SetState(Slime.SlimeState.Following);
        transform.Find("Arrow").gameObject.SetActive(false);
        ChargeLevel = 0;
    }

    public void Shoot(){
        if(!Charging)
            return;

        Charging = false;
        GetComponent<Slime>()?.SetState(Slime.SlimeState.Following);
        transform.Find("Arrow").gameObject.SetActive(false);
        ammo -= ChargeLevel;
        GetComponent<Slime>()?.Shrink(chargeLevel);
        //shoot projectile on launchDirection
        GameObject proj = Instantiate(Projectile, new Vector3(transform.position.x, transform.position.y + heightOffset, transform.position.z), transform.rotation);
        (proj.GetComponent<IProjectile>())?.Launch(transform.forward, projectileSpeed, maxDistance);
        (proj.GetComponent<Slime>())?.Grow(chargeLevel-1);
    }

    public void Reload(int count){
        if(count > 0)
            Ammo += count;
    }

    void Awake(){
        Ammo = startingCharge;
        transform.Find("Arrow")?.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Charging){
            if(timer >= chargeTime){
                ChargeLevel++;
                timer -= chargeTime;
            }

            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] objectsHit = Physics.RaycastAll(mouseRay, raycastDistance, LayerMask.GetMask("Ground"));
            Vector3 targetPosition;
            if(objectsHit.Length > 0){
                targetPosition = objectsHit[0].point;
            }else{
                targetPosition = transform.position + transform.forward;
            }
            transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));

            timer += Time.deltaTime;
        }
    }
}