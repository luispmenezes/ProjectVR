using UnityEngine;
using System.Collections;

public class rigidSync : MonoBehaviour {

	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;

	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!GetComponent<NetworkView>().isMine){
			SyncedMovement();
		}
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
	    gameObject.GetComponent<Rigidbody>().position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
	}
}
