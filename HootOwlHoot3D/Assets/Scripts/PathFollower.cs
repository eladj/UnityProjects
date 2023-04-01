using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public float MoveSpeed;
    
    private List<GameObject> nodes;
    private int currentNodeIndex;
    private Vector3 lastNodePosition;
    private Vector3 currentTargetPosition;
    private float timerSec;
    bool moving = false;

    // Use this for initialization
    void Start()
    {
        CheckNode();
    }

    void CheckNode()
    {
        timerSec = 0;
        lastNodePosition = transform.position;
        currentTargetPosition = nodes[currentNodeIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving){
            timerSec += Time.deltaTime * MoveSpeed;
            if (transform.position != currentTargetPosition)
            {
                transform.position = Vector3.Lerp(lastNodePosition, currentTargetPosition, timerSec);
            }
            else
            {
                // We reached the current node.
                if (currentNodeIndex < nodes.Count - 1)
                {
                    currentNodeIndex++;
                    CheckNode();
                }
                else {
                    // We reached the final node
                    moving = false;
                    GameManager.dragonReachedPosition.Invoke();
                }
            }
        }
    }

    // Move the object along the path of the game objects
    public void MoveInPath(List<GameObject> path){
        if (path.Count == 0) return;
        lastNodePosition = transform.position;
        currentTargetPosition = path[0].transform.position;
        currentNodeIndex = 0;
        nodes = path;
        timerSec = 0;
        moving = true;
    }
}
