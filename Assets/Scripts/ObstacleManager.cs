// This scripts handle the generated obstacle (ie. movtion, delete)
// It also notify the game manager when to generate new obstacle-series and when 
//  to update the game socre.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject player;

    // ======== Parameters ====== //
    // obstacle will get delete out of this range 
    public static float deleteBoundaryX = -7;
    public static float deleteBoundaryZ = -10;
    // update score when obstacle gets out of this range 
    public static float scoreUpdateBoundaryX = -2;
    public static float scoreUpdateBoundaryZ = -2;
    // notify game manger to generare new obstacles when the last tobstacle gets out of this range
    public static float genNewObsBoundartXZ = -6;
    // play alert sound when obstacle get into this range
    public static float playAlertSoundBoundaryZ = 14;

    // ======== obstacle info ====== // 
    private string type; // "1" = carHorizontal; "2" = birdHorizontal; "3" = carVertical; "4" = birdVertical 
    private bool isLastObstacleInSeries = false; // whether this obstacle is the last in its series
    private bool usedToUpdateScore = false; // whether this obstacle is used to update the gamescore
    private bool usedToGenNewObs = false; // whether this obstacle is used to notify the gamemanager to generate new obs
    private bool playedAlertSound = false; // wther this obstacle alerted the player

    // ======== sound  ====== //
    public AudioClip alertSoundJump; // audio that reminds player to jump
    public AudioClip alertSoundDash; // audio that reminds player to dash down
    public AudioSource audioSource;
    private static float jumpAlertSoundVolume = 1.4f;
    private static float dashAlertSoundVolume = 1.0f;


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
        audioSource = GetComponent<AudioSource>();
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
        checkForSoundAlert();
    }

    // this function set the object as the last obstacle
    public void setAsLastObstacle()
    {
        isLastObstacleInSeries = true;
    }

    // set the type of this obstacle (called by game manager whne the obstacle created)
    public void setType(string type)
    {
        this.type = type;
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
            if (!usedToUpdateScore)
            {
                gameManager.gameScore += GameManager.gameScoreIncr;
                usedToUpdateScore = true;
            }
    }

    // this function checks whether need to notidy the game manger to spawn new obstacle-series
    private void checkForGenNewObs()
    {
        // spawn if the last obstacle in a series is out of boundary
        if ((transform.position.x < genNewObsBoundartXZ || transform.position.z < genNewObsBoundartXZ)
            && !gameManager.gameOver)
            // notify game manager to generate new obstacle series onec per LAST obstacle
            if (GetComponent<ObstacleManager>().isLastObstacleInSeries && !usedToGenNewObs)
            {
                gameManager.SpawnObstacleSeries();
                usedToGenNewObs = true;
            }
    }

    // This function checks whether this obstacle should play alert sound
    private void checkForSoundAlert()
    {
        // stop if game is already over
        if (gameManager.gameOver)
            return;

        // alert for carVertical
        if(type=="3" && !playedAlertSound)
        {
            if (transform.position.z < playAlertSoundBoundaryZ)
            {
                audioSource.PlayOneShot(alertSoundJump, jumpAlertSoundVolume);
                playedAlertSound = true;
            }
        }
        // alert for birdVertical
        else if(type == "4" && !playedAlertSound && player.transform.position.y > transform.position.y-1)
        {
            if (transform.position.z < playAlertSoundBoundaryZ)
            {
                audioSource.PlayOneShot(alertSoundDash, dashAlertSoundVolume);
                playedAlertSound = true;
            }
        }
    }
}
