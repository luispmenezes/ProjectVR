using UnityEngine;
using System.Collections;

public class NetworkBall : MonoBehaviour {

	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
    private Quaternion rotation = Quaternion.identity;

    void Update(){
		
	    if (GetComponent<NetworkView>().isMine){
        	//Do nothing
    	}
    	else{
        	SyncedMovement();
    	}
    }
 
 	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){
	    Vector3 syncPosition = Vector3.zero;
  	    Vector3 syncVelocity = Vector3.zero;
  	    Quaternion syncRotation = Quaternion.identity;

    	if (stream.isWriting){
        	syncPosition = GetComponent<Rigidbody>().position;
        	stream.Serialize(ref syncPosition);
 
        	syncVelocity = GetComponent<Rigidbody>().velocity;
        	stream.Serialize(ref syncVelocity);

        	syncRotation = transform.rotation;
        	stream.Serialize(ref syncRotation);
    	}
    	else{
        	stream.Serialize(ref syncPosition);
        	stream.Serialize(ref syncVelocity);
        	stream.Serialize(ref syncRotation);
 
  	        syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;
 
            syncEndPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = GetComponent<Rigidbody>().position;

            rotation = syncRotation;
    	}
	}

	private void SyncedMovement(){
	    syncTime += Time.deltaTime;
	    GetComponent<Rigidbody>().position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
	    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, syncTime / syncDelay);
	}

}
