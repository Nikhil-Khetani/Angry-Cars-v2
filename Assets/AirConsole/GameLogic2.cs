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
	[Range (0,50.0f)]
	public float maxDistanceBehind =20.0f;
	private Vector3 pathDirection;
	private float pathLength;
	private int firstPlayerLap;
	private int prevFirstPlayerLap;
	private int i=0;
	private int alive_players;
	private bool driving;
	private Vector3 cornerStartPosition;
	private Vector3 startPosition;

	private List<Material> CarMats = new List<Material> ();
	private Material CarMat;

	private GameObject myPath;

	public float CarVelocity=0.1f;
	[Range (0, 50.0f)]
	public float maxDistanceFromPath=20.0f;



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

	void Start(){
		StartCoroutine(restartCoroutine());
	}

	   
    

    IEnumerator restartCoroutine()
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);
        yield return new WaitForSeconds(30);
		//Debug.Log("End Coroutine at timestamp : " + Time.time);
		prevFirstDistanceAlongPath = 0;
		FirstDistanceAlongPath = 0;
		restartAll(50);
		
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

		newPlayer.GetComponent<Player_Movement>().initialise(CarVelocity, CarMats[deviceID], driving);
		playerObjects.Add(deviceID, newPlayer);
		players.Add(deviceID, newPlayer.GetComponent<Player_Movement>());
		
	}
	
	private void RemovePlayer(int deviceID){
		Destroy(playerObjects[deviceID], 0f);
	}

	void OnMessage (int from, JToken data){
		Debug.Log ("message: " + data);

		if (players.ContainsKey (from) && data["move"] != null) {
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
				explodePlayer(player.Value);
			}
			if(player.Value.getPlayerDistanceToPath()>maxDistanceFromPath){
				explodePlayer(player.Value);
			}
		}

		if (alive_players<2){
			foreach(var player in players){
				if(player.Value.checkIfExploded()==false){
					player.Value.changePoints(2);
				}
			}
			restartAll(prevFirstDistanceAlongPath-20);
		}

		pathDirection = pathCreator.path.GetDirectionAtDistance(prevFirstDistanceAlongPath);
		camera_offset = -20*pathDirection + camera_y_offset;
		camera_pos = pathCreator.path.GetPointAtDistance(prevFirstDistanceAlongPath) + camera_offset;

		Camera.main.transform.rotation = Quaternion.LookRotation(pathDirection+ new Vector3(0,-1,0));
		Camera.main.transform.position = camera_pos;


	}

	void explodePlayer(Player_Movement player){
		if(alive_players == players.Count){
			player.changePoints(-2);
		}
		if(players.Count>4){
			if(alive_players == players.Count-1){
				player.changePoints(-1);
			}
			else if(alive_players == 2){
				player.changePoints(1);
			}
		}
		alive_players-=1;

	}

	void restartAll(float dist){
		alive_players=0;
		foreach (var player in players)
		{
			restart(dist-5, player.Key , player.Value);
			alive_players+=1;
		}
		pathDirection = pathCreator.path.GetDirectionAtDistance(dist-10);
		camera_offset = -20*pathDirection + camera_y_offset;
		camera_pos = pathCreator.path.GetPointAtDistance(dist-10) + camera_offset;

		Camera.main.transform.rotation = Quaternion.LookRotation(pathDirection+ new Vector3(0,-1,0));
		Camera.main.transform.position = camera_pos;
		StartCoroutine(GoCoroutine());
		
	}

	void restart(float pathPosition, int position, Player_Movement player){
		//Debug.Log("Restart from GameLogic");
		cornerStartPosition = pathCreator.path.GetPointAtDistance(pathPosition) + Quaternion.LookRotation(pathCreator.path.GetDirectionAtDistance(pathPosition))* new Vector3(-4,5,10);
		position-=1;
		driving = false;
		startPosition = cornerStartPosition + new Vector3(2.5f*(position%4),0,8.5f*(position-(position%4)) );
		player.restartCar(startPosition, pathCreator.path.GetDirectionAtDistance(pathPosition) );		
	}

	IEnumerator GoCoroutine()
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);
        yield return new WaitForSeconds(3);
		//Debug.Log("End Coroutine at timestamp : " + Time.time);
		foreach (var player in players)
		{
			player.Value.Go();
			Debug.Log("Go from player");
		}
    }
}