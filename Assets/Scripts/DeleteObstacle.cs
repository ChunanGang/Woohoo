// Delete Obstacles when they are out od scene 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObstacle : MonoBehaviour
{
    public float boundaryLeft = -10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < boundaryLeft)
        {
            Object.Destroy(gameObject);
        }
    }
}
