using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToNearbyPosition : MoveTo
{
    [SerializeField] private float movementCooldown = 5f;
    [SerializeField] private float walkingRange = 10f;
    [SerializeField, Range(0f, 1f)] private float minMoveRangeRelative = 0.3f;
    private float timer;
    private bool walking = false;
    public bool Walking { get=>walking; }
    private Vector2 targetPosition = Vector2.zero;

    void Awake(){
        timer = movementCooldown;
    }

    public override void Activate(){
        timer = movementCooldown;
        walking = false;
        targetPosition = Vector2.zero;
		GetComponent<NavMeshAgent>().ResetPath();
        enabled = true;
        base.Activate();
    }

    public void Deactivate(){
        timer = movementCooldown;
        walking = false;
        targetPosition = Vector2.zero;
        enabled = false;
    }

    public override Vector2 Direction {
        get{
            if(timer <= 0f){
                Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
                if(!walking){
                    // calcula a proxima posição
                    targetPosition = position2D + (walkingRange * Random.insideUnitCircle);
                    walking = true;
                    return (targetPosition - position2D).normalized;
                }else{
                    if(Vector2.Distance(position2D, targetPosition) <= (GetComponent<Movable>().MoveSpeed * Time.fixedDeltaTime)){
                        walking = false;
                        timer = movementCooldown;
                    }else{
                        return (targetPosition - position2D).normalized;
                    }
                }
            }
            walking = false;
            return Vector2.zero;
        }
    }

    protected Vector2 GetNearbyPosition(Vector2 position2D){
        Vector2 random = Random.insideUnitCircle;
        if(random.x < 0 && random.x > -minMoveRangeRelative){
            random.x = -minMoveRangeRelative;
        }
        if(random.x >= 0 && random.x < minMoveRangeRelative){
            random.x = minMoveRangeRelative;
        }
        if(random.y < 0 && random.y > -minMoveRangeRelative){
            random.y = -minMoveRangeRelative;
        }
        if(random.y >= 0 && random.y < minMoveRangeRelative){
            random.y = minMoveRangeRelative;
        }

        Vector2 newPosition = position2D + (walkingRange * random);
        return newPosition;
    }

    public override NavMeshPath Path{
        get{
            if(timer <= 0f){
                NavMeshAgent agent = GetComponent<NavMeshAgent>();
                Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
                if(!walking){
                    // calcula a proxima posição
                    targetPosition = GetNearbyPosition(position2D);

                    Vector3 position = new Vector3(targetPosition.x, transform.position.y, targetPosition.y);
                    NavMeshHit hit;
					int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
                    for(int i = 0; !NavMesh.SamplePosition(position, out hit, i, walkableMask); i++);
                    targetPosition = new Vector2(position.x, position.z);

                    walking = true;
                    NavMeshPath newPath = new NavMeshPath();
                    agent.CalculatePath(hit.position, newPath);
                    return newPath;
                }else{
                    Debug.Log("walking deu true");
                    if(Vector3.Distance(transform.position, agent.pathEndPosition) <= 2f
                    || Vector3.Distance(agent.nextPosition, agent.pathEndPosition) <= 2f){
                           
                        walking = false;
                        timer = movementCooldown;
                        return null;
                    }else{
                        return GetComponent<NavMeshAgent>().path;
                    }
                }
            }
            return null;
        }
    }

    public void Update(){
        if(timer > 0f){
            timer -= Time.deltaTime;
        }
    }
}