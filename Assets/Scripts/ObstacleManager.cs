// This scripts handle the generated obstacle (ie. movtion, delete)
// It also notify the game manager when to generate new obstacle-series and when 
//  to update the game socre.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public GameManager gameManager;
    public ObstacleManager thisObstacle;

    private bool isLastObstacleInSeries = false; // whether this obstacle is the last in its series
    private bool usedToUpdateScore = false; // whether this obstacle is used to update the gamescore
    private bool usedToGenNewObs = false; // whether this obstacle is used to notify the gamemanager to generate new obs
    // obstacle will get delete out of this range 
    public static float deleteBoundaryX = -7;
    public static float deleteBoundaryZ = -3;
    // update score when obstacle gets out of this range 
    public static float scoreUpdateBoundaryX = -2;
    public static float scoreUpdateBoundaryZ = -2;
    // notify game manger to generare new obstacles when the last tobstacle gets out of this range
    public static float genNewObsBoundartX = -3;
    public static float genNewObsBoundartZ = -3;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        thisObstacle = GetComponent<ObstacleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // move left
        float speed = GameObject.Find("GameManager").GetComponent<GameManager>().ObstacleSpeed;
        transform.Translate(Vector3.left * Time.deltaTime * speed);

        // checks 
        checkForGenNewObs();
        checkForUpdateScore();
        checkForDelete(); // delet is the last to avoid null-pointer error

    }

    // this function set the object as the last obstacle
    public void setAsLastObstacle()
    {
        GetComponent<ObstacleManager>().isLastObstacleInSeries = true;
    }

    // this function checks whether delete the current obstacle or not
    private void checkForDelete()
    {
        if (transform.position.x < deleteBoundaryX || transform.position.z < deleteBoundaryZ)
            Object.Destroy(gameObject);
    }

    // this function checks whether update the game score or not
    private void checkForUpdateScore()
    {
        if ((transform.position.x < scoreUpdateBoundaryX || transform.position.z < scoreUpdateBoundaryZ)
            && !gameManager.gameOver)
            // update the score for game manager onec per obstacle
            if (!thisObstacle.usedToUpdateScore)
            {
                gameManager.gameScore += GameManager.gameScoreIncr;
                thisObstacle.usedToUpdateScore = true;
            }
    }

    // this function checks whether need to notidy the game manger to spawn new obstacle-series
    private void checkForGenNewObs()
    {
        // spawn if the last obstacle in a series is out of boundary
        if ((transform.position.x < genNewObsBoundartX || transform.position.z < genNewObsBoundartZ)
            && !gameManager.gameOver)
            // notify game manager to generate new obstacle series onec per LAST obstacle
            if (GetComponent<ObstacleManager>().isLastObstacleInSeries && !thisObstacle.usedToGenNewObs)
            {
                gameManager.SpawnObstacleSeries();
                thisObstacle.usedToGenNewObs = true;
            }
    }
}
