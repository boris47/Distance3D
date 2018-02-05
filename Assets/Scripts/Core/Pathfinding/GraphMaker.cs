
using UnityEngine;

namespace AI.Pathfinding
{

	public class GraphMaker : MonoBehaviour
	{

		public	static	GraphMaker		Instance			= null;

		private	AINode[]				Nodes				= null;
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
			foreach ( IAINode node in Nodes )
			{
				AINode interactable = node as AINode;
				node.Neighbours = System.Array.FindAll
				( 
					Nodes, 
					n => ( n.transform.position - interactable.transform.position ).sqrMagnitude <= scanRadius * scanRadius &&
					n != (AINode)node
				);
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
		internal	void	ResetCosts()
		{
			foreach ( IAINode node in Nodes )
			{
				node.Cost = float.MaxValue;
			}
		}

	}

}