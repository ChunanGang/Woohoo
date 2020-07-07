// Script that controls the motion of the player
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    private Rigidbody playerRb;
    public float jumpForce = 10; // the jumping force
    public bool isOnGround = true; // whether the player is on ground
    public GameManager gameManager;
    public ParticleSystem fart;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        jumpForce *= playerRb.mass;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        fart = GameObject.Find("Fart").GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // quit if game is over
        if (gameManager.gameOver)
            return;

        // the player jump when "Q" pressed
        if(Input.GetKeyDown(KeyCode.Q)){
            playerRb.AddForce(Vector3.up * jumpForce , ForceMode.Impulse);
            fart.Play();
            isOnGround = false;
        }
        // the player goes down when "A" pressed
        else if (Input.GetKeyDown(KeyCode.A))
        {
            playerRb.AddForce(Vector3.down * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision colliObj)
    {
        if (colliObj.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
        else if (colliObj.gameObject.CompareTag("Obstacle"))
        {
            //Debug.Log(colliObj.contacts[0].normal);
            // game over unless only touch the top
            //if(colliObj.contacts[0].normal != new Vector3(0,1,0))
                gameManager.gameOver = true;
        } 
    }
}
