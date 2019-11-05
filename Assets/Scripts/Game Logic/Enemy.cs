using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public delegate void VoidDelegate();
public delegate void IntDelegate(int value);

public class Enemy : MonoBehaviour, IDamageable
{
    protected enum EnemyState{
        Idle,
        Fleeing,
        Attacking,
        Null
    }

    [SerializeField] private GameObject slimePrefab;
    public Transform centerPosition;
    [SerializeField] protected int maxHP;
    [SerializeField] protected int mass = 1;
    [SerializeField] protected float damageCheckCooldown = 0.5f;
    [SerializeField] protected float lookAroundCooldown = 1.5f;
    [SerializeField] protected Transform attackPoint;
    private float attackRange;
    [SerializeField] protected float attackRadius = 0.5f;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float detectionRange = 10;
    private float moveSpeedSlow = 0f;
    protected bool blind = false;
    [SerializeField] private AnimationCurve slowCurve;
    protected GameObject SlimePrefab { get=>slimePrefab; }
    public int HP { get; protected set; }
    private EnemyState currentState;
    protected EnemyState CurrentState { get=>currentState; }
    private float timer;
    private float lastTargetSize = 1.0f;
    protected Transform Target{
        get{
            switch(currentState){
                case EnemyState.Attacking:
                return GetComponent<MoveToTarget>().target;
                case EnemyState.Fleeing:
                return GetComponent<MoveAwayFromTarget>().target;
                default:
                    return null;
            }
        }
        set{
            GetComponent<MoveToTarget>().target = value;
            GetComponent<MoveAwayFromTarget>().target = value;
            if(value.localScale.x != lastTargetSize){
                GetComponent<MoveToTarget>().minDistance -= lastTargetSize;
                GetComponent<MoveToTarget>().maxDistance -= lastTargetSize;

                lastTargetSize = value.localScale.x;

                GetComponent<MoveToTarget>().minDistance += lastTargetSize;
                GetComponent<MoveToTarget>().maxDistance += lastTargetSize;
            }
        }
    }
    private List<Slime> attackers = new List<Slime>();
    protected IntDelegate OnTakeDamage = null;
    private Animator animator;

    public void Awake(){
        attackRange = Vector3.Distance(transform.position, attackPoint.position);
        HP = maxHP;
        timer = 0;
        animator = GetComponent<Animator>();
    }

    public void Start(){
        SetState(EnemyState.Idle);
    }

    public void OnValidate(){
        GetComponent<MoveToTarget>().minDistance = Vector3.Distance(transform.position, attackPoint.position);
        GetComponent<MoveToTarget>().maxDistance = Vector3.Distance(transform.position, attackPoint.position) + attackRadius/2;
        GetComponent<MoveAwayFromTarget>().minDistance = detectionRange;
    }

#if UNITY_EDITOR
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    public static void DrawAttackRange(Enemy enemy, GizmoType gizmoType){
        if(enemy.attackPoint != null){
            Gizmos.DrawWireSphere(enemy.attackPoint.position, enemy.attackRadius);
        }
        Handles.DrawWireDisc(enemy.transform.position, enemy.transform.up, enemy.detectionRange);
    }
#endif

    public void Stop(){
        blind = true;
        damage = 0;
        SetState(EnemyState.Idle);
    }

    protected void SetState(EnemyState newState){
        switch(newState){
            case EnemyState.Attacking:
                GetComponent<NavMeshAgent>().enabled = true;
                GetComponent<NavMeshAgent>().ResetPath();
                GetComponent<Movable>().CanMove = true;
                GetComponent<MoveToNearbyPosition>().Deactivate();

                GetComponent<MoveToTarget>().Activate();

                break;
            case EnemyState.Fleeing:
                GetComponent<NavMeshAgent>().enabled = true;
                GetComponent<NavMeshAgent>().ResetPath();
                GetComponent<Movable>().CanMove = true;
                GetComponent<MoveToNearbyPosition>().Deactivate();

                GetComponent<MoveAwayFromTarget>().Activate();

                animator.SetBool("Attacking", false);
                break;
            case EnemyState.Idle:
                GetComponent<NavMeshAgent>().enabled = true;
                GetComponent<NavMeshAgent>().ResetPath();
                GetComponent<Movable>().CanMove = true;

                GetComponent<MoveToNearbyPosition>().Activate();

                animator.SetBool("Attacking", false);
                break;
            case EnemyState.Null:
                GetComponent<NavMeshAgent>().enabled = false;
                GetComponent<NavMeshAgent>().ResetPath();
                GetComponent<Movable>().CanMove = false;
                GetComponent<MoveToNearbyPosition>().Deactivate();

                animator.SetBool("Attacking", false);
                break;
        }
        currentState = newState;
        // switch(currentState){
        //     case EnemyState.Attacking:
        //         Debug.Log("attacking " + Target.name);
        //         break;
        //     case EnemyState.Fleeing:
        //         Debug.Log("running from " + Target.name);
        //         break;
        //     case EnemyState.Idle:
        //         Debug.Log("just chillin\'");
        //         break;
        // }
    }

    // Essa função checa se tem alguma slime no alcance de detecção e reage de maneira apropriada
    protected virtual void LookAround(){
        if(blind)
            return;
            
        if(Physics.CheckCapsule(transform.position, new Vector3(transform.position.x, 0, transform.position.z), detectionRange, LayerMask.GetMask("Slime"))){
            Target = Slime.mainBody.transform;
            // SetState(EnemyState.Attacking);
            SetState(EnemyState.Fleeing);
        }
    }

    protected int CompareDistance(Collider a, Collider b){
        float distanceA = Vector3.Distance(transform.position, a.transform.position);
        float distanceB = Vector3.Distance(transform.position, b.transform.position);
        return (int)(distanceA - distanceB);
    }

    public virtual void Attack(){
        // Virtual para permitir implementação de attaques ranged, que n teremos nessa build
        foreach(Collider col in Physics.OverlapSphere(attackPoint.position, attackRadius, LayerMask.GetMask("Slime"))){
            Slime slime = col.GetComponent<Slime>();
            slime.TakeDamage(damage);
        }
    }

    public bool TakeDamage(int dmg){
        HP = Mathf.Max(HP - dmg, 0);
        if(HP > 0){
            if(OnTakeDamage != null)
                OnTakeDamage(dmg);
        }
        return (HP <= 0);
    }

    public void Attach(Slime slime){
        attackers.Add(slime);
        slime.transform.SetParent(transform, true);
    }

    public void Update(){
        // Checa o dano causado pelas slimes grudadas
        if(timer > damageCheckCooldown && HP > 0){
            timer -= damageCheckCooldown;

            foreach(Slime slime in attackers.ToArray()){
                if(slime == null){
                    attackers.Remove(slime);
                }
            }

            for(int i = attackers.Count - 1; i >= 0 && HP > 0; i--){
                if(attackers[i].CurrentState == Slime.SlimeState.Attacking){
                    attackers[i].Attack();
                }
            }

            if(HP <= 0){
                foreach(Slime slime in attackers){
                    slime.transform.rotation = Quaternion.FromToRotation(slime.transform.up, Vector3.up);
                    slime.transform.SetParent(null, true);
                    slime.SetState(Slime.SlimeState.Idle);
                }
                animator.SetTrigger("Die");
            }

            float previousSlow = moveSpeedSlow;
            float weight = 0f;
            foreach(Slime slime in attackers){
                weight += slime.HP;
            }
            moveSpeedSlow = slowCurve.Evaluate(weight);
            GetComponent<Movable>().MoveSpeed /= (1 - previousSlow);
            GetComponent<Movable>().MoveSpeed *= (1 - moveSpeedSlow);
        }

        switch(currentState){
            case EnemyState.Attacking:
                if(Target == null
                || Vector3.Distance(transform.position, Target.position) > detectionRange * 1.2
                || Target.GetComponent<IDamageable>().HP <= 0){
                    SetState(EnemyState.Idle);
                }else if(Vector3.Distance(transform.position, Target.position) <= (attackRange + attackRadius/2)){
                    animator.SetBool("Attacking", true);
                }else{
                    animator.SetBool("Attacking", false);
                }
                break;
            case EnemyState.Fleeing:
                if(Target == null
                || Vector3.Distance(transform.position, Target.position) > GetComponent<MoveAwayFromTarget>().maxDistance
                || Target.GetComponent<IDamageable>().HP <= 0){
                    SetState(EnemyState.Idle);
                }
                break;
            case EnemyState.Idle:
                if(!GetComponent<MoveToNearbyPosition>().Walking){
                    LookAround();
                }
                break;
            case EnemyState.Null:
                break;
        }

        timer += Time.deltaTime;
    }

    public virtual void Die(){ // Cada inimigo pode dar override nisso aqui, spawna o que spawna na morte e chama base.Die() Talvez um callback seja melhor?
        if(HP <= 0){
            if(mass > 0){
                GetComponent<Collider>().enabled = false;
                Slime newSlime = Instantiate(SlimePrefab, transform).GetComponent<Slime>();
                newSlime.transform.SetParent(null, true);
                newSlime.Grow(mass);
                newSlime.SetState(Slime.SlimeState.Idle);
            }
            Destroy(gameObject);
        }
    }
}
