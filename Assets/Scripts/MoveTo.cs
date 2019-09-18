using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movable))]
public abstract class MoveTo : MonoBehaviour {
	// Funcao que retorna a direcao na qual o objeto deve se mover
	// Apropriada para movimentacao baseada em velocidade de Rigidbody2D
	public virtual Vector2 Direction{ get{ return Vector2.zero; } }
	// Talvez seja possivel fazer scripts que definem uma posicao ao inves de direcao
	// Mas isso é mais apropriado para movimentacao com translacoes, entao nao usaremos

	public void Reset(){
		GetComponent<Movable>().nextPosition = this;
	}
}
