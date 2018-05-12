
using UnityEngine;
using AI.Pathfinding;
using System;

public interface IAINode : IHeapItem<IAINode> {

		uint					ID							{ get;		}
		IAINode					Linker						{ get;      }
		IAINode					Parent						{ get; set; }
		float					gCost						{ get; set; }
		float					fCost						{ get;		}
		float					Heuristic					{ get; set; }
		bool					IsWalkable					{ get; set; }
		AINode[]				Neighbours					{ get; set; }
		Vector3					Position					{ get;      }

		void					OnNodeReached( Player player );

		bool					Visited						{ get; set; }
}

public interface IAINodeLinker {
	
}


[RequireComponent( typeof ( HighLighter ) ), System.Serializable]
public abstract class AINode : Interactable, IAINode {

	private	static	uint					ID	= 0;

	///
	/// PATHFINDING		START
	///
	[SerializeField]
	private		AINode						m_Linker	= null;

				uint						IAINode.ID			{	get { return m_ID ; } }
				IAINode						IAINode.Linker		{	get { return m_Linker as IAINode; } }
				IAINode						IAINode.Parent		{	get; set; }
				float						IAINode.gCost		{	get; set; }
				float						IAINode.Heuristic	{	get; set; }
				float						IAINode.fCost		{	get { IAINode node = ( this as IAINode ); { return node.gCost + node.Heuristic; } }	}
				Vector3						IAINode.Position	{	get { return transform.position; } }
				bool						IAINode.Visited		{	get; set; }

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

	private		int							m_HeapIndex;
				int							IHeapItem<IAINode>.HeapIndex
	{
		get { return m_HeapIndex; }
		set { m_HeapIndex = value; }
	}

	private		uint						m_ID = 0;



	private void Awake()
	{
		m_ID = ID;
		ID ++;
	}


	int IComparable<IAINode>.CompareTo( IAINode other )
	{
		IAINode node = ( this as IAINode );
		int compare =  node.fCost.CompareTo( other.fCost );
		if (compare == 0)
		{
			compare = node.Heuristic.CompareTo( other.Heuristic );
		}
		return -compare;
	}


	bool IEquatable<IAINode>.Equals( IAINode other )
	{
		return m_ID == other.ID;
	}


	public abstract	void	OnNodeReached( Player player );


	private void OnDrawGizmos()
	{
		if ( m_Linker == null )
			return;

		Gizmos.DrawLine( transform.position, m_Linker.transform.position );
	}
}
