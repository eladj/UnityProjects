using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public Pipe pipePrefab;

    public float spawnStartTime;
    public float spawnRate;
    private float spawnStartPositionX;

    public float pipeMinHeight;
    public float pipeMaxHeight;

    void Start(){
        spawnStartPositionX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 1.0f;
    }

    private void OnEnable(){
        InvokeRepeating("SpawnPipe", spawnStartTime, spawnRate);
    }

    private void OnDisable(){
        CancelInvoke(nameof(SpawnPipe));
    }

    private void SpawnPipe(){
        Vector3 instantiatePosition = new Vector3(spawnStartPositionX, Random.Range(pipeMinHeight, pipeMaxHeight), 0);
        Instantiate(pipePrefab, instantiatePosition, Quaternion.identity);
    }
}
