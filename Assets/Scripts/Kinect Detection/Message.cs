using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Message : MonoBehaviour {

	public int idx;
	public int nVertex;
	public List<List<int>> vertices;
	
	public Message(int idx, int nVertex) {
		this.idx = idx;
		this.nVertex = nVertex;
		vertices =  new List<List<int>>();
	}
	
	public void addVertex(int x, int y, int z){
		List<int> vertex = new List<int>();
		vertex.Add(x);
		vertex.Add(y);
		vertex.Add(z);
		vertices.Add( vertex );
	}
}
