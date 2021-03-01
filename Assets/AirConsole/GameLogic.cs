using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
public class GameLogic : MonoBehaviour
{   public GameObject myCube;
    CubeMovement cube_movement_script;
    Vector3 movement;
    // Start is called before the first frame update
    void Start()
    {
        cube_movement_script = myCube.GetComponent<CubeMovement>();
        movement = new Vector3(0,0,1);
    }

    void Awake(){
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnMessage(int fromDeviceID, JToken data){
        Debug.Log("message from" + fromDeviceID + ", data:" + data);
        if (data["move"] != null  && data["move"].ToString().Equals("-2")){
            cube_movement_script.Move(movement);
            Debug.Log("cube pos") ;
        }
    }
    
    void OnDestroy(){

        //unregister events
        if (AirConsole.instance != null){
                AirConsole.instance.onMessage -= OnMessage;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}