using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public int connectionPort = 25001;
	public GameObject playerType0, playerType1, playerType2, playerType3, playerType4, playerType5,box;
	public int scorePlayerRight;
	public int scorePlayerLeft;
	public GUIStyle style;
    //indicates wich sensor is being used
	private int sensor_id;

    //indicates which camera type visualiser is being used
	private int camera_id;

    //indicates which position is selected
	private int view_id;
	
	//indicates if the instance is a client or a server (true = server, false = client)
	private bool peerType;

    //indicates the ip to which the client will connect
	private string serverIP;

	//countdown timer
	private bool timer_running = false;
	private float timer = 1000f;

	public bool iAmPlayer;	

	//private bool extr;		

	void Start(){
		sensor_id = UIManagerScript.sensor;
		camera_id = UIManagerScript.cam;
		view_id = UIManagerScript.view;
		peerType = UIManagerScript.is_server;
		serverIP = UIManagerScript.ipAddress;
		//extr = UIManagerScript.extr;

		//if the instance is a server, initiates the server
		if(peerType)
		{
			Network.InitializeServer (32, connectionPort, false);

		}
		else	//if the instance is a client, connects to the server
		{
			Network.Connect (serverIP, connectionPort);
		}
	}

	/**
	 * Sets default values for quick play 
	 */
	void Loaddefault()
	{
		sensor_id = 0;
		camera_id = 0;
		view_id = 0	;
		peerType = true;
		serverIP = null;
	}

	void OnGUI(){
	}

	/*
	 * Spawns a player, since this is pong, the position will be defined by the peerType(client, server) 
	 *
	 */
	public void SpawnPlayer(bool first)
	{
		float z=-18,x=0,y=2;
		int rz=0, rx=0, ry=0;
		GameObject obj = null;

		if(sensor_id!=4 && sensor_id != 5)
			iAmPlayer = true;
		else
			iAmPlayer = false;

		//Set Position Coordinates
		if(first)
		{
            z = 20;
            ry = 180;
		}
		else
		{
			z = -20;
		}

		//Set Prefab
		switch(sensor_id){
			case 0 : 	obj = playerType0; y=4; break;
			case 1 :	obj = playerType1; y=4; if(first) {z+=6;} else {z-=6;} break;
            //case 2: obj = playerType2; y = 5; if (first) { z += 10; WiiScript.posZ = 18f; } else { z -= 10; WiiScript.posZ = -18f; } break;
            case 2: obj = playerType2; y = 4; if (first) { ObsWii.posZ = 18f; } else { ObsWii.posZ = -18f; } break;
			case 3 : 	obj = playerType3; y=3; break;
			case 4 :	obj = playerType4; x=0;y=38;z=0;rx=90;ry=90;iAmPlayer=false;break;
			case 5 :	obj = playerType5; x=0;y=40;z=-5; rx=90; ry=90; break;
		}
		
		//Set Camera Type (for occulus rift)
		switch(camera_id){
			default:
				obj.GetComponent<Camera>().enabled = true;
				obj.transform.GetChild(0).gameObject.SetActive(false);
				break;
			case 1:
				obj.GetComponent<Camera> ().enabled = false;
				obj.transform.GetChild (0).gameObject.SetActive (true);
				break;
		}

		Network.Instantiate(obj, new Vector3(x, y, z), Quaternion.Euler(rx,ry,rz), 0);
		
		Debug.Log("Spawn Player");
	}
}
