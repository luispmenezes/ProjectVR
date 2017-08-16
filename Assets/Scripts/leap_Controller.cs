using UnityEngine;
using System.Collections;

public class leap_Controller : MonoBehaviour {

	public GameObject leap_prefab;


	// Use this for initialization
	void Start () {
		Debug.Log ("atemtpeing leap");
		Instantiate (leap_prefab, new Vector3 (0, 0, 0), Quaternion.identity);
		//leap_prefab.transform.parent = transform;
	}
	
	// Update is called once per frame
	void Update () {
		//ta aqui 1 erro
		leap_prefab.transform.position = transform.position;
		//GameObject obj = GameObject.Find ("leap_prefab(Clone");
		Debug.Log (transform.position);
	}
}
