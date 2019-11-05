using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveAwayFromTarget : MoveTo{
	[SerializeField] public float maxDistance = 0.5f;
	[SerializeField] public float minDistance = 0.5f;
	private bool running = false;
	public Transform target;
	private Rigidbody rigid;
	private Vector2 lastPosition;

	// Salva a referencia para o rigidbody
	void Start(){
		rigid = GetComponent<Rigidbody>();
		if(target != null)
			lastPosition = new Vector2(target.position.x, target.position.z);
	}

	public void Activate(Transform newTarget){
		GetComponent<NavMeshAgent>().ResetPath();
		target = newTarget;
		Activate();
	}

	override public void Activate(){
		running =false;
		base.Activate();
		GetComponent<NavMeshAgent>().ResetPath();
		GetComponent<NavMeshAgent>().stoppingDistance = 0;
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

			if((distance < minDistance && distance < maxDistance) || running){
				if(lastPosition != target2Dposition && (Vector2.Distance(lastPosition, target2Dposition) > 0.5f)){
					lastPosition = target2Dposition;
					NavMeshPath newPath = new NavMeshPath();
					Vector2 escapePosition = current2Dposition + ((current2Dposition - target2Dposition).normalized * ((2*maxDistance) - distance));
					GetComponent<NavMeshAgent>().CalculatePath(new Vector3(escapePosition.x, transform.position.y, escapePosition.y), newPath);
					running = true;
					return newPath;
				}else if(distance < maxDistance){
					return GetComponent<NavMeshAgent>().path;
				}else if(distance >= maxDistance){
					running = false;
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
				return (current2Dposition - target2Dposition).normalized;

			if(distance < minDistance)
				return Vector2.zero;

			Vector2 velocity2D = new Vector2(rigid.velocity.x, rigid.velocity.z);
			return velocity2D.normalized;
		}
	}
}