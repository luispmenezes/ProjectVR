using UnityEngine;
using System.Collections;

public class BallMove : MonoBehaviour {

    //public GameObject other;
    public Rigidbody rb;
    public bool that = false;

	// Use this for initialization
	void Start () {
        goBall();
        //rb = other.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void goBall()
    {
        rb.AddForce(new Vector2(50.0f, 50.0f));
    }

    void OnCollisionEnter(Collision coll)
    {
        
        //Vector3 velY = rb.velocity;
        //velY.y = (velY.y / 2.0f);
        if (that)
        {
            rb.velocity = new Vector3(-10f, 0f, 0f);
            that = false;
        }
        else
        {
            rb.velocity = new Vector3(10f, 0f, 0f);
            that = true;
        }
        
        //Debug.Log(rb.velocity);

    }
}
