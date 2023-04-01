using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public float speed;
    private float leftBound;
    void Start(){
        leftBound = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 2.0f;
    }

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < leftBound){
            Destroy(gameObject);
        }
    }
}
