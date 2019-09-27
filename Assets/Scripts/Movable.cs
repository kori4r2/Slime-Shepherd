using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NavMeshAgent))]
public class Movable : MonoBehaviour {
	[SerializeField] private float moveSpeed = 5f;
	public float MoveSpeed {
		get=>moveSpeed;
		set{
			GetComponent<NavMeshAgent>().speed = moveSpeed;
			moveSpeed = value;
		}
	}
	[SerializeField, Range(0.5f, 1f)] private float turnRate = 0.5f;
	public MoveTo nextPosition;
	private bool canMove = true;
	public bool CanMove{
		get { return canMove; }
		set {
			if(value != canMove && rigid != null)
				rigid.velocity = Vector2.zero;
			canMove = value;
		}
	}
	private Rigidbody rigid;
	private NavMeshAgent agent;
	private Animator anim;

	public void Reset(){
		moveSpeed = 5f;
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
	}

	void OnValidate(){
		GetComponent<NavMeshAgent>().speed = moveSpeed;
		GetComponent<NavMeshAgent>().angularSpeed = Mathf.Rad2Deg * (turnRate /0.03f);
	}

	// Salva a referencia para o rigdigbody
	void Awake(){
		//canMove = true;
		agent = GetComponent<NavMeshAgent>();
		rigid = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
	}

	// Atualiza a velocidade atual de acordo com a direcao definida pelo script de MoveTo
	void FixedUpdate(){
		if(CanMove && nextPosition != null){
			if(nextPosition.GetType() == typeof(MoveToInput)){
				rigid.isKinematic = false;
				Vector2 direction2D = nextPosition.Direction;
				rigid.velocity = new Vector3(direction2D.x * moveSpeed, rigid.velocity.y, direction2D.y * moveSpeed);
			}else if(agent.enabled){
				rigid.isKinematic = true;
				NavMeshPath newPath = nextPosition.Path;
				if(newPath != null){
					if(newPath != agent.path){
						agent.SetPath(newPath);
					}
				}else{
					agent.ResetPath();
				}
			}
		}else if(agent.enabled){
			agent.velocity = Vector3.zero;
			rigid.velocity = Vector3.zero;
		}
	}
}
