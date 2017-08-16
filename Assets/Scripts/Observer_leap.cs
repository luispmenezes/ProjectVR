using UnityEngine;
using System.Collections;

public class Observer_leap : MonoBehaviour {

	public float speed = 10f;
	public float jumpSpeed = 0.1f;
    public float turnspeed = 2f;

	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
    private int cameraType=0;
	
	private bool winner = false;
	private bool looser = false;

    void Start(){
        ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

        if(!GetComponent<NetworkView>().isMine){       
            this.GetComponent<Camera>().enabled = false; 
            this.transform.GetChild(0).gameObject.SetActive(false);
			this.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    void Update(){
		
	    if (GetComponent<NetworkView>().isMine){
        	InputMovement();
        	InputColorChange();
    	}
    	else{
        	SyncedMovement();
    	}
    }
 
    void InputMovement()
    {
        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * speed;
            //GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + Vector3.forward * speed * Time.deltaTime);
 
        if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * speed;
            //GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position - Vector3.forward * speed * Time.deltaTime);
 
        if (Input.GetKey(KeyCode.D))
            transform.Rotate(0,turnspeed,0);
            //GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + Vector3.right * speed * Time.deltaTime);
 
        if (Input.GetKey(KeyCode.A))
            transform.Rotate(0,-turnspeed,0);
            //GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position - Vector3.right * speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space))
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

	private void OnGUI()
	{
		if(GetComponent<NetworkView>().isMine)
		{
			if (winner) {
				Texture2D tex = (Texture2D) Resources.Load("winner");
				GUI.Label (new Rect ((Screen.width - 500) / 2, (Screen.height - 500) / 2, 500, 500), tex);
			}
			if (looser) { 
				Texture2D tex = (Texture2D)Resources.Load ("feia");
				GUI.Label (new Rect ((Screen.width - 500) / 2, (Screen.height - 500) / 2, 500, 500), tex);
			}
		}
	}
 
	[RPC] void ChangeColorTo(Vector3 color){
    	GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);
 
    	if (GetComponent<NetworkView>().isMine)
        	GetComponent<NetworkView>().RPC("ChangeColorTo", RPCMode.OthersBuffered, color);
	}
}
