
// This is the game manager class that controls the game logics,
// as well as the spwaning of obstacles

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Obstacles that move hirizontally
    public GameObject carHorizontal;
    public GameObject birdHorizontal;

    // public GameObject carHorizontal;

    public bool gameOver = false;

    // used to control the auto-spwaning of obstacles
    private float startDelay = 2.0f;
    private float interval = 1.5f;

    private static float ProbDec = .1f; // used to generate obstacle series  
    private static float obstacleDistance = 10.0f; // how far apart each obstacle from each other in one series
   
    
    void Start()
    {
        //InvokeRepeating("SpawnObstacle", startDelay, interval);
        Invoke("SpawnObstacleSeries", 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // function that generate obstalces series in the scene 
    public void SpawnObstacleSeries()
    {
        GameObject lastObstacle = null; // record the last created obstalce
        GameObject creatingObstacle; // the curring creating object
        float distance = 0.0f;

        // generate obstacle series
        string obstacleSeries = GenerateObstacleSeries("", 1);
        foreach (char obstacle in obstacleSeries)
        {
            // get obstalce type
            if (obstacle == '1')
                creatingObstacle = carHorizontal;
            else
                creatingObstacle = birdHorizontal;
            // create in the scnee
            Vector3 createPosistion = new Vector3(creatingObstacle.transform.position.x + distance,
                       creatingObstacle.transform.position.y,
                       creatingObstacle.transform.position.z);
            lastObstacle = Instantiate(creatingObstacle, createPosistion, creatingObstacle.transform.rotation);
            // move distance
            distance += obstacleDistance;
        }

        // mark the last created obstalce in the series
        lastObstacle.GetComponent<DeleteObstacle>().setAsLastObstacle();
    }

    // functions that generate a series of ostacles that will follow the prevObstacle.
    // Obstacle notation: "1" = carHorizontal; "2" = birdHorizontal
    // prevObstacle: the previous one obstacle generated
    // prob: probability that the next ostacle will get generated. 
    //      function will return when prob <= 0
    string GenerateObstacleSeries(string prevObstacle, float prob)
    {
        // check for stoping 
        if (Random.Range(0.0001f, 1.0f) > prob)
            return "";

        // the next ostacle will get generated
        int nextObstacleInt = Random.Range(1, 3); // random int between [1,2]
        string nextObstacle = nextObstacleInt.ToString();

        // there can be at most two continuous car (cannot be 3)
        if ( prevObstacle == "1" && nextObstacle == "1")
        {
            // not next obstacle (stop atfer two conti-cars)
            if (Random.Range(0.0001f, 1.0f) > prob-ProbDec)
                return "1";
            // have next obstacle (must be bird)
            else
                return "12" + GenerateObstacleSeries(nextObstacle, prob - 2*ProbDec);
        }

        // otherwise keep generating
        else
            return nextObstacle + GenerateObstacleSeries(nextObstacle, prob - ProbDec);
      
    }

}
