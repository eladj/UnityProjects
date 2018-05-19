using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	public float speed = 1.0f;

	private Rigidbody2D rb;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
    void FixedUpdate () {
        float moveHorizontal = Input.GetAxis ("Horizontal");
        float moveVertical = Input.GetAxis ("Vertical");

        Vector3 movement = new Vector2 (moveHorizontal, moveVertical);

		// rb.AddForce(movement * speed);
		Vector3 newPos = this.transform.position + movement * speed * Time.deltaTime;
		newPos.x = Mathf.Clamp(newPos.x, -6.4f, 6.4f);
		newPos.y = Mathf.Clamp(newPos.y, -4.75f, 3.7f);
		this.transform.position = newPos;
    }

	// void OnCollisionEnter2D(Collision2D collision){
	// 	Debug.Log("Ship collided");
	// }

	// void OnTriggerEnter2D(Collider collider){
	// 	Debug.Log("Ship triggered");
	// }
}
