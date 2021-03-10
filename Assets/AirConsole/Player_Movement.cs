using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Player_Movement : MonoBehaviour
{
    Vector3 movement;
    public float vel=0.1f;
    int left;
    int right;
    float xrot;
    float zrot;
    public Rigidbody rb;
    public PathCreator pathCreator;
    private float pathPosition;
    private float prevDist;
    private float newDist;
    private int lap;
    private bool state;
    //1=normal, 2=frozen, 3=exploded
    // Start is called before the first frame update
    void Start()
    {
      pathCreator = GameObject.Find("Path").GetComponent<PathCreator>();
      //rb = GetComponent<Rigidbody>();   
      rb.centerOfMass += new Vector3(0,-0.4f,0);
      lap=0;
    }
    public void initialise(float velocity){
        vel = velocity;
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
        if(left==1 && right==1){
            rb.AddForce(transform.forward*-0.1f);
        }
        else{
        rb.AddForce(transform.forward * vel);
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.x = 0;
        rb.velocity= transform.TransformDirection(localVelocity);
        }


        transform.Rotate(new Vector3 (0, (right-left), 0));
        if (transform.position.y<-100){
            transform.position = new Vector3(10,10,20);
        }
    }


    public void explode(){
        Debug.Log("Exploded");
    }

    public float getPlayerDistanceAlongPath(){
        newDist = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        if(newDist <= 20 && prevDist >= pathCreator.path.GetLength()-20){
            lap+=1;
        }
        else if (prevDist <= 20 && newDist >= pathCreator.path.GetLength()-20){
            lap-=1;
        }
        prevDist = newDist;
        return newDist;
    }



    public int getLap(){
        return lap;
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
