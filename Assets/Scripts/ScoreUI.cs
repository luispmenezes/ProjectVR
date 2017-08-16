using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreUI : MonoBehaviour {

	private int scoreRed;
	private int scoreBlue;
	public GUIStyle style;
	public int winningScore;
	private bool fim = false;
	private bool gameOverCalled = false;
	public GameManager gameManager;
	
	Dictionary<int, Transform> segments, segments2;


	void OnGUI()
	{
		GUIStyle mystyle = new GUIStyle();
		Font myFont = (Font)Resources.Load("Fonts/LCD", typeof(Font));
		mystyle.font = myFont;
		mystyle.fontSize =  60;
		mystyle.fontStyle= FontStyle.Bold;
		mystyle.onNormal.textColor = Color.yellow;
		mystyle.onActive.textColor = Color.blue;
		mystyle.onFocused.textColor = Color.red;

		if (!fim) {
			GameObject go = GameObject.Find ("Ball(Clone)");
			if(go != null)
			{
				Ball other = (Ball)go.GetComponent (typeof(Ball));
				scoreRed = other.GetScoreRed ();
				scoreBlue = other.GetScoreBlue ();
			}
			else
			{
				scoreRed = scoreBlue = 0;
			}
		}

		float x = Screen.width/2f;
		float y = 30f;
		float width = 400f;
		float height = 50f;
		SetNumber(scoreRed, scoreBlue);

		if (scoreRed >= winningScore || scoreBlue >= winningScore)
		{
			GameObject ball = GameObject.Find("Ball(Clone)");
			if (ball != null)
			{
				ball.SetActive(false);
				fim=true;
			}


			string winMessage = "Player Left won";
			if (scoreBlue >= winningScore)
			{
				winMessage = "Player Right won";
			}

			//GUI.Box(new Rect(85,100,450,100),"");
			GUI.Button(new Rect(95,100,200,40),winMessage, mystyle);
		}


		if(fim && !gameOverCalled)
		{
			gameManager.gameOver();
			gameOverCalled = true;
		}

	}

	public void restartGame(){
		scoreBlue = scoreRed = 0;
		fim = gameOverCalled = false;
	}
	
	void Awake()
	{
		segments = new Dictionary<int, Transform>();
		segments[1] = GameObject.Find ("seg1").GetComponent<Transform> ();
		segments[2] = GameObject.Find ("seg2").GetComponent<Transform> ();
		segments[3] = GameObject.Find ("seg3").GetComponent<Transform> ();
		segments[4] = GameObject.Find ("seg4").GetComponent<Transform> ();
		segments[5] = GameObject.Find ("seg5").GetComponent<Transform> ();
		segments[6] = GameObject.Find ("seg6").GetComponent<Transform> ();
		segments[7] = GameObject.Find ("seg7").GetComponent<Transform> ();

		segments2 = new Dictionary<int, Transform>();
		segments2[1] = GameObject.Find ("_seg1").GetComponent<Transform> ();
		segments2[2] = GameObject.Find ("_seg2").GetComponent<Transform> ();
		segments2[3] = GameObject.Find ("_seg3").GetComponent<Transform> ();
		segments2[4] = GameObject.Find ("_seg4").GetComponent<Transform> ();
		segments2[5] = GameObject.Find ("_seg5").GetComponent<Transform> ();
		segments2[6] = GameObject.Find ("_seg6").GetComponent<Transform> ();
		segments2[7] = GameObject.Find ("_seg7").GetComponent<Transform> ();
		
	}
	
	public void SetNumber(int n, int m)
	{
		switch (n){
			case 0:
				segments[1].gameObject.SetActive(true);
				segments[2].gameObject.SetActive(true);
				segments[3].gameObject.SetActive(true);
				segments[4].gameObject.SetActive(true);
				segments[5].gameObject.SetActive(true);
				segments[6].gameObject.SetActive(true);
				segments[7].gameObject.SetActive(false);
				break;
			case 1:
				segments[1].gameObject.SetActive(false);
				segments[2].gameObject.SetActive(false);
				segments[3].gameObject.SetActive(true);
				segments[4].gameObject.SetActive(true);
				segments[5].gameObject.SetActive(false);
				segments[6].gameObject.SetActive(false);
				segments[7].gameObject.SetActive(false);
				break;
			case 2:
				segments[1].gameObject.SetActive(true);
				segments[2].gameObject.SetActive(true);
				segments[3].gameObject.SetActive(false);
				segments[4].gameObject.SetActive(true);
				segments[5].gameObject.SetActive(true);
				segments[6].gameObject.SetActive(false);
				segments[7].gameObject.SetActive(true);
				break;
			case 3:
				segments[1].gameObject.SetActive(true);
				segments[2].gameObject.SetActive(true);
				segments[3].gameObject.SetActive(false);
				segments[4].gameObject.SetActive(false);
				segments[5].gameObject.SetActive(true);
				segments[6].gameObject.SetActive(true);
				segments[7].gameObject.SetActive(true);
				break;
			case 4:
				segments[1].gameObject.SetActive(true);
				segments[2].gameObject.SetActive(false);
				segments[3].gameObject.SetActive(true);
				segments[4].gameObject.SetActive(false);
				segments[5].gameObject.SetActive(false);
				segments[6].gameObject.SetActive(true);
				segments[7].gameObject.SetActive(true);
				break;
			case 5:
				segments[1].gameObject.SetActive(false);
				segments[2].gameObject.SetActive(true);
				segments[3].gameObject.SetActive(true);
				segments[4].gameObject.SetActive(false);
				segments[5].gameObject.SetActive(true);
				segments[6].gameObject.SetActive(true);
				segments[7].gameObject.SetActive(true);
				break;
			case 6:
				segments[1].gameObject.SetActive(false);
				segments[2].gameObject.SetActive(true);
				segments[3].gameObject.SetActive(true);
				segments[4].gameObject.SetActive(true);
				segments[5].gameObject.SetActive(true);
				segments[6].gameObject.SetActive(true);
				segments[7].gameObject.SetActive(true);
				break;
			case 7:
				segments[1].gameObject.SetActive(true);
				segments[2].gameObject.SetActive(true);
				segments[3].gameObject.SetActive(false);
				segments[4].gameObject.SetActive(false);
				segments[5].gameObject.SetActive(false);
				segments[6].gameObject.SetActive(true);
				segments[7].gameObject.SetActive(false);
				break;
			case 8:
				segments[1].gameObject.SetActive(true);
				segments[2].gameObject.SetActive(true);
				segments[3].gameObject.SetActive(true);
				segments[4].gameObject.SetActive(true);
				segments[5].gameObject.SetActive(true);
				segments[6].gameObject.SetActive(true);
				segments[7].gameObject.SetActive(true);
				break;
			case 9:
				segments[1].gameObject.SetActive(true);
				segments[2].gameObject.SetActive(true);
				segments[3].gameObject.SetActive(true);
				segments[4].gameObject.SetActive(false);
				segments[5].gameObject.SetActive(true);
				segments[6].gameObject.SetActive(true);
				segments[7].gameObject.SetActive(true);
				break;
			}

		switch (m){
		case 0:
			segments2[1].gameObject.SetActive(true);
			segments2[2].gameObject.SetActive(true);
			segments2[3].gameObject.SetActive(true);
			segments2[4].gameObject.SetActive(true);
			segments2[5].gameObject.SetActive(true);
			segments2[6].gameObject.SetActive(true);
			segments2[7].gameObject.SetActive(false);
			break;
		case 1:
			segments2[1].gameObject.SetActive(false);
			segments2[2].gameObject.SetActive(false);
			segments2[3].gameObject.SetActive(true);
			segments2[4].gameObject.SetActive(true);
			segments2[5].gameObject.SetActive(false);
			segments2[6].gameObject.SetActive(false);
			segments2[7].gameObject.SetActive(false);
			break;
		case 2:
			segments2[1].gameObject.SetActive(true);
			segments2[2].gameObject.SetActive(true);
			segments2[3].gameObject.SetActive(false);
			segments2[4].gameObject.SetActive(true);
			segments2[5].gameObject.SetActive(true);
			segments2[6].gameObject.SetActive(false);
			segments2[7].gameObject.SetActive(true);
			break;
		case 3:
			segments2[1].gameObject.SetActive(true);
			segments2[2].gameObject.SetActive(true);
			segments2[3].gameObject.SetActive(false);
			segments2[4].gameObject.SetActive(false);
			segments2[5].gameObject.SetActive(true);
			segments2[6].gameObject.SetActive(true);
			segments2[7].gameObject.SetActive(true);
			break;
		case 4:
			segments2[1].gameObject.SetActive(true);
			segments2[2].gameObject.SetActive(false);
			segments2[3].gameObject.SetActive(true);
			segments2[4].gameObject.SetActive(false);
			segments2[5].gameObject.SetActive(false);
			segments2[6].gameObject.SetActive(true);
			segments2[7].gameObject.SetActive(true);
			break;
		case 5:
			segments2[1].gameObject.SetActive(false);
			segments2[2].gameObject.SetActive(true);
			segments2[3].gameObject.SetActive(true);
			segments2[4].gameObject.SetActive(false);
			segments2[5].gameObject.SetActive(true);
			segments2[6].gameObject.SetActive(true);
			segments2[7].gameObject.SetActive(true);
			break;
		case 6:
			segments2[1].gameObject.SetActive(false);
			segments2[2].gameObject.SetActive(true);
			segments2[3].gameObject.SetActive(true);
			segments2[4].gameObject.SetActive(true);
			segments2[5].gameObject.SetActive(true);
			segments2[6].gameObject.SetActive(true);
			segments2[7].gameObject.SetActive(true);
			break;
		case 7:
			segments2[1].gameObject.SetActive(true);
			segments2[2].gameObject.SetActive(true);
			segments2[3].gameObject.SetActive(false);
			segments2[4].gameObject.SetActive(false);
			segments2[5].gameObject.SetActive(false);
			segments2[6].gameObject.SetActive(true);
			segments2[7].gameObject.SetActive(false);
			break;
		case 8:
			segments2[1].gameObject.SetActive(true);
			segments2[2].gameObject.SetActive(true);
			segments2[3].gameObject.SetActive(true);
			segments2[4].gameObject.SetActive(true);
			segments2[5].gameObject.SetActive(true);
			segments2[6].gameObject.SetActive(true);
			segments2[7].gameObject.SetActive(true);
			break;
		case 9:
			segments2[1].gameObject.SetActive(true);
			segments2[2].gameObject.SetActive(true);
			segments2[3].gameObject.SetActive(true);
			segments2[4].gameObject.SetActive(false);
			segments2[5].gameObject.SetActive(true);
			segments2[6].gameObject.SetActive(true);
			segments2[7].gameObject.SetActive(true);
			break;
		}
	}
}
