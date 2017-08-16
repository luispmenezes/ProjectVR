using UnityEngine;
using System.Collections;

public class Omni : MonoBehaviour {

	public float speed = 10f;
	public float jumpSpeed = 0.1f;
    public float turnspeed = 2f;

	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;


	// Use this for initialization
	void Start () {
        if(!GetComponent<NetworkView>().isMine){       
            this.GetComponent<Camera>().enabled = false; 
            this.transform.GetChild(0).gameObject.SetActive(false);   
        }else{
			GameObject.Find("haptic_controller").GetComponent<SimpleShapeManipulationAndPhysicsJenga>().enabled = true;
        }
	}
	
	// Update is called once per frame
	 void Update(){
		
	    if (GetComponent<NetworkView>().isMine){
        	InputMovement();
        	InputColorChange();
        	//GetComponent<Camera>().enabled = true;
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
 
	[RPC] void ChangeColorTo(Vector3 color){
    	GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);
 
    	if (GetComponent<NetworkView>().isMine)
        	GetComponent<NetworkView>().RPC("ChangeColorTo", RPCMode.OthersBuffered, color);
	}
}
