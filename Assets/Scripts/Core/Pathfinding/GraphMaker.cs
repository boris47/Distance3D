
using UnityEngine;

namespace AI.Pathfinding
{

	interface IGrapMakerEditor {

		float			ScanRadius	{ get; }
		AINode[]		Nodes		{ get; set; }

	}

	public class GraphMaker : MonoBehaviour, IGrapMakerEditor
	{

		public	static	GraphMaker		Instance			= null;

		[ SerializeField ][Range( 0.1f, 10f )]
		private	float					m_ScanRadius		= 1.1f;
		float							IGrapMakerEditor.ScanRadius
		{
			get { return m_ScanRadius; }
		}


		private static	AINode[]		m_Nodes				= null;
		AINode[]						IGrapMakerEditor.Nodes
		{
			get { return m_Nodes; }
			set { m_Nodes = value; }
		}


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
		}
		

		//////////////////////////////////////////////////////////////////////////
		// UpdaeNeighbours
		public	void	UpdateNeighbours( IAINode iNode, bool isUpdate )
		{
			if (  iNode is IAINodeLinker )
				return;

			if ( m_Nodes == null )
				m_Nodes = FindObjectsOfType<AINode>();

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
				node.Visited	= false;
			}
		}

	}

}