using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToNearbyPosition : MoveTo
{
    [SerializeField] private float movementCooldown = 5f;
    [SerializeField] private float walkingRange = 10f;
    private float timer = 0f;

    public override Vector2 Direction {
        get{
            return Vector2.zero;
        }
    }

    public void Update(){
        
    }
}