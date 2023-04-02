using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    [SerializeField] private int islandIndex;
    [SerializeField] private IslandType islandType;
    // private GameObject platform;
    private Vector3 startPosition;
    private float floatAmplitude;
    private float floatFrequency;
    private float floatOffset;

    void Awake()
    {
        startPosition = transform.position;
        // platform = transform.Find("Platform").gameObject;
        floatOffset = Random.Range(0f, Mathf.PI);
        floatAmplitude = Random.Range(0.05f, 0.1f);
        floatFrequency = Random.Range(0.2f, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPosition + transform.up * floatAmplitude * Mathf.Sin(Time.time * floatFrequency + floatOffset);
    }

    public int IslandIndex(){
        return islandIndex;
    }

    public Vector3 Position(){
        return startPosition;
    }

    public string IslandColor(){
        return islandType.ToString();
    }

    // void SetPlatformColor(){
    //     Material mat = platform.GetComponent<MeshRenderer>().material;
    //     Color rgba = new Color();
    //     switch (islandType){
    //         case (IslandType.Red):
    //             rgba = new Color(0.75f, 0.0f, 0.0f, 1.0f);
    //             break;
    //         case (IslandType.Blue):
    //             rgba = new Color(0.2f, 0f, 0.75f, 1.0f);
    //             break;
    //         case (IslandType.Yellow):
    //             rgba = new Color(0.75f, 0.75f, 0.0f, 1.0f);
    //             break;
    //         case (IslandType.Green):
    //             rgba = new Color(0f, 0.75f, 0.0f, 1.0f);
    //             break;
    //         case (IslandType.Purple):
    //             rgba = new Color(0.3f, 0f, 0.75f, 1.0f);
    //             break;
    //         case (IslandType.Orange):
    //             rgba = new Color(0.75f, 0.2f, 0.04f, 1.0f);
    //             break;                                                                                
    //     }
    //     mat.color = rgba;
    // }
}
