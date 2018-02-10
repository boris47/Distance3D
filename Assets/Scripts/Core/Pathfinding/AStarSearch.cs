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
		private	AINode	GetBestNode( IEnumerable set, bool useHeuristic )
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
		private	AINode[]	RetracePath( AINode startNode, AINode endNode )
		{
			List<AINode> path = new List<AINode>();
			AINode currentNode = endNode;
			while ( currentNode != startNode )
			{
				path.Add( currentNode );
				currentNode = ( currentNode as IAINode ).Parent;
			}

			path.Reverse();
			GraphMaker.Instance.ResetNodes();
			return path.ToArray();
		}



		//////////////////////////////////////////////////////////////////////////
		// FindPath
		public AINode[]	FindPath( AINode startNode, AINode endNode )
		{
			HashSet<AINode> closedSet	= new HashSet<AINode>();
			List<AINode>	openSet		= new List<AINode>();

			if ( startNode.IsWalkable == false )
				return null;

			( startNode as IAINode ).Cost = 0;
			( startNode as IAINode ).Heuristic = ( startNode.transform.position - endNode.transform.position ).sqrMagnitude;
			openSet.Add( startNode );

			// Start scan
			while ( openSet.Count > 0 )
			{
				AINode currentNode = GetBestNode( openSet, true );

				if ( currentNode == endNode )
				{
				//	Debug.Log("We found the end node!");
					return RetracePath( startNode, endNode );
				}

				if ( currentNode == null )
					return null;


				// First node is always discovered
				closedSet.Add( currentNode );
				openSet.Remove( currentNode );

				// Setup its neighbours
				foreach( AINode neighbour in currentNode.Neighbours )
				{
					// Ignore the neighbor which is already evaluated.
					if ( neighbour.IsWalkable == false || closedSet.Contains( neighbour ) )
						continue;

					IAINode INeigh = ( neighbour as IAINode );

					float gCost = ( currentNode as IAINode ).Cost + ( currentNode.transform.position - neighbour.transform.position ).sqrMagnitude;
					if ( gCost < INeigh.Cost || openSet.Contains(neighbour) == false )
					{
						INeigh.Cost		= gCost;
						INeigh.Heuristic	= ( neighbour.transform.position - endNode.transform.position ).sqrMagnitude;
						INeigh.Parent		= currentNode;

						if ( openSet.Contains( neighbour ) == false )
							openSet.Add( neighbour );
					}

				}
			}

			// no path found
			return null;
		}

	}

}