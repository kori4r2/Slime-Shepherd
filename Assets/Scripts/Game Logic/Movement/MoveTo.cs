using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Movable))]
public abstract class MoveTo : MonoBehaviour {
	// Propriedade que retorna a direcao na qual o objeto deve se mover
	// Apropriada para movimentacao baseada em velocidade de Rigidbody2D
	public virtual Vector2 Direction{ get{ return Vector2.zero; } }
	// Propriedade que retorna a o caminho que o objeto deve ir
	// Apropriada para movimentacao baseada em nav mesh
	public virtual NavMeshPath Path{ get{ return null; } }

	public virtual void Activate(){
		GetComponent<Movable>().nextPosition = this;
	}

	public void Reset(){
		GetComponent<Movable>().nextPosition = this;
	}
}
