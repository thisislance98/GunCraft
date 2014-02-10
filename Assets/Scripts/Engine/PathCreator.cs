using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathNode
{
	public PathNode Parent;
	public float TotalCost;
	public float PathDistance;
	public Vector3 Pos;
	
	public PathNode(Vector3 position, PathNode parent, Vector3 target) 
	{
		Pos = position;
		Parent = parent;
		
		if (Parent != null)
			PathDistance = Parent.PathDistance + Vector3.Distance(Parent.Pos,Pos);
		
//		TotalCost = PathDistance + Vector3.Distance(Pos,target);
		TotalCost = Vector3.Distance(Pos,target);
	}
	
}

public class PathNodeComparer : IComparer<PathNode>
{
	public int Compare(PathNode node1, PathNode node2)
	{
	      if (node1.TotalCost > node2.TotalCost)
	         return 1;
	      if (node1.TotalCost < node2.TotalCost)
	         return -1;
	      else
	         return 0;	
	}
	
}
public class PathCreator {
	

	SortedList<PathNode,Vector3> openNodes = new SortedList<PathNode,Vector3>(new PathNodeComparer());
	
	Dictionary<PathNode,Vector3> closedNodes = new Dictionary<PathNode, Vector3>();
	
	Vector3 currentTarget;
	TerrainBrain terrainBrain;
	bool climbingAllowed;
	
	public void Clear()
	{
		openNodes.Clear();
		closedNodes.Clear();
	}
	
	public List<Vector3> CreatePath(Vector3 start, Vector3 target, bool allowClimbing)
	{
		terrainBrain = TerrainBrain.Instance();
		climbingAllowed = allowClimbing;
		start = new Vector3(Mathf.Floor(start.x)+.5f,Mathf.Floor(start.y)+.5f,Mathf.Floor(start.z)+.5f);
		currentTarget = target;
		
		PathNode startNode = new PathNode(start,null,target);
		openNodes.Add(startNode,start);
		
		List<Vector3> finalPath = null;
		
		int rounds = 100;
		while (openNodes.Count > 0 && finalPath == null && rounds >= 0)
		{
			
			PathNode currentNode = openNodes.Keys[0];
			
			ExpandBestNode(currentNode);
	
			// get the path if we found it		
			if (Vector3.Distance(currentNode.Pos,target) <= 1 || rounds == 0 || openNodes.Count == 0)
			{
				if (openNodes.Count == 0)
					Debug.Log("no open nodes on round: " + rounds);
				
				finalPath = ExtractPath(currentNode);
			}

			rounds--;
		}
		
		return finalPath;
		
	}
	

	
	void ExpandBestNode(PathNode currentNode)
	{
		Vector3 currentNodePos = currentNode.Pos;
		
		// first remove the current node from the open list and put it in the closed list
		openNodes.Remove(currentNode);
		closedNodes.Add(currentNode,currentNode.Pos);
		
		
		// look around current position and add open nodes when possible
		
		
		for (float x = currentNodePos.x - 1; x <= currentNodePos.x + 1; x++)
			for (float y = currentNodePos.y - 1; y <= currentNodePos.y + 1; y++)
				for (float z = currentNodePos.z - 1; z <= currentNodePos.z + 1; z++)
			{
				Vector3 pos = new Vector3(x,y,z);
				if (currentNodePos == pos)
					continue;
				
				AddOpenNodeAtPos(currentNode, pos);
				
			}
//		AddOpenNodeAtPos(currentNode, Vector3.up);
//		AddOpenNodeAtPos(currentNode, Vector3.down);
//		AddOpenNodeAtPos(currentNode, Vector3.left);
//		AddOpenNodeAtPos(currentNode, Vector3.right);
//		AddOpenNodeAtPos(currentNode, Vector3.forward);
//		AddOpenNodeAtPos(currentNode, Vector3.back);
		
	}
	
	void AddOpenNodeAtPos( PathNode currentNode, Vector3 pos)
	{
		Vector3 currentNodePos = currentNode.Pos;
		
		// only consider nodes that are not in the closed list and are not solid blocks
		if (IsValidNodePos(currentNode,pos))
		{
			
			// if the current pos is not in the open list then we add it
			if (!openNodes.Values.Contains(pos))
			{
				
				PathNode newNode = new PathNode(pos,currentNode,currentTarget);
				if (!openNodes.ContainsKey(newNode))
					openNodes.Add(newNode,pos);
			}
			else
			{
			

			}				

		}		
		
	}
	
	bool IsValidNodePos(PathNode currentNode, Vector3 pos)
	{
		
		if (closedNodes.ContainsValue(pos) || terrainBrain.getTerrainDensity(pos) != 0)
			return false;
		
		// don't allow diagnals
		if (pos.x != currentNode.Pos.x && pos.z != currentNode.Pos.z)
			return false;
		
		// return false if they are trying to go between two diaganol blocks
		if (pos.y != currentNode.Pos.y && 
			terrainBrain.getTerrainDensity(new Vector3(pos.x,currentNode.Pos.y,pos.z)) != 0 &&
			terrainBrain.getTerrainDensity(new Vector3(currentNode.Pos.x,pos.y,currentNode.Pos.z)) != 0)
			return false;
		
		// return false if climbing is not allowed and they are in the air
		if (!climbingAllowed && terrainBrain.getTerrainDensity(pos + Vector3.down) == 0)
			return false;
		
		// return false if the node is in the air above a flat surface so that they don't fly
		if (terrainBrain.getTerrainDensity(pos + Vector3.down) == 0 &&
			terrainBrain.getTerrainDensity(pos + Vector3.down + Vector3.right) == 0 &&
			terrainBrain.getTerrainDensity(pos + Vector3.down + Vector3.left) == 0 &&
			terrainBrain.getTerrainDensity(pos + Vector3.down + Vector3.forward) == 0 &&
			terrainBrain.getTerrainDensity(pos + Vector3.down + Vector3.back) == 0)
			return false;
		
					 		
		return true;
	}
	
		// returns the final path from the last node
	List<Vector3> ExtractPath(PathNode finalNode)
	{
		List<Vector3> path = new List<Vector3>();
		
		PathNode current = finalNode;
		
		do 
		{
			path.Add(current.Pos);
			
			if (current.Parent == null)
				return path;
			
			current = current.Parent;
		}
		while (current.Parent != null);
		
		path.Reverse();
		return path;
	}
		
}
	


