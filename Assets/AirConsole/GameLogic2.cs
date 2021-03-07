using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class GameLogic2 : MonoBehaviour {

	public GameObject playerPrefab;

	public Dictionary<int, Player_Movement> players = new Dictionary<int, Player_Movement> (); 
	public Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject> (); 

	private Vector3 camera_pos;
	private Vector3 camera_offset;


	void Awake () {
		AirConsole.instance.onMessage += OnMessage;		
		AirConsole.instance.onReady += OnReady;		
		AirConsole.instance.onConnect += OnConnect;	
		camera_offset = new Vector3 (-30,30,-30);
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
		camera_pos = new Vector3(0,0,0);
		foreach (var player in players)
		{
			camera_pos+=new Vector3 (player.Value.getx(),player.Value.gety(),player.Value.getz());	
		}
		camera_pos/=players.Count;
		camera_pos = camera_pos+camera_offset;

		Debug.Log(camera_pos);
		Camera.main.transform.position = camera_pos;
	}
}