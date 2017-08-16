using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObstacleManager : MonoBehaviour {
	
	public static ObstacleManager Instance;
	
	public GameObject obsPrefab;
	public NetworkView netView;
	//DIMENSOES DO PLANO DE JOGO
	public float xmin=0, xmax=0, zmin=0, zmax=0;
	private float renderScale =1.0f;
	private float offsetX=0,offsetY=0;
	private int kXmin=0, kXmax=512, kYmin=0, kYmax=424;
	//CONVERSAO DE ALTURA 1M real equivale a x unidades de jogo
	public float heightConv=2;
	private static List<GameObject> obstacles = new List<GameObject>();
	private List<List<Vector3>> verticies;
	private static int obstacleCount = 0; 
	
	public void Awake(){
		Instance = this;
	}
	
	public void parseMenssagePoly(string menssage){
		verticies = new List<List<Vector3>> ();
		var lines = menssage.Split(new [] { '\r', '\n' },StringSplitOptions.RemoveEmptyEntries);
		//destroyAll();
		
		foreach (string s in lines)
		{
			if(s.Substring(0,1)=="$"){
				Debug.Log(s);
				int idx     = Int32.Parse(s.Substring(1, 2));
				float heigth  = heightConv * (((float)Int32.Parse(s.Substring(6, 4)))/1000);
				int nVertex = Int32.Parse(s.Substring(13, 2));
				
				List<Vector3> temp= new List<Vector3>();
				Vector2[] vertices2D = new Vector2[nVertex];
				
				for(int i =0; i < nVertex; i++){
					//MENEZES DO FUTURO JA SABES -> STRING TOO BIG 
					if((21+(i*9)) < s.Length){
						float vertexX = projectX(512-Int32.Parse(s.Substring(18+(i*9), 3)));
						float vertexZ = projectZ(Int32.Parse(s.Substring(22+(i*9), 3)));
						temp.Add(new Vector3(vertexX, 0 , vertexZ));
						temp.Add(new Vector3(vertexX, heigth , vertexZ));
						vertices2D[i] = new Vector2(vertexX,vertexZ);
					}
				}
				
				Mesh m = create2DMesh(vertices2D);
				for(int i=0; i < m.vertices.Length; i++){
					m.vertices[i].z = m.vertices[i].y;
					m.vertices[i].y = heigth;
				}
				GameObject obj = (GameObject) Instantiate(obsPrefab, new Vector3(0,0.1f,0), Quaternion.Euler(new Vector3(90,180,180)));
				obstacleCount++;
				obj.GetComponent<MeshFilter>().mesh = m;
				obstacles.Add(obj);
				
				verticies.Add(temp);
				//createMesh(temp);
			}
		}
	}
	
	public void parseMenssage(string menssage){
		var lines = menssage.Split(new [] { '\r', '\n' },StringSplitOptions.RemoveEmptyEntries);
		NetworkViewID viewID = Network.AllocateViewID ();
		netView.RPC ("destroyAll", RPCMode.AllBuffered, viewID);
		
		foreach (string s in lines)
		{
			if(s.Substring(0,1)=="$"){
				Debug.Log("Parsing:"+s);
				int idx = Int32.Parse(s.Substring(1, 2));
				float centerZ = projectX(Int32.Parse(s.Substring(13, 3))) + offsetX;
				float centerX = projectZ(Int32.Parse(s.Substring(17, 3))) + offsetY;
				float angle   = Int32.Parse(s.Substring(28, 4));
				float scaleZ  = ((xmax-xmin)/512) * Int32.Parse(s.Substring(39, 3))*renderScale;
				float scaleX  = ((zmax-zmin)/424) * Int32.Parse(s.Substring(43, 3))*renderScale;
				float scaleY  = heightConv * (((float)Int32.Parse (s.Substring(55,3)))/1000);
				
				//Debug.Log("I "+idx+" CX "+centerX+" CY "+centerZ+" A "+angle+" SX "+scaleX+" SY "+scaleY+ " SZ "+scaleZ);
				/*ObstacleManager[] oms = FindObjectsOfType(typeof(ObstacleManager)) as ObstacleManager[];
				foreach (ObstacleManager o in oms) {
					o.updateObs(idx,centerX, centerZ,angle,scaleX,scaleY,scaleZ);
				}
				*/
				netView.RPC ("updateObs", RPCMode.AllBuffered, viewID, idx,centerX, centerZ,angle,scaleX,scaleY,scaleZ);
				//updateObs(idx,centerX, centerZ,angle,scaleX,scaleY,scaleZ);
			}else{
				//Debug.Log("Parsing:"+s);gqay
				renderScale = float.Parse(s.Substring(6,2));
				offsetY = Int32.Parse(s.Substring(30,4));
				offsetX = Int32.Parse(s.Substring(43,4));
				kYmin = Int32.Parse(s.Substring(50,3));
				kYmax = Int32.Parse(s.Substring(54,3));
				kXmin = Int32.Parse(s.Substring(61,3));
				kXmax = Int32.Parse(s.Substring(65,3));
				
				Debug.Log("\n->SC: "+renderScale+"OffX: "+ offsetX+ "OffY: "+offsetY+"\n");
			}
		}
		
		//destroyUnused (lines.Length);
		
	}
	
	[RPC]
	private void updateObs(NetworkViewID viewID,int idx, float cX, float cZ, float a, float sX, float sY, float sZ){
		Debug.Log ("PING  "+this.ToString());
		if (idx >= obstacleCount) {
			//INSTANTIATE
			
			GameObject temp = ((GameObject) Instantiate(obsPrefab, new Vector3(cX, sY/2 ,cZ), Quaternion.Euler(new Vector3(0,-a,0))));
			temp.transform.localScale = new Vector3(sX,sY,sZ);
			obstacles.Add(temp);
			obstacleCount++;
		} else {
			//UPDATE
			//obstacles.transform.position = new Vector3(cX, sY/2 ,cZ);
			//obstacles[idx].transform.rotation = Quaternion.Euler(new Vector3(0,-a,0));
			//obstacles[idx].transform.localScale = new Vector3(sX,sY,sZ);
		}
	}
	
	private void destroyUnused(int length){
		if (obstacleCount > length) {
			for(int i = length; i < obstacleCount; i++){
				Destroy(obstacles[i]);
				obstacles[i]=null;
			}
		}
		obstacleCount = length;
	}
	
	[RPC]
	public void destroyAll(NetworkViewID viewID){
		foreach(GameObject go in obstacles){
			Destroy(go);
		}
		obstacles = new List<GameObject> ();
		obstacleCount = 0;
	}
	
	
	private float projectX(float x){
		float sx = (xmax-xmin)/(kXmax-kXmin);
		float tx = ((kXmax*xmin)-(kXmin*xmax))/(kXmax-kXmin);
		
		return (sx*x)+tx;
	}
	
	private float projectZ(float z){
		float sz = (zmax-zmin)/(kYmax-kYmin);
		float tz = ((kYmax*zmin)-(kYmin*zmax))/(kYmax-kYmin);
		
		return (z * sz) + tz;
	}
	
	private void createMesh(List<Vector3> verticies){
		
	} 
	
	private Mesh create2DMesh(Vector2[] vertices2D){
		Triangulator tr = new Triangulator(vertices2D);
		int[] indices = tr.Triangulate();
		
		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[vertices2D.Length];
		for (int i=0; i<vertices.Length; i++) {
			vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
		}
		Mesh msh = new Mesh();
		msh.vertices = vertices;
		msh.triangles = indices;
		msh.RecalculateNormals();
		msh.RecalculateBounds();
		
		return msh;
	}
	
	
}
