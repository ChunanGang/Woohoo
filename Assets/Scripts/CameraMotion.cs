// This scripts allows the camera to move with the player 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    Vector3 offset; // the distance between the player and cmr 

    public GameManager gameManager;
    public GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
        offset = transform.position - player.transform.position;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // camera follows the player if game not over
        if (!gameManager.gameOver)
        {
            transform.position = player.transform.position + offset;
        }
    }
}
