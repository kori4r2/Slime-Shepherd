using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggresive : Enemy
{
    protected override void LookAround(){
        Collider[] colliders = Physics.OverlapCapsule(transform.position, new Vector3(transform.position.x, 0, transform.position.z), detectionRange, LayerMask.GetMask("Slime"));
        System.Array.Sort(colliders, CompareDistance);

        if(colliders.Length > 0){
            foreach(Collider col in colliders){
                Slime slime = col.GetComponent<Slime>();
                if(slime.CurrentState != Slime.SlimeState.Attacking
                && slime.CurrentState != Slime.SlimeState.Flying
                && slime.CurrentState != Slime.SlimeState.Null){
                    Target = col.transform;
                    SetState(EnemyState.Attacking);
                }
            }
        }
    }
}
