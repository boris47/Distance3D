
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
		void Awake ()
		{
			Instance = this;

			// Find all nodes
			Nodes = FindObjectsOfType<AINode>();

			Debug.Log( "Nodes: " + Nodes.Length );

			// neighbours setup
			foreach ( AINode node in Nodes )
			{
				UpdateNeighbours( node, false );
			}
		}


		//////////////////////////////////////////////////////////////////////////
		// UpdaeNeighbours
		public	void	UpdateNeighbours( AINode node, bool isUpdate )
		{
			if ( isUpdate == true )
			{
				// update previous neighbours
				foreach( IAINode neigh in ( node as IAINode ).Neighbours )
				{
					UpdateNeighbours( neigh as AINode, false );
				}
			}

			// get current neighbours
			AINode interactable = node as AINode;
			( node as IAINode ).Neighbours = System.Array.FindAll
			( 
				Nodes, 
				n => ( n.transform.position - interactable.transform.position ).sqrMagnitude <= scanRadius * scanRadius &&
				n != (AINode)node
			);

			if ( isUpdate == true )
			{
				// update previous neighbours
				foreach( IAINode neigh in ( node as IAINode ).Neighbours )
				{
					UpdateNeighbours( neigh as AINode, false );
				}
			}
		}


		//////////////////////////////////////////////////////////////////////////
		// GetNearestNode
		public	AINode	GetNearestNode( Vector3 position )
		{
			float currentDistance = float.MaxValue;
			AINode result = null;

			foreach ( AINode node in Nodes )
			{
				float distance = ( node.transform.position - position ).sqrMagnitude;
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
				node.Cost	= float.MaxValue;
				node.Parent = null;
			}
		}

	}

}