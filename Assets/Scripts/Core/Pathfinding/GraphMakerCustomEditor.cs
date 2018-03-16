using UnityEngine;
using UnityEditor;

namespace AI.Pathfinding {

	[CustomEditor(typeof(GraphMaker))]
	public class GraphMakerCustomEditor : Editor {

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			IGrapMakerEditor graphInterface = ( IGrapMakerEditor )target;
			GraphMaker graphMaker = ( GraphMaker ) target;

			if ( GUILayout.Button( "Node Count" ) )
			{
				Debug.Log( "Node Count: " + FindObjectsOfType<AINode>().Length );
			}

			if ( GUILayout.Button( "Build" ) )
			{
				graphInterface.Nodes = FindObjectsOfType<AINode>();
				foreach( IAINode node in graphInterface.Nodes )
				{
					UpdateNeighbours( node, false );
					EditorUtility.SetDirty( node as AINode );
				}
				Debug.Log( "Build done, node count: " + graphInterface.Nodes.Length );
				EditorUtility.SetDirty( graphMaker );
			}
		}

		//////////////////////////////////////////////////////////////////////////
		// UpdaeNeighbours
		private	void	UpdateNeighbours( IAINode iNode, bool isUpdate )
		{
			IGrapMakerEditor graphInterface = ( IGrapMakerEditor )target;
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
				graphInterface.Nodes, 
				n => ( n.transform.position - iNode.Position ).sqrMagnitude <= graphInterface.ScanRadius * graphInterface.ScanRadius &&
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

	}

}