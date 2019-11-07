using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeObstacle : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        Slime slime = collider.GetComponent<Slime>();
        if(slime != null)
        {
            collider.isTrigger = false;

            //Calculate new position to be
            Vector3 collidePosition = collider.transform.position;
            NavMeshHit navMeshHit;

            int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
            for(int i = 2; !NavMesh.SamplePosition(collidePosition, out navMeshHit, i, walkableMask); i++);

            Vector3 newPosition = navMeshHit.position;
            newPosition.y = Slime.mainBody.transform.position.y;

            slime.transform.position = newPosition;
            slime.SetState(Slime.SlimeState.Idle);
        }
    }
}
