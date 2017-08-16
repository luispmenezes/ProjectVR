using UnityEngine;
using System.Collections;

public class Observer : MonoBehaviour {

	public float speed = 10f;
	public float jumpSpeed = 0.1f;
    public float turnspeed = 2f;

	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
    private int cameraType=0;

	private bool dead = false;
	public int hp = 100;

	//game timer
	/*
	private float timer;
	private bool timer_running;*/

	private bool winner = false;

    void Start(){
        ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

        if(!GetComponent<NetworkView>().isMine){       
            this.GetComponent<Camera>().enabled = false; 
            this.transform.GetChild(0).gameObject.SetActive(false);
			//this.transform.GetChild(1).gameObject.SetActive(false);
        }

		//inicializacao do timer
		/*
		GameObject netManager = GameObject.Find ("NetManager");
		NetworkManager nmScript = netManager.GetComponent("NetworkManager") as NetworkManager;*/
		/*
		timer = nmScript.getTimer ();
		timer_running = true;*/

    }

    void Update(){
		
	    if (GetComponent<NetworkView>().isMine){
        	InputMovement();
        	InputColorChange();
    	}
    	else{
        	SyncedMovement();
    	}

		//if no health, looses
		if (hp == 0) 
			LostTheGame();

		//update game timer
		/*
		if (timer <= 0f)
			timer_running = false;
		else if (timer_running)
			timer -= Time.deltaTime;
		*/
		
    }

    
	private void LostTheGame()
	{
		GameObject shooter = GameObject.Find ("leapController(Clone)");
		NetworkView netView_shooter = (NetworkView)shooter.GetComponent<NetworkView> ();
		netView_shooter.RPC ("target_hit", RPCMode.OthersBuffered);
	}
 
    void InputMovement()
    {
       // if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * speed;
            //GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + Vector3.forward * speed * Time.deltaTime);
 
       // if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * speed;
            //GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position - Vector3.forward * speed * Time.deltaTime);
 
        if (Input.GetKey(KeyCode.D))
			transform.position += transform.right * speed;
            //transform.Rotate(0,turnspeed,0);
            //GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + Vector3.right * speed * Time.deltaTime);
 
        if (Input.GetKey(KeyCode.A))
			transform.position -= transform.right * speed;
           // transform.Rotate(0,-turnspeed,0);
            //GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position - Vector3.right * speed * Time.deltaTime);

      //  if (Input.GetKey(KeyCode.Space))
            GetComponent<Rigidbody>().velocity =  new Vector3(0,jumpSpeed,0);    

    }
 
 	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){
	    Vector3 syncPosition = Vector3.zero;
  	    Vector3 syncVelocity = Vector3.zero;
    	if (stream.isWriting){
        	syncPosition = GetComponent<Rigidbody>().position;
        	stream.Serialize(ref syncPosition);
 
        	syncVelocity = GetComponent<Rigidbody>().velocity;
        	stream.Serialize(ref syncVelocity);
    	}
    	else{
        	stream.Serialize(ref syncPosition);
        	stream.Serialize(ref syncVelocity);
 
  	        syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;
 
            syncEndPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = GetComponent<Rigidbody>().position;
    	}
	}

	private void SyncedMovement(){
	    syncTime += Time.deltaTime;
	    GetComponent<Rigidbody>().position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
	}

	private void InputColorChange(){
    
    	if (Input.GetKeyDown(KeyCode.R))
        	ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
	}

	void OnGUI()
	{
		if (gameObject.GetComponent<NetworkView> ().isMine) {
			if (dead) { 
				Texture2D tex = (Texture2D)Resources.Load ("feia");
				GUI.Label (new Rect ((Screen.width - 500) / 2, (Screen.height - 500) / 2, 500, 500), tex);
			}

			if(winner)
			{
				Texture2D tex = (Texture2D) Resources.Load("winner");
				GUI.Label (new Rect ((Screen.width - 500) / 2, (Screen.height - 500) / 2, 500, 500), tex);
			}
			/*
			GUI.Box(new Rect(Screen.width-80, 40, 60, 23), "HP "+hp);*/

			/*
			if(timer_running)
				GUI.Label (new Rect((Screen.width+60)/2, 20, 40, 20), ((int) timer).ToString ());*/
		}
	}
 
	[RPC] void ChangeColorTo(Vector3 color){
    	GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);
 
    	if (GetComponent<NetworkView>().isMine)
        	GetComponent<NetworkView>().RPC("ChangeColorTo", RPCMode.OthersBuffered, color);
	}


	//MODIFICACOES RPCS
	[RPC]
	bool gotShot()
	{
		if (hp > 0) 
			hp -= 10;
		else
			dead = true;
		return dead;

	}

	/*
	public void timerExpired()
	{
		if (!dead)
			winner = true;
	}*/
}
