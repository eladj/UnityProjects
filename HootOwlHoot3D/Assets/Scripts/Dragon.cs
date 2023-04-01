using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    public int curIslandIndex;
    public int index;
    
    public void SetInteractable(bool value)
    {
        gameObject.GetComponent<SphereCollider>().enabled = value;
    }

    public void MoveInPath(List<GameObject> path){
        gameObject.GetComponent<PathFollower>().MoveInPath(path);
    }

//    void OnMouseDown()
//    {
//        // Code here is called when the GameObject is clicked on.
//         GameManager.dragonSelected.Invoke();
//    }

}
