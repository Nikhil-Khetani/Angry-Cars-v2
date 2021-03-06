using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    Vector3 movement;
    float vel=0.1f;
    int left;
    int right;
    float xrot;
    float zrot;
    float selfRightingTorque = 1.0f;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
      //rb = GetComponent<Rigidbody>();   
      rb.centerOfMass += new Vector3(0,-0.7f,0);
    }
    public void Move(string data){
        switch(data){
        case "2":
			right = 1;
			break;
		case "-2":
			left = 1;
			break;
		case "1":
			right = 0;
			break;
		case "-1":
			left = 0;
			break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate(){
        transform.position += transform.forward * vel;
        transform.Rotate(new Vector3 (0, (right-left), 0));
        if (transform.position.y<-100){
            transform.position = new Vector3(10,10,20);
        }
    }

    public float getx(){
        return transform.position.x;
    }
    public float gety(){
        return transform.position.y;
    }
    public float getz(){
        return transform.position.z;
    }
}
