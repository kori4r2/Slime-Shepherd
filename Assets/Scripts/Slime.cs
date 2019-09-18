using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public class Slime : MonoBehaviour, IProjectile, IDamageable
{
    public enum SlimeState{
        Idle,
        Charging,
        Following,
        Flying,
        Attacking,
        Returning
    }

    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private bool isMainBody = false;
    [SerializeField] private float lifeExpectancy = 10f;
    [SerializeField] private int nAttacks = 5;

    public static Slime mainBody;
    private SlimeState currentState = SlimeState.Idle;
    private Rigidbody rb;
    private Collider col;
    private Vector3 initialPosition;
    private float timer;
    private int attacksLeft;
    public int HP {
        get => size;
        private set{
            size = Mathf.Max(value, 0);
        }
    }

    [SerializeField] private int size = 1;

    public void Awake(){
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        initialPosition = transform.position;
    }

    public void Start(){
        if(isMainBody){
            mainBody = this;
            size = GetComponent<Launcher>().Ammo;
            SetState(SlimeState.Following);
        }else{
            GetComponent<MoveToTarget>().target = mainBody.transform;
            SetState(SlimeState.Flying);
        }
    }

    public void SetState(SlimeState newState){
        switch(newState){
            case SlimeState.Idle:
                GetComponent<Movable>().enabled = true;
                GetComponent<MoveToNearbyPosition>().enabled = true; // Unico Moveto com update

                GetComponent<Movable>().nextPosition = GetComponent<MoveToNearbyPosition>();

                col.isTrigger = false;
                rb.useGravity = true;
                initialPosition = transform.position;

                timer = lifeExpectancy;
                break;
            case SlimeState.Charging:
                GetComponent<Movable>().enabled = false;
                GetComponent<MoveToNearbyPosition>().enabled = false; // Unico Moveto com update

                col.isTrigger = false;
                rb.useGravity = true;
                break;
            case SlimeState.Flying:
                GetComponent<Movable>().enabled = false;
                GetComponent<MoveToNearbyPosition>().enabled = false; // Unico Moveto com update

                col.isTrigger = true;
                rb.useGravity = false;
                initialPosition = transform.position;
                break;
            case SlimeState.Attacking:
                GetComponent<Movable>().enabled = false;
                GetComponent<MoveToNearbyPosition>().enabled = false; // Unico Moveto com update

                col.isTrigger = true;
                rb.useGravity = false;

                attacksLeft = nAttacks;
                break;
            case SlimeState.Returning:
                GetComponent<Movable>().enabled = true;
                GetComponent<MoveToNearbyPosition>().enabled = false; // Unico Moveto com update

                GetComponent<Movable>().nextPosition = GetComponent<MoveToTarget>();
                GetComponent<MoveToTarget>().target = mainBody.transform;

                col.isTrigger = false;
                rb.useGravity = true;
                break;
            case SlimeState.Following:
                GetComponent<Movable>().enabled = true;
                GetComponent<MoveToNearbyPosition>().enabled = false; // Unico Moveto com update

                GetComponent<Movable>().nextPosition = GetComponent<MoveToTarget>();
                GetComponent<MoveToTarget>().target = Shepherd.instance.transform;

                col.isTrigger = false;
                rb.useGravity = true;
                break;
        }
        currentState = newState;
    }

    public void Grow(int count){
        if(isMainBody){
            GetComponent<Launcher>().Reload(count);
            if(size == 0){
                GetComponent<MeshRenderer>().enabled = true;
            }
        }
        size += count;
        rb.mass = size;
    }

    public void Shrink(int count){
        size = Mathf.Max(size - count, 0);
        rb.mass = size;
        if(size <= 0){
            if(isMainBody){
                GetComponent<MeshRenderer>().enabled = false;
            }else{
                // Inicia a animação de morte, que precisa chamar Die() no final
                Die(); // Placeholder
            }
        }
    }

    public void Launch(Vector3 direction, float speed, float maxDistance = float.PositiveInfinity){
        SetState(SlimeState.Flying);
        //rb.velocity = speed * direction;
        rb.velocity = new Vector3(speed * direction.x, 0f, speed * direction.z);
        timer = maxDistance; // É float e n é usado quando tá voando, vai ser usado pra n criar mais variavel
    }

    public void Attack(){
        if(transform.parent.GetComponent<IDamageable>().TakeDamage(size))
            return; // Se o alvo do ataque morreu nem verifica quantos ataques tem sobrando
        
        attacksLeft--;
        if(attacksLeft <= 0){
            Shrink(size);
        }
    }

    public bool TakeDamage(int dmg){
        Shrink(dmg);
        return (HP <= 0);
    }

    // Função pra ser chamada no animator
    public void Die(){
        if(size <= 0){
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other){
        // Colisao com os inimigos
        if(currentState == SlimeState.Flying && other.gameObject.layer == LayerMask.NameToLayer("Enemy")){
            RaycastHit hit;
            if(Physics.Raycast(transform.position, other.transform.position, out hit)){
                rb.velocity = Vector3.zero;
                transform.position = hit.point;
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal);
                other.GetComponent<Enemy>().Attach(this);
                SetState(SlimeState.Attacking);
            }
        }
    }

    void OnCollisionEnter(Collision other){
        // Colisao com a slime principal
        if(currentState == SlimeState.Returning && other.gameObject.layer == LayerMask.NameToLayer("Slime")){
            Slime otherSlime = other.gameObject.GetComponent<Slime>();
            if(otherSlime.isMainBody){
                otherSlime.Grow(size);
                size = 0; // Talvez mudar como isso acontece
                Die();
            }
        }
    }

    public void Update(){
        // Colocar a maquina de estados aqui
        switch(currentState){
            case SlimeState.Idle:
                if(timer <= 0){
                    Shrink(size);
                }

                timer -= Time.deltaTime;
                break;
            case SlimeState.Flying:
                if(Vector3.Distance(transform.position, initialPosition) > timer){
                    // SetState(SlimeState.Idle);
                    SetState(SlimeState.Returning); // Placeholder no prototipo
                }
                break;
            case SlimeState.Attacking:
                break;
            case SlimeState.Returning:
                break;
            case SlimeState.Following:
                break;
        }
    }
}
