using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{   
    
    Vector3 movement;
    float vel=0.1f;
    int left;
    int right;
    float xrot;
    float zrot;
    float selfRightingTorque = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        movement = new Vector3(0.0f,0.0f,0.01f);
        left = 0;
        right = 0;
        xrot = 0.0f;
        zrot = 0.0f;
        
    }
    
    public void TurnLeft(){
        left = 1;
    }
    public void StopLeft(){
        left = 0;
    }
    public void TurnRight(){
        right=1;
    }
    public void StopRight(){
        right=0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        transform.position += transform.forward * vel;

        transform.Rotate(new Vector3 (0, (right-left)*5, 0));
        
        

        
    }
}
