
using UnityEngine;

public interface IAINode {

		IAINode					Linker						{ get;      }
		IAINode					Parent						{ get; set; }
		float					gCost						{ get; set; }
		float					Heuristic					{ get; set; }
		bool					IsWalkable					{ get; set; }
		AINode[]				Neighbours					{ get; set; }
		Vector3					Position					{ get;      }

		void					OnNodeReached( Player player );
}

public interface IAINodeLinker {
	
}


[RequireComponent( typeof ( HighLighter ) ), System.Serializable]
public abstract class AINode : Interactable, IAINode {
	///
	/// PATHFINDING		START
	///
	[SerializeField]
	private		AINode						m_Linker	= null;
				IAINode						IAINode.Linker
	{
		get { return m_Linker as IAINode; }
	}

				IAINode						IAINode.Parent
	{
		get; set;
	}
	
				float						IAINode.gCost
	{
		get; set;
	}

				float						IAINode.Heuristic
	{
		get; set;
	}

				Vector3						IAINode.Position
	{
		get { return transform.position; }
	}
	[SerializeField]
	protected	bool						m_IsWalkable		= true;
				bool						IAINode.IsWalkable
	{
		get { return m_IsWalkable; }
		set { m_IsWalkable = value; IsInteractable = value; }
	}

	[SerializeField]
	private		AINode[]					m_Neighbours		= null;
				AINode[]					IAINode.Neighbours
	{
		get { return m_Neighbours; }
		set { m_Neighbours = value; }
	}

	///
	/// PATHFINDING		END
	///

	/////////////////////////////////////////////////////////////////////////////////

	public abstract	void	OnNodeReached( Player player );


	private void OnDrawGizmos()
	{
		if ( m_Linker == null )
			return;

		Gizmos.DrawLine( transform.position, m_Linker.transform.position );
	}

}
