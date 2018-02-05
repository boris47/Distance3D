using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Pathfinding
{

	public class AStarSearch : MonoBehaviour
	{
		public	static	AStarSearch Instance = null;

		

		//////////////////////////////////////////////////////////////////////////
		// AWAKE
		private	void	Awake()
		{
			Instance = this;
		}


		//////////////////////////////////////////////////////////////////////////
		// GetBestNode
		private	AINode	GetBestNode( AINode[] set, bool useHeuristic )
		{
			IAINode bestNode = null;
			float bestTotal = float.MaxValue;

			foreach( IAINode n in set )
			{
				if ( n.IsWalkable == false )
					continue;

				float totalCost = useHeuristic ? n.Cost + n.Heuristic : n.Cost;
				if ( totalCost < bestTotal )
				{
					bestTotal = totalCost;
					bestNode = n;
				}
			}
			return bestNode as AINode;
		}


		//////////////////////////////////////////////////////////////////////////
		// FindPath
		public AINode[]	FindPath( AINode startNode, AINode endNode )
		{
			List<AINode> openSet	= new List<AINode>();
			List<AINode> closedSet	= new List<AINode>();

			if ( startNode.IsWalkable == false )
				return null;

			( startNode as IAINode ).Cost = 0;
			( startNode as IAINode ).Heuristic = ( startNode.transform.position - endNode.transform.position ).sqrMagnitude;

			openSet.Add( startNode );

			while ( openSet.Count > 0 )
			{
				AINode n = GetBestNode( openSet.ToArray(), true );

				if ( n == null )
					return null;

				openSet.Remove( n );
				closedSet.Add( n );

				if ( n == endNode )
				{
				//	Debug.Log("We found the end node!");
					break;
				}


				foreach( IAINode neigh in n.Neighbours )
				{
					if ( neigh.IsWalkable == false )
						continue;

					AINode interactable = neigh as AINode;
					if ( !closedSet.Contains( interactable ) && !openSet.Contains( interactable ) )
					{
						neigh.Cost = n.Cost + ( interactable.transform.position - n.transform.position ).sqrMagnitude;
						neigh.Heuristic = ( interactable.transform.position - endNode.transform.position ).sqrMagnitude;
						openSet.Add( interactable );
					}
				}
			}


			List<AINode> bestPathList = new List<AINode>();

			// Find best path
			bestPathList.Add( endNode );
			AINode currentNode = endNode;
			while ( currentNode != startNode )
			{
				// Get the neighbours of the current node
				AINode[] neighbours = currentNode.Neighbours;

				// Find the best neighbour
				IAINode bestNeigh = GetBestNode( neighbours, false );
				if ( bestNeigh == null )
					return null;

				bestPathList.Add( currentNode = ( bestNeigh as AINode ) );
			}

			GraphMaker.Instance.ResetCosts();

			bestPathList.Reverse();
			return bestPathList.ToArray();
		}

	}

}