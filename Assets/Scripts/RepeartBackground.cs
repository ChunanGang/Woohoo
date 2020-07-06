
// This script is used to repeat the background/ ground,
// in order to create the effect that the player keeps running

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeartBackground : MonoBehaviour
{
    private Vector3 startPos; // the object's start position 
    private float repeatWidth; // repeat after mving this much distance

    void Start()
    {
        startPos = transform.position;
        // calculate the repeatwidth by using half of the object's width
        repeatWidth = GetComponent<BoxCollider>().size.x * transform.localScale.x / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if( transform.position.x  < startPos.x - repeatWidth)
        {
            transform.position = startPos;
        }   
    }
}
