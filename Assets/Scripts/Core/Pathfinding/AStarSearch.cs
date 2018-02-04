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
		private	InteractableAINode	GetBestNode( InteractableAINode[] set, bool useHeuristic )
		{
			IAINode bestNode = null;
			float bestTotal = float.MaxValue;

			foreach( IAINode n in set )
			{
				float totalCost = useHeuristic ? n.Cost + n.Heuristic : n.Cost;
				if ( totalCost < bestTotal )
				{
					bestTotal = totalCost;
					bestNode = n;
				}
			}
			return bestNode as InteractableAINode;
		}


		//////////////////////////////////////////////////////////////////////////
		// FindPath
		public InteractableAINode[]	FindPath( InteractableAINode startNode, InteractableAINode endNode )
		{
			List<InteractableAINode> openSet	= new List<InteractableAINode>();
			List<InteractableAINode> closedSet	= new List<InteractableAINode>();

			( startNode as IAINode ).Cost = 0;
			( startNode as IAINode ).Heuristic = ( startNode.transform.position - endNode.transform.position ).sqrMagnitude;

			openSet.Add( startNode );

			while ( openSet.Count > 0 )
			{
				InteractableAINode n = GetBestNode( openSet.ToArray(), true );
				openSet.Remove( n );
				closedSet.Add( n );

				if ((InteractableAINode)n == endNode)
				{
				//	Debug.Log("We found the end node!");
					break;
				}
//				yield return null;

				foreach( IAINode neigh in n.Neighbours )
				{
					InteractableAINode interactable = neigh as InteractableAINode;
					if ( !closedSet.Contains( interactable ) && !openSet.Contains( interactable ) )
					{
						neigh.Cost = n.Cost + ( interactable.transform.position - n.transform.position ).sqrMagnitude;
						neigh.Heuristic = ( interactable.transform.position - endNode.transform.position ).sqrMagnitude;
						openSet.Add( interactable );
					}
				}
			}

			List<InteractableAINode> bestPathList = new List<InteractableAINode>();

			// Find best path
			bestPathList.Add( endNode );
			InteractableAINode currentNode = endNode;
			while ( currentNode != startNode )
			{
				// Get the neighbours of the current node
				InteractableAINode[] neighbours = currentNode.Neighbours;

				// Find the best neighbour
				IAINode bestNeigh = GetBestNode( neighbours, false );
				if ( bestNeigh == null )
					return null;

				bestPathList.Add( currentNode = ( bestNeigh as InteractableAINode ) );
			}

			GraphMaker.Instance.ResetCosts();

			bestPathList.Reverse();
			return bestPathList.ToArray();
		}

	}

}