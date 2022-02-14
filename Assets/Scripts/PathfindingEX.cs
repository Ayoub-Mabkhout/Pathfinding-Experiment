using System;
using System.Collections;
using System.Collections.Generic;
using BenTools.Data;
using UnityEngine;

public class PathfindingEX : MonoBehaviour{
    
    public Transform seeker, target;
    Node startNode;
    Node targetNode;
	Grid grid;
	int count;


    void Awake(){
        grid = GetComponent<Grid> ();
        
    }
    
    void Update(){
        grid.method = 3;
        startNode = grid.NodeFromWorldPoint(seeker.position);
	    targetNode = grid.NodeFromWorldPoint(target.position);
        grid.discoveredSet = new List<Node>();
        grid.expandedSet = new List<Node>();
        ResetNodes();
        grid.discoveredSet.Add(startNode);
        int start;
        int time;
        switch(grid.method){
            case 0:
                count = 0;
                DFS();
                Debug.Log("Expanded " + count + " nodes using DFS.\nPath cost is "+targetNode.gCost);
                break;
            case 1:
                count = 0;
                BFS();
                Debug.Log("Expanded " + count + " nodes using BFS.\nPath cost is "+targetNode.gCost);
                break;
            case 2:
                count = 0;
                UCS();
                Debug.Log("Expanded " + count + " nodes using UCS.\nPath cost is "+targetNode.gCost);
                break;
            case 3:
                count = 0;
                MyAStar();
                Debug.Log("Expanded " + count + " nodes using A*.\nPath cost is "+targetNode.gCost);
                break;
            case 4:
                count = 0;
                start = DateTime.Now.Millisecond;
                DFS();
                time = DateTime.Now.Millisecond - start;
                Debug.Log("Expanded " + count + " nodes using DFS.\nPath cost is "+targetNode.gCost+
                ". Time elapsed: "+ time + "ms.");
                ResetNodes();
                count = 0;
                start = DateTime.Now.Millisecond;
                BFS();
                time = DateTime.Now.Millisecond - start;
                Debug.Log("Expanded " + count + " nodes using BFS.\nPath cost is "+targetNode.gCost+
                ". Time elapsed:" + time + "ms.");
                ResetNodes();
                count = 0;
                start = DateTime.Now.Millisecond;
                UCS();
                time = DateTime.Now.Millisecond - start;
                Debug.Log("Expanded " + count + " nodes using UCS.\nPath cost is "+targetNode.gCost+
                ". Time elapsed: " + time + "ms.");
                ResetNodes();
                count = 0;
                start = DateTime.Now.Millisecond;
                MyAStar();
                time = DateTime.Now.Millisecond - start;
                Debug.Log("Expanded " + count + " nodes using A*.\nPath cost is "+targetNode.gCost+
                ". Time elapsed: " + time + "ms.");
                break;
            default:
                Debug.Log("Invalid method");
                break;
        }

    }

    private void ResetNodes(){
        foreach (Node n in grid.discoveredSet){
            n.gCost = 99999;
            n.hCost = 99999;
            //n.parent = null;

        }
            startNode.gCost = 0;
            startNode.hCost = GetDistance(startNode,targetNode);
            grid.discoveredSet.Clear();
            grid.expandedSet.Clear();
    }

    private List<Node> ExpandNode(Node node, bool _hCost){
        // if (grid.expandedSet.Contains(node)) return new List<Node>();
        grid.expandedSet.Add(node);
        count++;
        List<Node> temp = grid.GetNeighbours2(node);
        List<Node> neighbours = new List<Node>();
        foreach(Node n in temp){
            if (n.walkable)
                neighbours.Add(n);
        }
        foreach(Node neighbour in neighbours){
            int newCostToNeighbour = (int) node.gCost + GetDistance(neighbour,node);
            /*
            Debug.Log("Current node is: ("+node.gridX+","+node.gridY+")\n"+
             "current cost is "+node.gCost+" and the cost to node "+
                "("+neighbour.gridX+","+neighbour.gridY+") is "+newCostToNeighbour+"");
            */
            if (!grid.expandedSet.Contains(neighbour) || (grid.expandedSet.Contains(neighbour) && newCostToNeighbour < neighbour.gCost)){
                neighbour.gCost = newCostToNeighbour;
                neighbour.hCost = (_hCost)? GetDistance(neighbour,targetNode):0;
                neighbour.parent = node;
            }

            if(!grid.discoveredSet.Contains(neighbour)) grid.discoveredSet.Add(neighbour);
        }


        return neighbours;
    }


    private int GetDistance(Node nodeA, Node nodeB) {
        // return 1;
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}


    void RetracePath(Node startNode, Node endNode, ref List<Node> coloredPath) {
		grid.path = new List<Node>();
        /*
        switch(method){
            case 0:
                coloredPath = grid.DFSPath;
                break;
            case 1:
                coloredPath = grid.BFSPath;
                break;
            case 2:
                coloredPath = grid.UCSPath;
                break;
            case 3:
                coloredPath = grid.UCSPath;
                break;
            default:
                Debug.Log("Invalid method in RetracePath");
        }
        */

        coloredPath = new List<Node>();
		Node currentNode = endNode;
        int i = 0;

		while (currentNode != startNode && i < 500) {
        /*
            Debug.Log("Current node is: ("+currentNode.gridX+","+currentNode.gridY+")\n"+
                "and parent node is ("+currentNode.parent.gridX+","+currentNode.parent.gridY+")");
        */
			grid.path.Add(currentNode);
            coloredPath.Add(currentNode);
			currentNode = currentNode.parent;
            i++;
		}
		grid.path.Reverse();

	}


    // DFS implementation
    public void DFS(){
        Stack fringe = new Stack();
        fringe.Push(startNode);
        Node currentNode;
       do{
            currentNode = (Node) fringe.Pop();
            List<Node> neighbours = ExpandNode(currentNode,false);

            foreach(Node neighbour in neighbours){
                if(!neighbour.walkable || grid.expandedSet.Contains(neighbour)) continue;
                if(neighbour == targetNode){
                    RetracePath(startNode,targetNode,ref grid.DFSPath);
                    return;
                }

                fringe.Push(neighbour);
            }
        } while(fringe.Count != 0);
        
    }


    // BFS implementation
    public void BFS(){
        Queue<Node> fringe = new Queue<Node>();
        fringe.Enqueue(startNode);
        Node currentNode;
       do{
            currentNode = fringe.Dequeue();

            if (grid.expandedSet.Contains(currentNode)) continue;

            List<Node> neighbours = ExpandNode(currentNode,false);

            foreach(Node neighbour in neighbours){
                if(!neighbour.walkable || grid.expandedSet.Contains(neighbour)) continue;
                if(neighbour == targetNode){
                    RetracePath(startNode,targetNode,ref grid.BFSPath);
                    return;
                }

                fringe.Enqueue(neighbour);
            }
        } while(fringe.Count != 0);
    }

    // UCS implementation
    public void UCS(){
        BinaryPriorityQueue fringe = new BinaryPriorityQueue();
        int fringeSize = 1;
        fringe.Push(startNode);
        Node currentNode;
       do{
            currentNode = (Node) fringe.Pop();
            fringeSize --;
            // Debug.Log("Dequeing node ("+currentNode.gridX+","+currentNode.gridY+") with cost "+currentNode.gCost);


            if (grid.expandedSet.Contains(currentNode)) continue;

            List<Node> neighbours = ExpandNode(currentNode,false);

            foreach(Node neighbour in neighbours){
                if(!neighbour.walkable || grid.expandedSet.Contains(neighbour)) continue;
                if(neighbour == targetNode){
                    RetracePath(startNode,targetNode,ref grid.UCSPath);
                    return;
                }

                fringe.Push(neighbour);
                fringeSize ++;
                // Debug.Log("Enqueing node ("+neighbour.gridX+","+neighbour.gridY+") with cost " + neighbour.gCost);
            }
        } while(fringeSize != 0);

    }


    // A* implementation with fringe/priority queue
    public void MyAStar(){
        BinaryPriorityQueue fringe = new BinaryPriorityQueue();
        int fringeSize = 1;
        fringe.Push(startNode);
        Node currentNode;
       do{
            currentNode = (Node) fringe.Pop();
            fringeSize --;
            // Debug.Log("Dequeing node ("+currentNode.gridX+","+currentNode.gridY+") with cost "+currentNode.gCost);

            if (grid.expandedSet.Contains(currentNode)) continue;

            List<Node> neighbours = ExpandNode(currentNode,true);

            foreach(Node neighbour in neighbours){
                if(!neighbour.walkable || grid.expandedSet.Contains(neighbour)) continue;
                if(neighbour == targetNode){
                    RetracePath(startNode,targetNode,ref grid.AStarPath);
                    return;
                }

                fringe.Push(neighbour);
                fringeSize ++;
                // Debug.Log("Enqueing node ("+neighbour.gridX+","+neighbour.gridY+") with cost " + neighbour.gCost);
            }
        } while(fringeSize != 0);
    }
}
