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
    public bool driving;
    private bool exploded;

    public float centerOfMassDepth;
    private Material alive_material;
    public Material exploded_material;
    //1=normal, 2=frozen, 3=exploded
    // Start is called before the first frame update

    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public int points=0;
    

    void Start()
    {
      pathCreator = GameObject.Find("Path").GetComponent<PathCreator>();
      //rb = GetComponent<Rigidbody>();   
      rb.centerOfMass += new Vector3(0, centerOfMassDepth,0);
      lap=0;
      
    }

    public void initialise(float velocity, Material mat, bool driv){
        vel = velocity;
        alive_material = mat;
        driving = driv;
        this.GetComponent<Renderer>().material=alive_material;
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

    public void restartCar(Vector3 newPosition, Vector3 newOrientation){
        //Debug.Log("Restart this car");
        driving = false;
        exploded = false;
        this.GetComponent<Renderer>().material = alive_material;
        transform.position = newPosition;
        transform.rotation = Quaternion.LookRotation(newOrientation);
        rb.velocity = new Vector3(0,0,0);
    }

    public void Go(){
        Debug.Log("Go from player");
        driving=true;
    }

    void FixedUpdate(){
        float steering;
        float motor;
        if (driving){
        motor = maxMotorTorque * vel;
        steering = maxSteeringAngle * (right-left);
        }
        else{
            motor = 0;
            steering = 0;
        }
        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
                
            }
        
        }
    }

    public void explode(){
        Debug.Log("Exploded");
        driving=false;
        exploded = true;
        this.GetComponent<Renderer>().material = exploded_material;
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
    public float getPlayerDistanceToPath(){
        return Vector3.Distance(transform.position,pathCreator.path.GetClosestPointOnPath(transform.position));
    }
    public void changePoints(int change){
        points+=change;
        if(points<0){
            points=0;
        }
    }

    public bool checkIfExploded(){
        return driving;
    }
    public int getPoints(){
        return points;
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
[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}