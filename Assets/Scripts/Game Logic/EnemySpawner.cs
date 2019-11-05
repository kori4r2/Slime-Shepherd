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
    [SerializeField] private float spawnDistance = 10;
    [SerializeField] private float minPlayerDistance = 5;
    private List<Enemy> enemiesAlive = new List<Enemy>();
    public bool active;

#if UNITY_EDITOR
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    public static void DrawAttackRange(EnemySpawner enemySpawner, GizmoType gizmoType){
        Handles.DrawWireDisc(enemySpawner.transform.position, Vector3.up, enemySpawner.radius);
    }
#endif

    private void SpawnEnemy(){
        Vector2 circlePos = Random.insideUnitCircle * radius;
        Vector3 position = transform.position + new Vector3(circlePos.x, 0, circlePos.y);
        NavMeshHit hit;

        int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
        for(int i = 2; !NavMesh.SamplePosition(position, out hit, i, walkableMask); i++);

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

    public void Activate(){
        active = true;
        foreach(Enemy enemy in enemiesAlive.ToArray()){
            if(enemy != null)
                enemy.gameObject.SetActive(true);
            else
                enemiesAlive.Remove(enemy);
        }
    }

    public void Deactivate(){
    }

    void Start()
    {
        active = Vector3.Distance(transform.position, Shepherd.instance.transform.position) < (spawnDistance + radius);
        if(active){
            for(int i = 0; i < nEnemies; i++){
                SpawnEnemy();
            }
        }
    }

    void Update()
    {
        if(active && Vector3.Distance(transform.position, Shepherd.instance.transform.position) >= (minPlayerDistance + radius) && enemiesAlive.Count < nEnemies){
            SpawnEnemy();
        }
    }
}
