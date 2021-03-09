using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using PathCreation;

public class GameLogic2 : MonoBehaviour {

	public GameObject playerPrefab;

	public Dictionary<int, Player_Movement> players = new Dictionary<int, Player_Movement> (); 
	public Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject> (); 

	private Vector3 camera_pos;
	private Vector3 camera_offset;
	public Vector3 camera_y_offset;

	public PathCreator pathCreator;
	private float prevFirstDistanceAlongPath;
	private float FirstDistanceAlongPath;
	private float playerDist;
	public float maxDistanceBehind = 100;
	private Vector3 pathDirection;
	private float pathLength;
	private int firstPlayerLap;
	private int prevFirstPlayerLap;
	private int i=0;

	private List<Material> CarMats = new List<Material> ();
	private Material CarMat;

	private GameObject myPath;

	public float CarVelocity=0.1f;
	[Range (0, 10.0f)]
	public float maxDistanceFromPath = 20.0f;



	void Awake () {
		AirConsole.instance.onMessage += OnMessage;		
		AirConsole.instance.onReady += OnReady;		
		AirConsole.instance.onConnect += OnConnect;	
		camera_y_offset = new Vector3 (0,30,0);
		myPath = GameObject.Find("Path");
		pathCreator = myPath.GetComponent<PathCreator>();
		pathLength = pathCreator.path.GetLength();
		
		for(i=0; i<9; i++){
			CarMat = Resources.Load<Material>("Car"+ i.ToString());
			CarMats.Add(CarMat);
		}

	}

	void OnReady(string code){
		//Since people might be coming to the game from the AirConsole store once the game is live, 
		//I have to check for already connected devices here and cannot rely only on the OnConnect event 
		List<int> connectedDevices = AirConsole.instance.GetControllerDeviceIds();
		foreach (int deviceID in connectedDevices) {
			AddNewPlayer (deviceID);
		}
	}

	void OnConnect (int device){
		AddNewPlayer (device);
	}
	void OnDisconnect (int device){
		RemovePlayer(device);
	}

	private void AddNewPlayer(int deviceID){

		if (players.ContainsKey (deviceID)) {
			return;
		}

		//Instantiate player prefab, store device id + player script in a dictionary
		GameObject newPlayer = Instantiate (playerPrefab, transform.position+= new Vector3(2*deviceID, 0,-2*deviceID), transform.rotation) as GameObject;
		newPlayer.GetComponent<Renderer>().material = CarMats[deviceID];
		newPlayer.GetComponent<Player_Movement>().initialise(CarVelocity);
		playerObjects.Add(deviceID, newPlayer);
		players.Add(deviceID, newPlayer.GetComponent<Player_Movement>());
		
	}
	
	private void RemovePlayer(int deviceID){
		Destroy(playerObjects[deviceID], 0f);
	}

	void OnMessage (int from, JToken data){
		Debug.Log ("message: " + data);

		//When I get a message, I check if it's from any of the devices stored in my device Id dictionary
		if (players.ContainsKey (from) && data["move"] != null) {
			//I forward the command to the relevant player script, assigned by device ID
			players [from].Move(data["move"].ToString());
		}
	}

	void OnDestroy () {
		if (AirConsole.instance != null) {
			AirConsole.instance.onMessage -= OnMessage;		
			AirConsole.instance.onReady -= OnReady;		
			AirConsole.instance.onConnect -= OnConnect;		
		}
	}

	void FixedUpdate(){
		if (players.Count<1){
			return;
		}
		foreach (var player in players)
		{
			prevFirstDistanceAlongPath = FirstDistanceAlongPath;
			prevFirstPlayerLap = firstPlayerLap;

			playerDist = player.Value.getPlayerDistanceAlongPath();

			if(FirstDistanceAlongPath + pathLength *firstPlayerLap < playerDist+ pathLength * player.Value.getLap()){
				FirstDistanceAlongPath = playerDist;
				firstPlayerLap = player.Value.getLap();
			}

			else if(prevFirstDistanceAlongPath - playerDist>maxDistanceBehind){
				player.Value.explode();
			}

		}
		pathDirection = pathCreator.path.GetDirectionAtDistance(prevFirstDistanceAlongPath);
		camera_offset = -20*pathDirection + camera_y_offset;
		camera_pos = pathCreator.path.GetPointAtDistance(prevFirstDistanceAlongPath) + camera_offset;

		Camera.main.transform.rotation = Quaternion.LookRotation(pathDirection+ new Vector3(0,-1,0));
		Camera.main.transform.position = camera_pos;


	}
}