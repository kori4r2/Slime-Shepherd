using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movable : MonoBehaviour {
	[SerializeField] private float moveSpeed = 5f;
	public MoveTo nextPosition;
	private bool canMove;
	public bool CanMove{
		get { return canMove; }
		set {
			if(value != canMove)
				rigid.velocity = Vector2.zero;
			canMove = value;
		}
	}
	private Rigidbody rigid;
	private Animator anim;

	public void Reset(){
		moveSpeed = 5f;
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
	}

	// Salva a referencia para o rigdigbody
	void Awake(){
		canMove = true;
		rigid = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
	}

	// Atualiza a velocidade atual de acordo com a direcao definida pelo script de MoveTo
	void FixedUpdate(){
		if(nextPosition != null && canMove){
			Vector2 direction2D = nextPosition.Direction;
			rigid.velocity = new Vector3(direction2D.x * moveSpeed, rigid.velocity.y, direction2D.y * moveSpeed);
			if(rigid.velocity != Vector3.zero)
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rigid.velocity.normalized), 0.1f);
		}
	}
}
