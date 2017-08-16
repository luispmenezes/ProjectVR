using UnityEngine;
using System.Collections;
using System;

public class Ball : MonoBehaviour {

	public int x,y,z;
	private int scoreRed, scoreBlue;
	public GUIStyle style;
	private int r = 1;
	string Rnd = "Round: ";
	float time=2;

	void Start () 
	{
		Force ();

	}
	void Update(){
		if (Input.GetKey (KeyCode.R)) {
			Reset ();
		}
		if (GetComponent<Rigidbody> ().position.x > 11.75 || GetComponent<Rigidbody> ().position.x < -11.75 || GetComponent<Rigidbody> ().position.y > 8)
			Reset ();
			
		time -= Time.deltaTime;

	}

	void OnGUI()
	{
		GUIStyle mystyle = new GUIStyle ();
		Font myFont = (Font)Resources.Load ("Fonts/LCD", typeof(Font));
		mystyle.font = myFont;
		mystyle.fontSize = 60;
		mystyle.fontStyle = FontStyle.Bold;
		mystyle.onNormal.textColor = Color.yellow;
		mystyle.onActive.textColor = Color.blue;
		mystyle.onFocused.textColor = Color.red;

		//GUI.Box(new Rect(85,100,450,100),"");
		if (time > 0)
			GUI.Button (new Rect (95,100,200,40), Rnd + r, mystyle);
		else
			GUI.Button (new Rect (95,100,200,40), "", mystyle);
	}

	void OnCollisionEnter(Collision col)
	{
		Debug.Log ("Round: "+ r);
		if (col.collider.gameObject.tag == "parede_blue") {
			scoreBlue++;
			Debug.Log ("Round BLUE: "+ r);
			r++;
			Reset();
			time=2;
		}
		if (col.collider.gameObject.tag == "parede_red") {
			scoreRed++;
			Debug.Log ("Round RED: "+ r);
			r++;
			Reset();
			time=2;
		}
		//SO ESTA PARA OBSERVER! MUDAR
		try{
			if (col.collider.gameObject.tag == "Player") {

				if(GetComponent<Rigidbody>().position.z>0){
					GetComponent<Rigidbody> ().AddForce (new Vector3 (0, -1, -30), ForceMode.Impulse);
				}
				else{
					GetComponent<Rigidbody> ().AddForce (new Vector3 (0, -1, 30), ForceMode.Impulse);
				}
			}
		}catch(Exception e){}

		//collisions with leap motion hands
		try{
			if(col.collider.gameObject.transform.parent.parent.gameObject != null 
				&& col.collider.gameObject.transform.parent.parent.gameObject.ToString() == "RigidHand(Clone) (UnityEngine.GameObject)"){
				if(GetComponent<Rigidbody> ().velocity.magnitude > 25.0f)
				{
					GetComponent<Rigidbody> ().velocity *= 0.5f;
				}
			}
		}catch(Exception e){}
	}

	void Reset(){
		GetComponent<Rigidbody> ().transform.position = new Vector3 (0, 3, 0);
		GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
		Force ();
	}

	void Force(){
		int i = UnityEngine.Random.Range (-1, 1);
		if (i >= 0) {
			GetComponent<Rigidbody> ().AddForce (new Vector3 (x, y, z), ForceMode.Impulse);
		} else {
			GetComponent<Rigidbody> ().AddForce (new Vector3 (x, y, -z), ForceMode.Impulse);
		}
	}

	public int GetScoreBlue(){
		return scoreBlue;
	}
	public int GetScoreRed(){
		return scoreRed;
	}
}
