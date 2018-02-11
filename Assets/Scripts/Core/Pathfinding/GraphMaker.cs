
using UnityEngine;

namespace AI.Pathfinding
{

	public class GraphMaker : MonoBehaviour
	{

		public	static	GraphMaker		Instance			= null;

		private	AINode[]				Nodes				= null;
		public	int						NodeCount
		{
			get
			{
				if ( Nodes != null )
					return Nodes.Length;
				return 0;
			}
		}

		private	float					scanRadius			= 1.1f;



		//////////////////////////////////////////////////////////////////////////
		// AWAKE
		private	void	Awake ()
		{
			Instance = this;

			// Find all nodes
			Nodes = FindObjectsOfType<AINode>();

			Debug.Log( "Nodes: " + Nodes.Length );

			// neighbours setup
			foreach ( IAINode node in Nodes )
			{
				UpdateNeighbours( node, false );
			}
		}


		//////////////////////////////////////////////////////////////////////////
		// UpdaeNeighbours
		public	void	UpdateNeighbours( IAINode iNode, bool isUpdate )
		{

			if (  iNode is IAINodeLinker )
				return;

			// UPDATE PREVIOUS NEIGHBOURS
			if ( isUpdate == true )
			{
				// update previous neighbours
				foreach( IAINode neigh in iNode.Neighbours )
				{
					UpdateNeighbours( neigh, false );
				}
			}

			// Get neighbours by distance
			IAINode[] neighbours = System.Array.FindAll
			( 
				Nodes, 
				n => ( n.transform.position - iNode.Position ).sqrMagnitude <= scanRadius * scanRadius &&
				(AINode)n != (AINode)iNode
			);

			// create temporary array of neighbours and copy neighbours found
			bool hasLinker = iNode.Linker != null;
			AINode[] nodeNeighbours = new AINode[ neighbours.Length + ( hasLinker ? 1 : 0 ) ];
			System.Array.Copy( neighbours, nodeNeighbours, neighbours.Length );


			// LINKER ASSIGNMENT
			if ( hasLinker )
			{
				// add linker to this node
				nodeNeighbours[ nodeNeighbours.Length - 1 ] = iNode.Linker as AINode;
				
				IAINode			ILinker		= iNode.Linker;

				// resize Neighbours array
				var tmpNeighbours = ILinker.Neighbours;
				System.Array.Resize( ref tmpNeighbours, ( ILinker.Neighbours != null ) ? ILinker.Neighbours.Length + 1 : 1 );
				// add this node to linker
				tmpNeighbours[ tmpNeighbours.Length - 1 ] = iNode as AINode;
				ILinker.Neighbours = tmpNeighbours;
			}
		
			iNode.Neighbours = nodeNeighbours;
			

			// UPDATE CURRENT NEIGHBOURS
			if ( isUpdate == true )
			{
				// update previous neighbours
				foreach( IAINode neigh in iNode.Neighbours )
				{
					UpdateNeighbours( neigh, false );
				}
			}
		}


		//////////////////////////////////////////////////////////////////////////
		// GetNearestNode
		public	IAINode	GetNearestNode( Vector3 position )
		{
			float currentDistance = float.MaxValue;
			IAINode result = null;

			foreach ( IAINode node in Nodes )
			{
				float distance = ( node.Position - position ).sqrMagnitude;
				if ( distance < currentDistance * currentDistance )
				{
					currentDistance = distance;
					result = node;
				}
			}
			return result;
		}


		//////////////////////////////////////////////////////////////////////////
		// ResetCosts
		internal	void	ResetNodes()
		{
			foreach ( IAINode node in Nodes )
			{
				node.gCost	= float.MaxValue;
				node.Parent = null;
			}
		}

	}

}