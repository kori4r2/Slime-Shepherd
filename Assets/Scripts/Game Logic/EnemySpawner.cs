using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public delegate void SpawnerDelegate(Enemy enemy);

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] private int nEnemies;
    [SerializeField] private float radius;
    [SerializeField] private GameObject enemyPrefab;
    private List<Enemy> enemiesAlive = new List<Enemy>();

#if UNITY_EDITOR
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    public static void DrawAttackRange(EnemySpawner enemySpawner, GizmoType gizmoType){
        Handles.DrawWireDisc(enemySpawner.transform.position, Vector3.up, enemySpawner.radius);
    }
#endif

    private void SpawnEnemy(){
        Vector2 circlePos = Random.insideUnitCircle * radius;
        Vector3 position = new Vector3(circlePos.x, transform.position.y, circlePos.y);
        NavMeshHit hit;
        while(!NavMesh.SamplePosition(position, out hit, 2, NavMesh.AllAreas)){
            circlePos = Random.insideUnitCircle * radius;
            position = new Vector3(circlePos.x, transform.position.y, circlePos.y);
        }
        position = hit.position;

        GameObject newObj = Instantiate(enemyPrefab, position, Quaternion.identity);
        Enemy newEnemy = newObj.GetComponent<Enemy>();
        enemiesAlive.Add(newEnemy);
        newEnemy.OnDeath += EnemyDied;
    }

    public void EnemyDied(Enemy enemy){
        if(enemiesAlive.Contains(enemy)){
            enemiesAlive.Remove(enemy);
        }
    }
    
    void Start()
    {
        for(int i = 0; i < nEnemies; i++){
            SpawnEnemy();
        }
    }

    void Update()
    {
        if(enemiesAlive.Count < nEnemies){
            SpawnEnemy();
        }
    }
}
