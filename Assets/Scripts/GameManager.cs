using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private bool gameOverFlag;

	public bool gameInitiated;

	private bool selfRestart, opponentRestart;

	public ScoreUI scoreUI;

	public NetworkManager netManager;

	public GameObject ball;

	private int playersConnected;

	private float timePassed;

	private bool connectedAct;

	void Start () {
		selfRestart = opponentRestart = false;
		gameOverFlag = false;
		gameInitiated = false;
		playersConnected = 0;
		connectedAct = false;
	}
	
	void Update () {
		if(selfRestart && opponentRestart){
			restartGameAll();
		}

		if(connectedAct && timePassed <3f){
			timePassed += Time.deltaTime;
		}
		if(connectedAct && timePassed >3f){
			connectedAct = false;
			spawnNow();
		}
	}

	void OnConnectedToServer(){
		Debug.Log("Connected to Server");
		 connectedAct = true;
		 timePassed = 0f;
	}

	void spawnNow(){

		int nPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
		if(nPlayers == 0){
			netManager.SpawnPlayer(true);
		}
		else{
			netManager.SpawnPlayer(false);
		}
		if(netManager.iAmPlayer)
		{
			gameObject.GetComponent<NetworkView>().RPC("readyToStart", RPCMode.OthersBuffered, null);
			gameInitiated = true;
		}
	}

	void OnServerInitialized()
	{	
		netManager.SpawnPlayer(true);
	}


	void OnGUI(){
		if(netManager.iAmPlayer)
		{
			GUIStyle mystyle = new GUIStyle();
			Font myFont = (Font)Resources.Load("Fonts/LCD", typeof(Font));
			mystyle.font = myFont;
			mystyle.fontSize =  60;
			mystyle.fontStyle= FontStyle.Bold;
			mystyle.onNormal.textColor = Color.yellow;
			mystyle.onActive.textColor = Color.blue;
			mystyle.onFocused.textColor = Color.red;

			if(gameOverFlag)
			{
				/* restart button */
				float restart_x1 = Screen.width/2 -150f;
				float restart_x2 = 375f;
				float restart_y1 = Screen.height/2 - 50f;
				float restart_y2 = 65f;
				GUI.Box(new Rect(restart_x1, restart_y1, restart_x2, restart_y2), "");
				if(GUI.Button(new Rect(restart_x1+10f, restart_y1, restart_x2, restart_y2), "Restart game", mystyle))
				{
					
					selfRestart = true;
					gameObject.GetComponent<NetworkView>().RPC("restartRequestRPC", RPCMode.OthersBuffered, null);
				}
				/* exit button */
				float exit_x1 = 100f;
				float exit_x2 = 130f;
				float exit_y1 = Screen.height - 100f;
				float exit_y2 = 65f;
				GUI.Box(new Rect(exit_x1, exit_y1, exit_x2, exit_y2), "");
				if(GUI.Button(new Rect(exit_x1+10f, exit_y1, exit_x2, exit_y2), "Exit", mystyle))
				{
					Application.Quit();
				}
			
				if(selfRestart && !opponentRestart){
					waitingForPlayer();
				}
			}
			if(!gameInitiated){
				waitingForPlayer();
			}
		}
	}

	private void waitingForPlayer(){
		GUIStyle mystyle = new GUIStyle();
		Font myFont = (Font)Resources.Load("Fonts/LCD", typeof(Font));
		mystyle.font = myFont;
		mystyle.fontSize =  20;
		mystyle.fontStyle= FontStyle.Bold;
		mystyle.normal.textColor = Color.black;
		/*waiting for other player message */
		float lab_x1 = Screen.width/2 -100f;
		float lab_x2 = 375f;
		float lab_y1 = Screen.height/2 +15f;
		float lab_y2 = 65f;
		GUI.Label(new Rect(lab_x1, lab_y1, lab_x2, lab_y2), "Waiting for the other player...", mystyle);
	}

	private void restartGameAll(){
		gameObject.GetComponent<NetworkView>().RPC("restartGameRPC", RPCMode.OthersBuffered, null);
		restartGameLocal();
		
	}

	private void restartGameLocal(){
		selfRestart = opponentRestart = gameOverFlag = false;
		scoreUI.restartGame();
		if(Network.isServer)
		{
			startGame();
		}
	}

	public void gameOver(){
		gameOverFlag = true;
	}

	public void startGame(){
		Network.Instantiate(ball, new Vector3(0,3, 0), Quaternion.Euler(0,0,0), 0);
	}


	[RPC] void restartRequestRPC()
	{
		opponentRestart = true;
	}

	[RPC] void restartGameRPC()
	{
		restartGameLocal();
	}

	[RPC] void readyToStart()
	{
		if(Network.isServer){
			if(netManager.iAmPlayer){
				startGame();
			}
			else{
				playersConnected++;
				if(playersConnected == 2){
					startGame();	
				}
			}
		}
		gameInitiated = true;
	}
}
