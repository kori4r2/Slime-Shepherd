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
			moveSpeed = value;
			GetComponent<NavMeshAgent>().speed = moveSpeed;
		}
	}
	[SerializeField, Range(0.5f, 1f)] private float turnRate = 0.5f;
	public float TurnRate { get=>turnRate; }
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
				if(direction2D != Vector2.zero){
					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rigid.velocity.normalized, Vector3.up), turnRate);
				}
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

		if(anim != null){
			List<AnimatorControllerParameter> parameters = new List<AnimatorControllerParameter>(anim.parameters);
			if(parameters.Find(param => (param.name == "Speed" && param.type == AnimatorControllerParameterType.Float)) != null){
				if(nextPosition.GetType() == typeof(MoveToInput)){
					anim.SetFloat("Speed", rigid.velocity.sqrMagnitude);
				}else{
					anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
				}
			}
		}
	}

	public static float Distance2D(Vector3 a, Vector3 b){
		Vector2 a2D = new Vector2(a.x, a.z);
		Vector2 b2D = new Vector2(b.x, b.z);

		return Vector2.Distance(a2D, b2D);
	}
}
