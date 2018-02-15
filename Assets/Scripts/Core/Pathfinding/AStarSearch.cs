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
		private	IAINode	GetBestNode( IEnumerable set, bool useHeuristic )
		{
			IAINode bestNode = null;
			float bestTotal = float.MaxValue;

			foreach( IAINode n in set )
			{
				if ( n.IsWalkable == false )
					continue;

				float totalCost = useHeuristic ? n.gCost + n.Heuristic : n.gCost;
				if ( totalCost < bestTotal )
				{
					bestTotal = totalCost;
					bestNode = n;
				}
			}
			return bestNode;
		}


		//////////////////////////////////////////////////////////////////////////
		// FindPath
		private	IAINode[]	RetracePath( IAINode startNode, IAINode endNode )
		{
			List<IAINode> path = new List<IAINode>();
			IAINode currentNode = endNode;
			while ( currentNode != startNode )
			{
				path.Add( currentNode );
				currentNode = currentNode.Parent;
			}

			path.Reverse();
			GraphMaker.Instance.ResetNodes();
			return path.ToArray();
		}


		//////////////////////////////////////////////////////////////////////////
		// FindPath
		public IAINode[]	FindPath( Vector3 startPosition, Vector3 endPosition )
		{
			IAINode startNode	= GraphMaker.Instance.GetNearestNode( startPosition );
			IAINode endNode		= GraphMaker.Instance.GetNearestNode( endPosition );
			return FindPath( startNode, endNode );
		}


		//////////////////////////////////////////////////////////////////////////
		// FindPath
		public IAINode[]	FindPath( IAINode startNode, IAINode endNode )
		{
			if ( GraphMaker.Instance.IsGraphReady == false )
			{
				print( "AStarSearch::FindPath:Node graph is building!!" );
				return null;
			}

			HashSet<IAINode>	closedSet	= new HashSet<IAINode>();
			List<IAINode>		openSet		= new List<IAINode>();

			if ( startNode.IsWalkable == false )
				return null;

			startNode.gCost = 0;
			startNode.Heuristic = ( startNode.Position - endNode.Position ).sqrMagnitude;
			openSet.Add( startNode );

			// Start scan
			while ( openSet.Count > 0 )
			{
				IAINode currentNode = GetBestNode( openSet, true );

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
				foreach( IAINode iNeighbour in currentNode.Neighbours )
				{
					if ( iNeighbour == null )
					{
						print( "node " + ( currentNode as AINode ).name + " has neighbour as null " );
						return null;
					}

					// Ignore the neighbor which is already evaluated.
					if ( iNeighbour.IsWalkable == false || closedSet.Contains( iNeighbour ) )
						continue;


					float gCost = currentNode.gCost + ( currentNode.Position - iNeighbour.Position ).sqrMagnitude;
					if ( gCost < iNeighbour.gCost || openSet.Contains(iNeighbour) == false )
					{
						iNeighbour.gCost		= gCost;
						iNeighbour.Heuristic	= ( iNeighbour.Position - endNode.Position ).sqrMagnitude;
						iNeighbour.Parent		= currentNode;

						if ( openSet.Contains( iNeighbour ) == false )
							openSet.Add( iNeighbour );
					}

				}
			}

			// no path found
			return null;
		}

	}

}