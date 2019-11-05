using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] private FillImage fillImage;
    [SerializeField] private GameObject Projectile;
    [SerializeField] private float projectileSpeed = 10.0f;
    [SerializeField] private float heightOffset = 0.3f;
    [SerializeField] private float maxDistance = 50.0f;
    [SerializeField] private float chargeTime = 0.5f;
    [SerializeField] private int startingAmmo = 5;
    private int chargeLevel = 0;
    public int ChargeLevel{
        get => Mathf.Clamp(chargeLevel, 0, ammo);
        private set{
            chargeLevel = Mathf.Clamp(value, 0, ammo);
        }
    }
    public bool Charging { get; private set; }
    private float timer = 0f;
    [SerializeField]private int ammo = 0;
    public int Ammo {
        get => ammo;
        private set{
            ammo = Mathf.Clamp(value, 0, int.MaxValue);
        }
    }

    public bool ChargeShot(){
        if(ammo <= 0)
            return false;

        ChargeLevel = 1;
        timer = 0f;
        Charging = true;
        GetComponent<Slime>()?.SetState(Slime.SlimeState.Charging);
        transform.Find("Arrow").gameObject.SetActive(true);
        return true;
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
        transform.Find("Arrow")?.gameObject.SetActive(false);
        //shoot projectile on launchDirection
        GameObject proj = Instantiate(Projectile, new Vector3(transform.position.x, transform.position.y + heightOffset, transform.position.z), transform.rotation);
        (proj.GetComponent<IProjectile>())?.Launch(transform.forward, projectileSpeed, maxDistance);
        (proj.GetComponent<Slime>())?.Grow(ChargeLevel);
        // A função de shrink já diminui a munição
        //ammo -= ChargeLevel;
        GetComponent<Slime>()?.Shrink(ChargeLevel);
        ChargeLevel = 0;
    }

    public void Reload(int count){
        Ammo += count;
    }

    void Awake(){
        Ammo = startingAmmo;
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
            RaycastHit[] objectsHit = Physics.RaycastAll(mouseRay, CameraFollow.raycastDistance, LayerMask.GetMask("Ground"));
            Vector3 targetPosition;
            if(objectsHit.Length > 0){
                targetPosition = objectsHit[0].point;
            }else{
                targetPosition = transform.position + transform.forward;
            }
            transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));

            timer += Time.deltaTime;
        }

        // Debug.Log("ammo = " + ammo + "; chargeLevel = " + chargeLevel);
        if(fillImage != null){
            fillImage.MaxValue = ammo;
            fillImage.CurrentValue = chargeLevel;
        }
    }
}