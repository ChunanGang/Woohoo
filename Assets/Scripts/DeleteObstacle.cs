// Delete Obstacles when they are out od scene 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObstacle : MonoBehaviour
{
    public float boundaryLeft = -7;
    public GameManager gameManager;
    private bool isLastObstacleInSeries = false; // whether this obstacle is the last in its series
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < boundaryLeft)
        {
            // call the game manager to spawn a new obstalce series, when the last obstacle 
            //  in the previosu series is deleted, and when game is not over
            if (GetComponent<DeleteObstacle>().isLastObstacleInSeries && !gameManager.gameOver)
                gameManager.SpawnObstacleSeries();
            Object.Destroy(gameObject);
        }
    }

    // this function set the object as the last obstacle
    public void setAsLastObstacle()
    {
        GetComponent<DeleteObstacle>().isLastObstacleInSeries = true;
    }
}
