using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : MoveTo{
	[SerializeField] private float maxDistance = 0.5f;
	[SerializeField] private float minDistance = 0.5f;
	public Transform target;
	private Rigidbody rigid;

	// Salva a referencia para o rigidbody
	void Start(){
		rigid = GetComponent<Rigidbody>();
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
