// Script that moves the "horizontal obstacles" left with some speed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private float speed;
    private int objType; // 0 for background/ground, 1 for obstacles
    private static float ObstacleSpeed = 10.0f;
    private static float BackgroundSpeed = 5.0f;

    public GameManager gameManager;

    void Start()
    {
        //differnet speed for differt objs
        if (CompareTag("Obstacle")){
            objType = 1;
            speed = ObstacleSpeed;
        }
        else{
            objType = 0;
            speed = BackgroundSpeed;
        }

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // dont update background/ground when game over 
        if (gameManager.gameOver && objType == 0)
            return;

        transform.Translate(Vector3.left * Time.deltaTime * speed);
    }
}
