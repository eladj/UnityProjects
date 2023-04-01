using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float leftBound = -800;
    [SerializeField] private float rightBound = 600;

    void Update()
    {
        transform.position += Vector3.right * Time.deltaTime * speed;
        if (transform.position.x > rightBound) {
            transform.position = new Vector3(leftBound, transform.position.y, transform.position.z);
        }
    }
}
