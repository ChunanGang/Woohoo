// Script that controls the motion of the player
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    private Rigidbody playerRb;
    public float jumpForce;
    public float secondJumpForce;  // control the force at the second jump
    public bool isOnGround = true; // whether the player is on ground
    public int jumpCount = 0;

    GameManager gameManager;

    /* Animation related */
    Animator anim;
    
    /* Constants */
    private static int MAX_JUMP_COUNT = 2; // the player can only limited amount of tiems
    private static float jumpSoundVolume = 0.3f;

    /* sound */
    public AudioClip jumpSound;
    public AudioSource audioSource;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        jumpForce *= playerRb.mass;
        secondJumpForce *= playerRb.mass;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();

        // animation setup
        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("Walk", true);
    }

    void Update()
    {
        // quit if game is over or in start menu
        if (gameManager.gameOver || gameManager.inStartMenu)
            return;

        // pause or unpause when "P" pressed
        if (Input.GetKeyDown(KeyCode.P)) {
            gameManager.TogglePause();
        }

        // quit if paused
        if (gameManager.paused) {
            return;
        }

        // the player jump when "Q" pressed
        if (Input.GetKeyDown(KeyCode.Q) && jumpCount < MAX_JUMP_COUNT) {
            // first jump
            if (jumpCount == 0)
                playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // second jump, diff force mode
            else
                playerRb.AddForce(Vector3.up * secondJumpForce, ForceMode.Impulse);

            // play the sound
            audioSource.PlayOneShot(jumpSound, jumpSoundVolume);

            isOnGround = false;
            jumpCount++;
            anim.SetTrigger("Jump");      
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
            jumpCount = 0;
            anim.ResetTrigger("Jump");
        }
        else if (colliObj.gameObject.CompareTag("Obstacle"))
        {
            gameManager.gameOver = true;
            anim.SetBool("Walk", false);
            playerRb.constraints = RigidbodyConstraints.None;
        } 
    }
}
