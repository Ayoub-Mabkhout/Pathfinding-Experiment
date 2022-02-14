﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Awake() {
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		CreateGrid();
	}

	void CreateGrid() {
		grid = new Node[gridSizeX,gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
				grid[x,y] = new Node(walkable,worldPoint, x,y);
			}
		}
	}


	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}
		return neighbours;
	}

	public List<Node> GetNeighbours2(Node node){
		List<Node> neighbours = new List<Node>();
		// Debug.Log("For node  ("+node.gridX+","+node.gridY+")");
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if ((x == 0 && y == 0) || (x !=0 && y != 0))
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					// Debug.Log("Adding neighbour  ("+checkX+","+checkY+")");
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}
		return neighbours;
	}
	

	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}

	public List<Node> path;
	public List<Node> DFSPath;
	public List<Node> BFSPath;
	public List<Node> UCSPath;
	public List<Node> AStarPath;
	public List<Node> expandedSet;
	public List<Node> discoveredSet;
	public int method;
	
	void OnDrawGizmos() {

		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));

		if (grid != null) {
			if(method == 4){
				foreach (Node n in grid){
					Gizmos.color = (n.walkable)?Color.white:Color.red;
					if(DFSPath != null)
						if(DFSPath.Contains(n)) 
							Gizmos.color = Color.gray;
					if(BFSPath != null)
						if(BFSPath.Contains(n))
							Gizmos.color = Color.magenta;
					if(UCSPath != null)
						if(UCSPath.Contains(n)) 
							Gizmos.color = Color.yellow;
					if(AStarPath != null)
						if(AStarPath.Contains(n)) 
							Gizmos.color = Color.blue;

					Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
				}

			}
			else{
				foreach (Node n in grid) {
					Gizmos.color = (n.walkable)?Color.white:Color.red;
					if (discoveredSet != null)
						if (discoveredSet.Contains(n))
							Gizmos.color = Color.magenta;
					if (expandedSet != null)
						if (expandedSet.Contains(n))
							Gizmos.color = Color.cyan;
					if (path != null)
						if (path.Contains(n))
							Gizmos.color = Color.black;
					Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
				}
			}
		}
	}
}