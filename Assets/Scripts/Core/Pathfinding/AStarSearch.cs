using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;

namespace AI.Pathfinding
{

	public class AStarSearch : MonoBehaviour
	{
//		Stopwatch sw = new Stopwatch();

		public	static	AStarSearch Instance			= null;

		Heap<IAINode>				m_OpenSet			= null;

		private	int					m_PathNodeCount		= 0;

		//////////////////////////////////////////////////////////////////////////
		// AWAKE
		private	void	Awake()
		{
			Instance = this;
			m_OpenSet = new Heap<IAINode>( GraphMaker.Instance.NodeCount );
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
		private	int	RetracePath( IAINode startNode, IAINode endNode, ref IAINode[] path )
		{
			int currentNodeCount = 0;
			IAINode currentNode = endNode;

			while ( currentNode.Equals( startNode ) == false )
			{
				path[ currentNodeCount ] = currentNode;
				currentNode = currentNode.Parent;
				currentNodeCount ++;
			}
			path[ currentNodeCount ] = currentNode;
			currentNodeCount ++;
//			sw.Stop();
//			print( "Node count: " + currentNodeCount + ", path found in " + sw.ElapsedMilliseconds + "ms" );

			GraphMaker.Instance.ResetNodes();
			m_OpenSet.Reset();
			return currentNodeCount;
		}


		//////////////////////////////////////////////////////////////////////////
		// FindPath
		public int	FindPath( Vector3 startPosition, Vector3 endPosition, ref IAINode[] path )
		{
			IAINode startNode	= GraphMaker.Instance.GetNearestNode( startPosition );
			IAINode endNode		= GraphMaker.Instance.GetNearestNode( endPosition );
			return FindPath( startNode, endNode, ref path );
		}


		//////////////////////////////////////////////////////////////////////////
		// FindPath
		public int	FindPath( IAINode startNode, IAINode endNode, ref IAINode[] path )
		{
			if ( GraphMaker.Instance.NodeCount == 0 )
			{
				print( "AStarSearch::FindPath:Node graph has to be build !!" );
				return 0;
			}

			if ( endNode.IsWalkable == false )
				return 0;

			endNode.gCost = 0;
			endNode.Heuristic = ( endNode.Position - startNode.Position ).sqrMagnitude;

			// First node is always discovered
			m_OpenSet.Add( endNode );

//			sw.Reset();
//			sw.Start();

			// Start scan
			while ( m_OpenSet.Count > 0 )
			{
				IAINode currentNode = m_OpenSet.RemoveFirst();
				if ( currentNode.ID == startNode.ID )
				{
				//	Debug.Log("We found the end node!");
					return RetracePath( endNode, startNode, ref path );
				}

//				if ( currentNode == null )	return null;

				currentNode.Visited = true;

				// Setup its neighbours
				for ( int i = 0; i < currentNode.Neighbours.Length; i++ )
				{
					IAINode iNeighbour = currentNode.Neighbours[ i ];
					if ( iNeighbour == null )
					{
						print( "node " + ( currentNode as AINode ).name + " has neighbour as null " );
						return 0;
					}

					// Ignore the neighbour which is already evaluated.
					if ( iNeighbour.IsWalkable == false ||  iNeighbour.Visited == true )
						continue;


					float gCost = currentNode.gCost + ( currentNode.Position - iNeighbour.Position ).sqrMagnitude;
					bool containsNehigbour = m_OpenSet.Contains( iNeighbour );
					if ( gCost < iNeighbour.gCost || containsNehigbour == false )
					{
						iNeighbour.gCost		= gCost;
						iNeighbour.Heuristic	= ( iNeighbour.Position - startNode.Position ).sqrMagnitude;
						iNeighbour.Parent		= currentNode;

						if ( containsNehigbour == false )
						{
							m_OpenSet.Add( iNeighbour );
							m_PathNodeCount ++;
						}
					}

				}
			}

			// no path found
			return 0;
		}

	}

}