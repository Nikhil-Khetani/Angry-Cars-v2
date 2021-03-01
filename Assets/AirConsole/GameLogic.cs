using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
public class GameLogic : MonoBehaviour
{   public GameObject myCube;
    CubeMovement cube_movement_script;

    
    // Start is called before the first frame update
    void Start()
    {
        cube_movement_script = myCube.GetComponent<CubeMovement>();

    }

    void Awake(){
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnMessage(int fromDeviceID, JToken data){
        Debug.Log("message from" + fromDeviceID + ", data:" + data);
        if (data["move"] != null  && data["move"].ToString().Equals("-2")){
            cube_movement_script.TurnLeft();
            Debug.Log("TurnLeft") ;
        }
        if (data["move"] != null  && data["move"].ToString().Equals("-1")){
            cube_movement_script.StopLeft();
            Debug.Log("TurnLeft") ;
        }
        if (data["move"] != null  && data["move"].ToString().Equals("2")){
            cube_movement_script.TurnRight();
            Debug.Log("TurnRight") ;
        }
        if (data["move"] != null  && data["move"].ToString().Equals("1")){
            cube_movement_script.StopRight();
            Debug.Log("TurnRight") ;
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