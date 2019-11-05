using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveToTarget : MoveTo{
	[SerializeField] public float maxDistance = 0.5f;
	[SerializeField] public float minDistance = 0.5f;
	public Transform target;
	private Rigidbody rigid;
	private Vector2 lastPosition;

	// Salva a referencia para o rigidbody
	void Start(){
		rigid = GetComponent<Rigidbody>();
		if(target != null)
			lastPosition = new Vector2(target.position.x, target.position.z);
	}

	void OnValidate(){
		GetComponent<NavMeshAgent>().stoppingDistance = minDistance;
	}

	public void Activate(Transform newTarget){
		target = newTarget;
		Activate();
	}

	override public void Activate(){
		base.Activate();
		GetComponent<NavMeshAgent>().ResetPath();
		GetComponent<NavMeshAgent>().stoppingDistance = minDistance;
		lastPosition = new Vector2(float.MaxValue, float.MaxValue);
	}

	override public NavMeshPath Path{
		get{
			if(target == null){
				return null;
			}

			// Calcula a distancia ate o alvo a ser seguido
			Vector2 target2Dposition = new Vector2(target.position.x, target.position.z);
			Vector2 current2Dposition = new Vector2(rigid.position.x, rigid.position.z);
			float distance = Vector2.Distance(current2Dposition, target2Dposition);

			if(distance > maxDistance){
				if(lastPosition != target2Dposition && (Vector2.Distance(lastPosition, target2Dposition) > (minDistance/4f))){
					lastPosition = target2Dposition;
					NavMeshPath newPath = new NavMeshPath();
					GetComponent<NavMeshAgent>().CalculatePath(target.position, newPath);
					return newPath;
				}else{
					return GetComponent<NavMeshAgent>().path;
				}
			}

			lastPosition = target2Dposition;
			return null;
		}
	}

	override public Vector2 Direction{
		get{
			// Calcula a distancia ate o alvo a ser seguido
			Vector2 target2Dposition = new Vector2(target.position.x, target.position.z);
			Vector2 current2Dposition = new Vector2(rigid.position.x, rigid.position.z);
			float distance = Vector2.Distance(current2Dposition, target2Dposition);
			// Se estiver acima da distancia maxima permitida, retorna a direcao ao alvo normalizada
			if(distance > maxDistance)
				return (target2Dposition - current2Dposition).normalized;

			if(distance < minDistance)
				return Vector2.zero;

			Vector2 velocity2D = new Vector2(rigid.velocity.x, rigid.velocity.z);
			return velocity2D.normalized;
		}
	}
}
