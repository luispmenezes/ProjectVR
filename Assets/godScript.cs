using UnityEngine;
using System.Collections;

public class godScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (!GetComponent<NetworkView> ().isMine) {       
			this.GetComponent<Camera> ().enabled = false; 
			this.transform.GetChild (0).gameObject.SetActive (false);
			//this.transform.GetChild(1).gameObject.SetActive(false);
		} else {
			this.GetComponent<OD_Communication> ().enabled = true; 
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
