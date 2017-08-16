using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class OD_Communication : MonoBehaviour {

	private ObstacleManager obstacleManager;
	public String IP="127.0.0.1";
	public int port = 11000;
	private int dataFreq = 20;
	
	private Socket sender;
	private Boolean socketReady=false;
	private String msgContent="";
	private Boolean canDestroy;
	
	void Start(){
		while (!socketReady) {
			try {
				Debug.Log("Connecting...");
				IPAddress ipAddress = IPAddress.Parse (IP);
				IPEndPoint remoteEP = new IPEndPoint (ipAddress, port);
				sender = new Socket (AddressFamily.InterNetwork, 
			    	                       SocketType.Stream, ProtocolType.Tcp);
				sender.Connect (remoteEP);
				socketReady = true;
			} catch (Exception e) {
				Debug.Log ("Connection Failed!!!" + e);
				socketReady = false;
			}
		}
		canDestroy = false;
	}

	void Update(){
		String msg = readSocket ();
		//Debug.Log ("" + msg);
		if (msg != "") {
			//ObstacleManager.Instance.parseMenssagePoly (msg);
			ObstacleManager.Instance.parseMenssage (msg);
			canDestroy=true;
		} else if(canDestroy){
			NetworkViewID viewID = Network.AllocateViewID ();
			GetComponent<NetworkView> ().RPC ("destroyAll", RPCMode.AllBuffered, viewID);
			canDestroy=false;
		}
	}

	public String readSocket(){
		if (!socketReady)
			return "Socket not ready"; 
		byte[] buffer = new byte[1024];
		int bytesRead=0;
		try{
			sender.BeginReceive(buffer, bytesRead, 1024, SocketFlags.None, (asyncResult) =>
			{
				bytesRead = sender.EndReceive(asyncResult);
				saveContent(buffer,bytesRead);
				if(bytesRead == 0){
					return;
				}
			}, null);
		}catch(Exception e){
			Debug.Log("Failed to read from socket!!"+e);
		}

		return msgContent;
	}

	private void saveContent(byte[] buffer, int bufferSize){
		msgContent=Encoding.ASCII.GetString(buffer,0,bufferSize);
	}
		
}