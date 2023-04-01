using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private Rigidbody2D rb;
    public float jumpSize;

    private void OnEnable(){
        transform.position = new Vector3(-6, 1, 0);
        rb.velocity = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        // gameManager = gameManagerGameObject.GetComponent<GameManager>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            // print("space key was pressed");
            rb.AddForce(Vector3.up * jumpSize, ForceMode2D.Impulse);
        }

        if (Input.touchCount > 0){
            Touch touch =Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began){
                rb.AddForce(Vector3.up * jumpSize, ForceMode2D.Impulse);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.gameObject.tag == "Obstacle"){
            print("Collided with obstacle");
            FindObjectOfType<GameManager>().endGame();
        } else if (collider.gameObject.tag == "Gate"){
            FindObjectOfType<GameManager>().passedGate();
        }
    }
}
