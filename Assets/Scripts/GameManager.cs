
// This is the game manager class that controls the game logics,
// as well as the spwaning of obstacles

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Obstacles that move hirizontally
    public GameObject cubeHorizontal;
    // public GameObject carHorizontal;

    public bool gameOver = false;

    // used to control the auto-spwaning of obstacles
    private float startDelay = 2.0f;
    private float interval = 1.5f;
    
    void Start()
    {
        InvokeRepeating("SpawnObstacle", startDelay, interval);   
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
            CancelInvoke();
    }

    // function that generate obstalces
    void SpawnObstacle()
    {
        Instantiate(cubeHorizontal, cubeHorizontal.transform.position, cubeHorizontal.transform.rotation);
    }

}
