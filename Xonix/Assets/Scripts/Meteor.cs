using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {

	public float speed = 30f;
	public float init_torque = 0.02f;
	private Rigidbody2D rb;
	void Start () {
		rb = GetComponent<Rigidbody2D>();

		// Generate random angle for initial force vector
		float ang = Random.value * 2 * Mathf.PI;
		Vector2 force_vec = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
		rb.AddForce(force_vec*speed);
		rb.AddTorque(init_torque);
		Debug.Log("Set position: " + this.transform.position.ToString());
	}
	
	// void OnCollisionEnter2D(Collision2D collision){
	// 	Debug.Log("Collision detected");
	// }

}
