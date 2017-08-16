using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class GodMode : MonoBehaviour {
	/*Script Global Vars*/
	private float timer;
	private int objCount;
	private bool configLoaded = false;
	
	/*From Other Scripts*/
	bool gameInitiated;
	private Rigidbody ballRb;
	private Vector3 velocity;
	/*Android Properties*/
	private Touch me; // :3

	/*World Objects - Vars*/
	private Transform theSun;
	private Quaternion rot;
	GameObject obstaclePrefab;
	private Quaternion defRot;
	/*GodMode Variables*/
	private Ray ray;
	private Camera godCam;
	private RaycastHit hit;

	private NetworkView netView;
	public GameObject [] myObjects = new GameObject[5];

	/*UI Variables*/
	private Canvas UICanvas;
	private Slider speedSlider, gravitySlider, todSlider;
	private bool firstTime = true;
	/*Physics Manipulation Variables*/
	int timeOfTheDay;
	bool gravity;
	
	/*TimeOfTheDay = { 0 - Morning, 1 - Noon, 2 - Evening, 3 - Night }; (DONE)
	Bounciness = { 0 - No Bounce, 1 - Little, 2 - Normal, 3 - High };
	Speed = { 0 - Frozen, 1 - Slow, 2 - Normal, 3 - Fast };			  (DONE)
	Gravity = [0 , 20 ]; */

	void Awake()
	{
		/*ND*/
	}

	void Start()
	{    
		if (!GetComponent<NetworkView> ().isMine) {       
			this.GetComponent<Camera> ().enabled = false;
			this.transform.GetChild (0).gameObject.SetActive (false);
			this.transform.GetChild (1).gameObject.SetActive (false);
			this.GetComponent<GodMode> ().enabled = false;
		} else {
			try {
				obstaclePrefab = (Resources.Load ("box") as GameObject);
			} catch (MissingReferenceException nullRef) {
				Debug.LogException (nullRef);

				obstaclePrefab = GameObject.CreatePrimitive (PrimitiveType.Cube);
				obstaclePrefab.AddComponent<Rigidbody> ();
				obstaclePrefab.AddComponent<NetworkView> ();
				obstaclePrefab.AddComponent<NetworkObject> ();
				obstaclePrefab.AddComponent<rigidSync> ();
			}

			if (!obstaclePrefab) {
				Debug.Log ("Error: Obstacle Prefab Not Found!");
			}


			/*Game Start Flag - NOT WORKING (the game starts, var still false)*/
			//gameInitiated = GameObject.Find ("UI_score").GetComponent<GameManager> ().gameInitiated;

			/*THIS cam*/
			godCam = GetComponent<Camera> ();

			/*THIS network view*/
			netView = GetComponent<NetworkView> ();

			/*User Interface Objects*/
			UICanvas = transform.GetComponent<Canvas> ();
			speedSlider = GameObject.Find ("SpeedSlider").GetComponent<Slider> ();
			gravitySlider = GameObject.Find ("GravitySlider").GetComponent<Slider> ();
			todSlider = GameObject.Find ("TodSlider").GetComponent<Slider> ();

			defRot = GameObject.Find ("Directional Light").GetComponent<Transform> ().rotation;
		}
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		/*Global Timer*/
		timer += 1 * Time.deltaTime;
		/*NOT WORKING*/
		//if (gameInitiated) {
			if(!configLoaded)
			{
				LoadUI ();
				configLoaded = true;
			}
			/*Spawn Objects*/
			if (timer >= 0.5f && Input.GetKeyDown (KeyCode.Mouse0)) {
				ray = godCam.ScreenPointToRay (Input.mousePosition);
				Physics.Raycast (ray, out hit);
				if (!OutOfBounds (hit)) {		
					SpawnObject (hit);
				}
				timer = 0;
			}
		//}
	}

	public void UpdateGravity()
	{
		if(gravitySlider.value == 1)
		{	
			gravity = true;
		}else { gravity = false; }

		netView.RPC ("ChangeGravity", RPCMode.AllBuffered, gravity);

	}

	public void UpdateTOD()
	{	
		rot=new Quaternion();

		/*Change Time of day (By Rotating the Light on the main scene)*/
		switch ((int)todSlider.value) {
			case 0:
				rot.eulerAngles = new Vector3(0, 0, 0);
				break;
			case 1:
				rot.eulerAngles = new Vector3(90, 0, 0);
				break;
			case 2:
				rot.eulerAngles = new Vector3(180, 0, 0);
				break;
			case 3:
				rot.eulerAngles = new Vector3(270, 0, 0);
				break;
		}
		netView.RPC ("ChangeTod", RPCMode.AllBuffered,rot);
	}
	
	public void UpdateSpeed()
	{
		netView.RPC ("ChangeVelocity", RPCMode.AllBuffered, speedSlider.value);
	}
	
	void SpawnObject(RaycastHit hit)
	{
		if (GetComponent<NetworkView> ().isMine) {
			/*Object Creation*/
			Vector3 pos = new Vector3 (hit.point.x, 1f, hit.point.z);

			if (objCount >= 5) {
				DestroySpawnedObjects ();
			}
			myObjects [objCount] = (Network.Instantiate (obstaclePrefab, pos, Quaternion.identity, 0)) as GameObject;
			DontDestroyOnLoad (myObjects [objCount]);
			objCount++;
		}
	}

	public void DestroySpawnedObjects()
	{
		int i;
		for (i=0; i<myObjects.Length; i++) {
			Network.Destroy (myObjects [i]);
			myObjects[i] = null;
		}
		objCount = 0;
	}
	
/*
	public void SpawnObjectAndroid()
	{
		ray = Camera.main.ScreenPointToRay (touch.position);
		if (timer >= 0.5f)
		{
				timer = 0;
				clone = Instantiate (GameObject.CreatePrimitive (PrimitiveType.Sphere), new Vector3 (hit.point.x, hit.point.y, hit.point.z), Quaternion.identity) as GameObject;
				clone.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f); //Change primitive size
				DontDestroyOnLoad (clone);
						
				//GameObject spawnedPrefab = Instantiate(prefab, new Vector3(hit.point.x,hit.point.y,hit.point.z), Quaternion.identity) as GameObject;
				//DontDestroyOnLoad(spawnedPrefab);
		}
	}
	
	
/*

	/*Debug Functions*/
	private bool OutOfBounds(RaycastHit hit)
	{	/*
		 *Top Left = {8f, 0f, 13.5f};
		 *Top Right = { 8f, 0f, -10.5f };
		 *Bottom Right = { -8f, 0f, -10.5f };
		 *Bottom Left = { -8f, 0f, 13.5f };
		*/
		if((hit.point.x > -8f && hit.point.x < 8f) && (hit.point.z > -10.5f && hit.point.z < 13.5f))
		{
			return false;
		}else{ return true; }
	}

	public void ExitGame()
	{	//Changing to Main Menu
		Application.LoadLevel (0);
	}

	private void LoadUI()
	{
		speedSlider.enabled = true;
		todSlider.enabled = true;
		gravitySlider.enabled = true;
	}


	/*Remote Procedure Calls*/

	/*Este RPC Pode muito bem ser implementado como um script na luz da cena, poupando utilizaçao da largura de banda nas chamadas remotas (Por Avaliar com o grupo)
	[RPC]
	void RotateSun()
	{	//Day/Night Cycle
		if (Network.isServer) {
			theSun = GameObject.Find ("Directional Light").GetComponent<Transform> ();
			theSun.transform.Rotate (Vector3.right * (2f * Time.deltaTime));
		}
	}*/

	[RPC]
	public void LoadDefaultValues()
	{
		if (!firstTime) {
			GameObject.Find ("Ball(Clone)").GetComponent<Rigidbody> ().useGravity = true;
			GameObject.Find ("Ball(Clone)").GetComponent<Rigidbody> ().velocity = velocity;
			GameObject.Find ("Directional Light").GetComponent<Transform>().transform.rotation = defRot;
		}
	}
	
	[RPC]
	void ChangeVelocity(int scale)
	{
		//ballRb.AddRelativeForce(ballRb.velocity * speed);
		if (firstTime) {
			velocity = GameObject.Find ("Ball(Clone)").GetComponent<Rigidbody> ().velocity;
			firstTime = false;
		}
		if (scale == 1) {
			GameObject.Find ("Ball(Clone)").GetComponent<Rigidbody> ().velocity += new Vector3 (1f, 0f, 1f);
		}else{GameObject.Find ("Ball(Clone)").GetComponent<Rigidbody> ().velocity -= new Vector3 (0.5f, 0f, 0.5f);}
	}

	[RPC]
	void ChangeGravity(bool gravity)
	{
		GameObject.Find ("Ball(Clone)").GetComponent<Rigidbody> ().useGravity = gravity;
	}

	[RPC]
	void ChangeTod(Quaternion rotSun)
	{
		theSun = GameObject.Find ("Directional Light").GetComponent<Transform> ();
		theSun.transform.rotation = rotSun;
	}


}