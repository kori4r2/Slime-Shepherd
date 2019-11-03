using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToInput : MoveTo {
		private Vector2 input;

		// O objeto deve estar parado inicialmente
		void Start(){
			input = Vector2.zero;
		}

		// A direcao a ser seguida e determinada apenas pelo input
		override public Vector2 Direction{
			get{
				return input.normalized;
			}
		}

		// Update is called once per frame
		void Update() {
			input.x = Input.GetAxisRaw("Horizontal");
			input.y = Input.GetAxisRaw("Vertical");
		}
}
