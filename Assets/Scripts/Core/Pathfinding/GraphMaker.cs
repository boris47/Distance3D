
using UnityEngine;

namespace AI.Pathfinding
{

	public class GraphMaker : MonoBehaviour
	{

		public	static	GraphMaker		Instance			= null;

		// DEBUG
		[SerializeField][Header("Debug Only")]
		private bool					m_OnlyNodesCount	= false;

		[SerializeField]
		private	bool					m_ProcedurallyGen	= false;

		[ SerializeField ][Range(1, 20)]
		private	int						m_UpdateCount		= 1;

		[ SerializeField ][Range( 0.1f, 10f )]
		private	float					m_ScanRadius		= 1.1f;

		private	AINode[]				m_Nodes				= null;
		public	int						NodeCount
		{
			get
			{
				if ( m_Nodes != null )
					return m_Nodes.Length;
				return 0;
			}
		}

		private	bool					m_IsGraphReady		= false;
		public	bool					IsGraphReady
		{
			get { return m_IsGraphReady; }
		}



		//////////////////////////////////////////////////////////////////////////
		// AWAKE
		private	void	Awake ()
		{
			Instance = this;

			// Find all nodes
			m_Nodes = FindObjectsOfType<AINode>();

			Debug.Log( "GraphMaker::Info:Nodes: " + m_Nodes.Length );

			if ( m_OnlyNodesCount )
				return;

			if ( m_ProcedurallyGen )
				print( "GraphMaker::Awake: Generating Node Graph" );

			foreach ( IAINode node in m_Nodes )
			{
				node.Neighbours = null;

				if ( m_ProcedurallyGen == false )
					UpdateNeighbours( node, false );
			}

			if ( m_ProcedurallyGen == false )
				m_IsGraphReady = true;
		}
		

		//////////////////////////////////////////////////////////////////////////
		// START ( Coroutine )
		private System.Collections.IEnumerator Start()
		{
			if ( m_OnlyNodesCount )
				yield break;

			if ( m_ProcedurallyGen == false )
			{
				print( "GraphMaker::Start: Node Graph ready" );
				yield break;
			}

			int internalCounter = 0;
			for ( int i = 0; i-m_UpdateCount < m_Nodes.Length; i+= m_UpdateCount )
			{	
				while( internalCounter < m_UpdateCount )
				{
					if ( i + internalCounter >= m_Nodes.Length )
					{
						print( "GraphMaker::Start: Node Graph ready" );
						m_IsGraphReady = true;
						yield break;
					}

					IAINode node1 = m_Nodes[ i + internalCounter ];
					UpdateNeighbours( node1, false );
					internalCounter ++;
				}
				internalCounter = 0;
				yield return null;
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
				m_Nodes, 
				n => ( n.transform.position - iNode.Position ).sqrMagnitude <= m_ScanRadius * m_ScanRadius &&
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

			foreach ( IAINode node in m_Nodes )
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
			foreach ( IAINode node in m_Nodes )
			{
				node.Heuristic	= 0f;
				node.gCost		= float.MaxValue;
				node.Parent		= null;
			}
		}

	}

}