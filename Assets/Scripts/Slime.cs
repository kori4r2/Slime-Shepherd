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
        Returning,
        Null
    }

    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private bool isMainBody = false;
    [SerializeField] private float lifeExpectancy = 10f;
    [SerializeField] private int nAttacks = 5;

    public static Slime mainBody;
    public SlimeState CurrentState { get; private set; }
    private Rigidbody rb;
    private Collider col;
    private Animator anim;
    private Vector3 initialPosition;
    private float timer;
    private int attacksLeft;
    private bool isGrounded;
    private bool CanJump { get { return (jumpTimer <= 0f); } }
    [SerializeField] private float jumpCooldown = 0.4f;
    [SerializeField] private float jumpHeight = 0.35f;
    private float jumpTimer = 0;
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
        anim = GetComponent<Animator>();
        initialPosition = transform.position;
        if(isMainBody){
            mainBody = this;
            size = GetComponent<Launcher>().Ammo;
        }else{
            GetComponent<MoveToTarget>().target = mainBody.transform;
            SetState(SlimeState.Null);
        }
    }

    public void Start(){
        if(isMainBody){
            size = GetComponent<Launcher>().Ammo;
            GetComponent<MoveToTarget>().target = Shepherd.instance.transform;
            SetState(SlimeState.Following);
        }
    }

    public void SetState(SlimeState newState){
        switch(newState){
            case SlimeState.Idle:
                GetComponent<Movable>().CanMove = true;
                GetComponent<MoveToNearbyPosition>().enabled = true; // Unico Moveto com update

                GetComponent<Movable>().nextPosition = GetComponent<MoveToNearbyPosition>();

                col.isTrigger = false;
                rb.useGravity = true;
                rb.isKinematic = false;
                initialPosition = transform.position;

                timer = lifeExpectancy;

                // anim.SetBool("isGrounded", true);
                anim.SetBool("TopEnemy", false);
                anim.SetBool("ChargingAttack", false);
                break;
            case SlimeState.Charging:
                GetComponent<Movable>().CanMove = false;
                rb.velocity = Vector3.zero;
                GetComponent<MoveToNearbyPosition>().Deactivate(); // Unico Moveto com update

                col.isTrigger = false;
                rb.useGravity = true;
                rb.isKinematic = false;

                // anim.SetBool("isGrounded", true);
                anim.SetBool("TopEnemy", false);
                anim.SetBool("ChargingAttack", true);
                break;
            case SlimeState.Flying:
                GetComponent<Movable>().CanMove = false;
                GetComponent<MoveToNearbyPosition>().Deactivate(); // Unico Moveto com update

                col.isTrigger = true;
                rb.useGravity = false;
                rb.isKinematic = false;
                initialPosition = transform.position;

                // anim.SetBool("isGrounded", false);
                anim.SetBool("TopEnemy", false);
                anim.SetBool("ChargingAttack", false);
                break;
            case SlimeState.Attacking:
                GetComponent<Movable>().CanMove = false;
                GetComponent<MoveToNearbyPosition>().Deactivate(); // Unico Moveto com update

                col.isTrigger = true;
                rb.useGravity = false;
                rb.isKinematic = true;

                attacksLeft = nAttacks;

                // anim.SetBool("isGrounded", false);
                anim.SetBool("TopEnemy", true);
                anim.SetBool("ChargingAttack", false);
                break;
            case SlimeState.Returning:
                GetComponent<Movable>().CanMove = true;
                GetComponent<MoveToNearbyPosition>().Deactivate(); // Unico Moveto com update

                GetComponent<Movable>().nextPosition = GetComponent<MoveToTarget>();
                GetComponent<MoveToTarget>().target = mainBody.transform;

                col.isTrigger = false;
                rb.useGravity = true;
                rb.isKinematic = false;

                // anim.SetBool("isGrounded", true);
                anim.SetBool("TopEnemy", false);
                anim.SetBool("ChargingAttack", false);
                break;
            case SlimeState.Following:
                GetComponent<Movable>().CanMove = true;
                GetComponent<MoveToNearbyPosition>().Deactivate(); // Unico Moveto com update

                GetComponent<Movable>().nextPosition = GetComponent<MoveToTarget>();
                GetComponent<MoveToTarget>().target = Shepherd.instance.transform;

                col.isTrigger = false;
                rb.useGravity = true;
                rb.isKinematic = false;

                // anim.SetBool("isGrounded", true);
                anim.SetBool("TopEnemy", false);
                anim.SetBool("ChargingAttack", false);
                break;
            case SlimeState.Null:
                GetComponent<Movable>().CanMove = false;
                GetComponent<MoveToNearbyPosition>().Deactivate(); // Unico Moveto com update

                GetComponent<Movable>().nextPosition = GetComponent<MoveToTarget>();
                GetComponent<MoveToTarget>().target = mainBody.transform;

                col.isTrigger = true;
                rb.useGravity = false;
                rb.isKinematic = false;

                // anim.SetBool("isGrounded", true);
                anim.SetBool("TopEnemy", false);
                anim.SetBool("ChargingAttack", false);
                break;
        }
        CurrentState = newState;
    }

    public void Grow(int count){
        if(isMainBody){
            GetComponent<Launcher>().Reload(count);
            if(size == 0){
                GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            }
        }
        size += count;
        rb.mass = size;
    }

    public void Shrink(int count){
        if(isMainBody)
            GetComponent<Launcher>().Reload(-count);
        size = Mathf.Max(size - count, 0);
        rb.mass = size;

        if(size <= 0){
            if(isMainBody){
                GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            }else{
                GetComponent<Movable>().enabled = false;
                col.enabled = false;
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
                // Inicia a animação de morte, que precisa chamar Die() no final
                anim.SetTrigger("Die");
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
        anim.SetTrigger("Hit");
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
        if(CurrentState == SlimeState.Flying && other.gameObject.layer == LayerMask.NameToLayer("Enemy")){
            RaycastHit hit;
            if(Physics.Raycast(transform.position, other.transform.position - transform.position, out hit, 2 * Vector3.Distance(transform.position, other.transform.position), LayerMask.GetMask("Enemy"))){
                rb.velocity = Vector3.zero;
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal);
                transform.position = hit.point;
                other.GetComponent<Enemy>().Attach(this);
                SetState(SlimeState.Attacking);
            }
        }
    }

    void OnCollisionEnter(Collision other){
        // Colisao com a slime principal
        if(CurrentState == SlimeState.Returning && other.gameObject.layer == LayerMask.NameToLayer("Slime")){
            Slime otherSlime = other.gameObject.GetComponent<Slime>();
            if(otherSlime.isMainBody){
                otherSlime.Grow(size);
                // Talvez mudar como isso acontece (mas provavelmente n)
                size = 0;
                Die();
            }
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Ground")){
            anim.SetBool("isGrounded", true);
            isGrounded = true;
            jumpTimer = jumpCooldown;
        }
    }

    void OnCollisionExit(Collision other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground")){
            anim.SetBool("isGrounded", false);
            isGrounded = false;
        }
    }

    private void Jump(){
        // Seta a velocidade necessária para atingir a altura de pulo desejada
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(2 * jumpHeight * Physics.gravity.magnitude), rb.velocity.z);
    }

    public void Update(){
        switch(CurrentState){
            case SlimeState.Idle:
                // Checa se acabou o tempo de vida
                if(timer <= 0){
                    Shrink(size);
                }

                // Faz o pulo se necessário
                if(isGrounded && CanJump && ((Mathf.Abs(rb.velocity.x) > float.Epsilon) || (Mathf.Abs(rb.velocity.z) > float.Epsilon)) ){
                    Jump();
                }

                jumpTimer -= Time.deltaTime;
                timer -= Time.deltaTime;
                break;
            case SlimeState.Charging:
                // Neutraliza movimentações causada pela fisica, como colisões
                rb.velocity = Vector3.zero;
                break;
            case SlimeState.Flying:
                // Se tiver voado a distancia maxima (armazenada em timer) muda de estado
                if(Vector3.Distance(transform.position, initialPosition) > timer){
                    SetState(SlimeState.Idle);
                }
                break;
            case SlimeState.Attacking:
                break;
            case SlimeState.Returning:
                // Faz o pulo se necessário
                if(isGrounded && CanJump && ((Mathf.Abs(rb.velocity.x) > float.Epsilon) || (Mathf.Abs(rb.velocity.z) > float.Epsilon)) ){
                    Jump();
                }

                jumpTimer -= Time.deltaTime;
                break;
            case SlimeState.Following:
                // Faz o pulo se necessário
                if(isGrounded && CanJump && ((Mathf.Abs(rb.velocity.x) > float.Epsilon) || (Mathf.Abs(rb.velocity.z) > float.Epsilon)) ){
                    Jump();
                }

                jumpTimer -= Time.deltaTime;
                break;
        }

        anim.SetFloat("VerticalSpeed", rb.velocity.y);
    }
}
