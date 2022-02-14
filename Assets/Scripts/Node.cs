using System;
using UnityEngine;
using System.Collections;

public class Node : IComparable {
	
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public Node parent;
	
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}

	public int CompareTo(object obj){
		if (obj == null) return 1;
		Node other = obj as Node;
		if (this.fCost < other.fCost) return -1;
		else if (this.fCost > other.fCost) return 1;
		else return 0;

	}

	public int fCost {
		get {
			return (int)gCost + (int)hCost;
		}
	}
}
