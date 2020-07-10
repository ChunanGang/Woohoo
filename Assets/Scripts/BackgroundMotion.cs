// This scripts controls the motion of the background / ground.
// It also handle the circulation of the background / ground.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMotion : MonoBehaviour
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
        // move left
        float speed = GameObject.Find("GameManager").GetComponent<GameManager>().BackgroundSpeed;
        transform.Translate(Vector3.left * Time.deltaTime * speed);

        // circulation
        if (transform.position.x < startPos.x - repeatWidth)
        {
            transform.position = startPos;
        }

    }
}
