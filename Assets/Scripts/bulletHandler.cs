using UnityEngine;
using System.Collections;

public class bulletHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject[] invis = GameObject.FindGameObjectsWithTag("invis");
		foreach(GameObject obj in invis)
			Physics.IgnoreCollision(gameObject.GetComponent<SphereCollider>(), obj.GetComponent<BoxCollider>());
		Object.Destroy (gameObject, 1);
	}
}
