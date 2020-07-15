// This is the game manager class that controls the game logics,
// as well as the spwaning of obstacles
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ====== Obstacles related ========== //
    public GameObject car; // car obstacle prefab
    public GameObject bird; // bird obstacle prefab
    // used to control the auto-spwaning of obstacles
    private static float ProbDec = .1f; // used to generate obstacle series  
    private static float obstacleDistance = 10.0f; // how far apart each obstacle from each other in one series
    private static float verticalObsProb = 0.5f; // the probability that the obstacle goes vertically 

    // ====== Menu related game object ========= //
    // **For any game object, make sure to initiate in Start() and, if necessary, set it inactive in resetGameState() 
    private GameObject userInterface;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI gameTitleText;
    private TextMeshProUGUI gameOverText;
    private TextMeshProUGUI pauseText;
    private Button startButton;
    private Button restartButton;
    private Button returnToMenuButton;
    private Button resumeButton;
    private Button pauseButton;

    // ====== Game logic =========== //
    public float startDelay = 10;
    public static int gameScoreIncr = 20;
    public bool gameOver = false;
    public bool paused = false;
    public bool inStartMenu = true;
    public int gameScore = 0; // score is added whenever an obstacle is deleted
    // speed
    public static float speedUp = 1.2f;
    public const float initObstacleSpeed = 10.0f; // how fast obstacles move
    public float ObstacleSpeed;
    public float BackgroundSpeed = 5.0f; // how fast the background/ground move
    public Vector3 initGravity;
    
    // ====== GameMode ========== //
    // Mode 1: triggered at start; ususal game;
    // Mode 2: triggered at score> modeScore[1]; game is speeduped by a factor;
    // Mode 3: obstacles go vertically;
    private int mode = 0; // increase by 1 each time
    private int[] modeScore = { 0, 200, 400 }; // decide when mode is updated

    // ====== Player related ======== //
    private GameObject playerObj;
    private Transform initialPlayerTrans;

    void Start()
    {
        // find objects reference
        userInterface = GameObject.Find("Canvas").gameObject;
        scoreText = userInterface.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        gameTitleText = userInterface.transform.Find("GameTitle").GetComponent<TextMeshProUGUI>();
        gameOverText = userInterface.transform.Find("GameOverText").GetComponent<TextMeshProUGUI>();
        pauseText = userInterface.transform.Find("PauseText").GetComponent<TextMeshProUGUI>();
        startButton = userInterface.transform.Find("StartButton").GetComponent<Button>();
        restartButton = userInterface.transform.Find("RestartButton").GetComponent<Button>();
        returnToMenuButton = userInterface.transform.Find("ReturnToMenuButton").GetComponent<Button>();
        resumeButton = userInterface.transform.Find("ResumeButton").GetComponent<Button>();
        pauseButton = userInterface.transform.Find("PauseButton").GetComponent<Button>();
        playerObj = GameObject.Find("Player").gameObject;

        // initialize an empty transform, to avoid copying the reference instead of the value
        initialPlayerTrans = new GameObject().transform; 
        initialPlayerTrans.position = playerObj.transform.position;
        initialPlayerTrans.rotation = playerObj.transform.rotation;

        // initilizations
        ObstacleSpeed = initObstacleSpeed;
        initGravity = Physics.gravity;

        EnterStartMenu();
    }

    // Update is called once per frame
    void Update()
    {
        // update text
        scoreText.text = "Score: " + gameScore.ToString();
        if (gameOver)
        {
            gameOverText.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            returnToMenuButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
        }
    }

    // function that generate obstalces series in the scene 
    public void SpawnObstacleSeries()
    {
        GameObject lastObstacle = null; // record the last created obstalce
        float distance = 0.0f;

        // generate obstacle series
        string obstacleSeries = GenerateObstacleSeries("", 1);
        foreach (char type in obstacleSeries)
        {
            lastObstacle = createObstacle(type, distance);
            // increase distance
            distance += obstacleDistance;
        }
        // mark the last created obstalce in the series
        lastObstacle.GetComponent<ObstacleManager>().setAsLastObstacle();

        // check for mode update
        checkForUpdateMode();
    }

    // Functions that generate one obstacle in the scene.
    // It will randomly decide whther the obstalce goes vertically or horizontally.
    // type: whther the obstacle should be a car or bird. '1' for car, '2' for bird
    // distance: used to decide the creating location
    // Return the refernence to the obstalce created
    GameObject createObstacle(char type, float distance)
    {
        GameObject creatingObstacle;
        Vector3 creatingPosistion;
        Quaternion creatingRotaion;
        string finalType; // "1" = carHorizontal; "2" = birdHorizontal; "3" = carVertical; "4" = birdVertical 

        // get obstalce's general type
        if (type == '1')
            creatingObstacle = car;
        else
            creatingObstacle = bird;

        // randomly decide whther the obstalce goes vertically or horizontally
        // goes vertically
        if (mode > 1 && Random.Range(0.0f, 1.0f) < verticalObsProb)
        {
            // set position and rotaion
            creatingPosistion = new Vector3(creatingObstacle.transform.position.z,
                creatingObstacle.transform.position.y,
                creatingObstacle.transform.position.x + distance);
            creatingRotaion = Quaternion.AngleAxis(-90, Vector3.up);
            // set final type
            if (type == '1') // vertical car
                finalType = "3";
            else             // vertical bird
                finalType = "4";
        }
        // goes horizontal
        else
        {
            // set position and rotaion
            creatingPosistion = new Vector3(creatingObstacle.transform.position.x + distance,
                creatingObstacle.transform.position.y,
                creatingObstacle.transform.position.z);
            creatingRotaion = creatingObstacle.transform.rotation;
            finalType = type.ToString();
        }
        // create in the scnee
        creatingObstacle = Instantiate(creatingObstacle, creatingPosistion, creatingRotaion);
        // set the type
        creatingObstacle.GetComponent<ObstacleManager>().setType(finalType.ToString());

        return creatingObstacle;
    }

    // functions that generate a series of ostacles that will follow the prevObstacle.
    // Obstacle notation: "1" = car; "2" = bird
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
                return "12" + GenerateObstacleSeries("2", prob - 2*ProbDec);
        }

        // otherwise keep generating
        else
            return nextObstacle + GenerateObstacleSeries(nextObstacle, prob - ProbDec);
      
    }

    // This function chekc whether should move to the next mode
    // If so, move to next mode
    void checkForUpdateMode()
    {
        if (mode == 0) {
            if (gameScore > modeScore[1])
            {
                // speed up the whole game
                Physics.gravity *= speedUp;
                ObstacleSpeed *= speedUp;
                mode = 1;
            }
        }
        else if (mode == 1)
            if (gameScore > modeScore[2])
                mode = 2;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        resetGameState();

        scoreText.gameObject.SetActive(true);
        Invoke("SpawnObstacleSeries", startDelay);
    }

    public void EnterStartMenu()
    {
        resetGameState();
        
        gameTitleText.gameObject.SetActive(true);
        startButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        inStartMenu = true;
    }
    
    // This function is triggered by the pause action from user
    // It either puase or resume-from-pause
    public void TogglePause()
    {
        paused = !paused;
        // pressed to pause
        if (paused) {
            Time.timeScale = 0;
            pauseText.gameObject.SetActive(true);
            resumeButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
        }
        // pressed to resume
        else {
            Time.timeScale = 1;
            pauseText.gameObject.SetActive(false);
            resumeButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
        }
    }

    // This function resets the game state.
    // It is called before every start.
    private void resetGameState()
    {
        // set buttons
        scoreText.gameObject.SetActive(false);
        gameTitleText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        pauseText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        returnToMenuButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);

        // remove obstacle in the scene
        removeAllObstacles();

        // reset player's position
        playerObj.transform.position = initialPlayerTrans.position;
        playerObj.transform.rotation = initialPlayerTrans.rotation;

        // game logic
        gameOver = false;
        if (paused) { TogglePause(); }
        inStartMenu = false;

        // reset game settings
        gameScore = 0;
        mode = 0;
        ObstacleSpeed = initObstacleSpeed;
        Physics.gravity = initGravity;
    }

    // Remove all obstacle in the scene
    private void removeAllObstacles() 
    {
        foreach(GameObject obj in GameObject.FindObjectsOfType(typeof(GameObject))) {
            if (obj.CompareTag("Obstacle")) {
                Object.Destroy(obj); 
            }
        }
    } 
}
