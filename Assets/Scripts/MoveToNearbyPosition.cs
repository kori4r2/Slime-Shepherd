﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToNearbyPosition : MoveTo
{
    [SerializeField] private float movementCooldown = 5f;
    [SerializeField] private float walkingRange = 10f;
    private float timer;
    private bool walking = false;
    private Vector2 targetPosition = Vector2.zero;

    void Awake(){
        timer = movementCooldown;
    }

    public override void Activate(){
        enabled = true;
        base.Activate();
        timer = movementCooldown;
        walking = false;
        targetPosition = Vector2.zero;
    }

    public void Deactivate(){
        timer = movementCooldown;
        walking = false;
        targetPosition = Vector2.zero;
        this.enabled = false;
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
            return Vector2.zero;
        }
    }

    public override NavMeshPath Path{
        get{
            if(timer <= 0f){
                Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
                if(!walking){
                    // calcula a proxima posição
                    targetPosition = position2D + (walkingRange * Random.insideUnitCircle);
                    walking = true;
                    NavMeshPath newPath = new NavMeshPath();
                    GetComponent<NavMeshAgent>().CalculatePath(new Vector3(targetPosition.x, transform.position.y, targetPosition.y), newPath);
                    return newPath;
                }else{
                    if(Vector2.Distance(position2D, targetPosition) <= (GetComponent<Movable>().MoveSpeed * Time.fixedDeltaTime)){
                        walking = false;
                        timer = movementCooldown;
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