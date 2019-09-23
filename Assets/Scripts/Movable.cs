using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movable : MonoBehaviour {
	[SerializeField] private float moveSpeed = 5f;
	public float MoveSpeed { get=>moveSpeed; }
	[SerializeField, Range(0f, 1f)] private float turnRate = 0.1f;
	public float minYSpeed = 1f;
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
	private Animator anim;

	public void Reset(){
		moveSpeed = 5f;
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
	}

	// Salva a referencia para o rigdigbody
	void Awake(){
		//canMove = true;
		rigid = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
	}

	// Atualiza a velocidade atual de acordo com a direcao definida pelo script de MoveTo
	void FixedUpdate(){
		if(canMove && nextPosition != null){
			Vector2 direction2D = nextPosition.Direction;
			rigid.velocity = new Vector3(direction2D.x * moveSpeed, rigid.velocity.y, direction2D.y * moveSpeed);
			float clampedYVelocity = (rigid.velocity.y > minYSpeed)? 0.0f : rigid.velocity.y;
			Vector3 clampedVelocity = new Vector3(rigid.velocity.x, clampedYVelocity, rigid.velocity.z);
			if(rigid.velocity.magnitude > 1f){
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rigid.velocity.normalized), turnRate);
			}else{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up)), turnRate);
			}
		}
	}
}
