using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHP;
    [SerializeField] private float damageCheckPeriod;
    public int HP{ get; protected set; }
    private float timer;
    private List<Slime> attackers = new List<Slime>();

    public void Awake(){
        HP = maxHP;
        timer = 0;
    }

    public bool TakeDamage(int dmg){
        HP = Mathf.Max(HP - dmg, 0);
        return (HP <= 0);
    }

    public void Attach(Slime slime){
        attackers.Add(slime);
        slime.transform.SetParent(transform, true);
    }

    public void Update(){
        if(timer > damageCheckPeriod){
            timer -= damageCheckPeriod;

            for(int i = attackers.Count - 1; i >= 0 && HP > 0; i--){
                attackers[i].Attack();
            }

            if(HP <= 0){
                foreach(Slime slime in attackers){
                    slime.transform.rotation = Quaternion.FromToRotation(slime.transform.up, Vector3.up);
                    slime.transform.SetParent(null, true);
                    // slime.SetState(Slime.SlimeState.Idle);
                    slime.SetState(Slime.SlimeState.Returning);
                }
                Die();
            }
        }

        timer += Time.deltaTime;
    }

    public virtual void Die(){ // Cada inimigo da override nisso aqui, spawna o que spawna na morte e chama base.Die()
        if(HP <= 0){
            Destroy(gameObject);
        }
    }
}
