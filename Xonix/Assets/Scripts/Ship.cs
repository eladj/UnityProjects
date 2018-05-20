using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ship : MonoBehaviour {

	public float speed = 1.0f;
	public GameManager gameManager;

	// private Rigidbody2D rb;
	
	// Use this for initialization
	void Start () {
		// rb = GetComponent<Rigidbody2D>();
	}
    void FixedUpdate () {
		// Get movement from input
        float moveHorizontal = Input.GetAxis ("Horizontal");
        float moveVertical = Input.GetAxis ("Vertical");
		
		// Limit movement to only one axis at a time (no diagonal movement)
		if (Mathf.Abs(moveHorizontal) > Mathf.Abs(moveVertical)){
			moveVertical = 0;
		} else {
			moveHorizontal = 0;
		}
        Vector3 movement = new Vector2 (moveHorizontal, moveVertical);

		// rb.AddForce(movement * speed);
		Vector3 newPos = this.transform.position + movement * speed * Time.deltaTime;
		newPos.x = Mathf.Clamp(newPos.x, 0f, gameManager.mapSizeX - 1);
		newPos.y = Mathf.Clamp(newPos.y, 0f, gameManager.mapSizeY - 1);
		this.transform.position = newPos;
		Vector2Int newPosInt = Vector2Int.FloorToInt(newPos);

		// Add current position to trail
		gameManager.UpdatePlayerPosition(newPosInt);
		Debug.Log("Current position: " + Vector3Int.RoundToInt(newPos).ToString());
    }

	// void OnCollisionEnter2D(Collision2D collision){
	// 	Debug.Log("Ship collided");
	// }

	// void OnTriggerEnter2D(Collider collider){
	// 	Debug.Log("Ship triggered with: " + collider.tag);
	// }
}
