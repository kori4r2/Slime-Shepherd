using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SlimeSpawner : MonoBehaviour
{

    [SerializeField] private int nSlimes;
    [SerializeField] private float radius;
    [SerializeField] private GameObject slimePrefab;
    private List<Slime> slimesAlive = new List<Slime>();

#if UNITY_EDITOR
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    public static void DrawAttackRange(SlimeSpawner slimeSpawner, GizmoType gizmoType){
        Handles.DrawWireDisc(slimeSpawner.transform.position, Vector3.up, slimeSpawner.radius);
    }
#endif

    private void SpawnSlime(){
        Vector2 circlePos = Random.insideUnitCircle * radius;
        Vector3 position = new Vector3(transform.position.x + circlePos.x, transform.position.y, transform.position.z + circlePos.y);
        NavMeshHit hit;
        while(!NavMesh.SamplePosition(position, out hit, 2, NavMesh.AllAreas)){
            circlePos = Random.insideUnitCircle * radius;
            position = new Vector3(transform.position.x + circlePos.x, transform.position.y, transform.position.z + circlePos.y);
        }
        position = hit.position;

        GameObject newObj = Instantiate(slimePrefab, position, Quaternion.identity);
        Slime newSlime = newObj.GetComponent<Slime>();
        slimesAlive.Add(newSlime);
        newSlime.GetComponent<NavMeshAgent>().agentTypeID = NavMesh.GetSettingsByIndex(1).agentTypeID;
        newSlime.GetComponent<NavMeshAgent>().baseOffset = -0.15f;
        newSlime.SetState(Slime.SlimeState.Idle);
    }

    void Start()
    {
        for(int i = 0; i < nSlimes; i++){
            SpawnSlime();
        }
    }

    void Update()
    {
        if(slimesAlive.Count < nSlimes){
            SpawnSlime();
        }
    }
}