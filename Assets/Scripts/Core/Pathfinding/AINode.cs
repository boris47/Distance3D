
using UnityEngine;

namespace AI.Pathfinding {

	internal interface IAINode {

		AINode					Parent			{ get; set; }
		float					Cost			{ get; set; }
		float					Heuristic		{ get; set; }
		AINode[]				Neighbours		{ get; set; }
		bool					IsWalkable		{ get; set; }
	}

}

[RequireComponent( typeof ( HighLighter ) )]
public abstract class AINode : Interactable, AI.Pathfinding.IAINode {
	///
	/// PATHFINDING		START
	///
	private		float						m_Cost				= float.MaxValue;
				float						AI.Pathfinding.IAINode.Cost
	{
		get { return m_Cost; }
		set { m_Cost = value; }
	}

	private		float						m_Heuristic			= 0f;
				float						AI.Pathfinding.IAINode.Heuristic
	{
		get { return m_Heuristic; }
		set { m_Heuristic = value; }
	}

	protected	bool						m_IsWalkable		= true;
	public		bool						IsWalkable
	{
		get { return m_IsWalkable; }
		set { m_IsWalkable = value; IsInteractable = value; }
	}
	[SerializeField]
	private		AINode[]					m_Neighbours		= null;
	public		AINode[]					Neighbours
	{
		get { return m_Neighbours; }
	}
				AINode[]					AI.Pathfinding.IAINode.Neighbours
	{
		get { return m_Neighbours; }
		set { m_Neighbours = value; }
	}

	private		AINode						m_parent			= null;
	AINode AI.Pathfinding.IAINode.Parent {
		get { return m_parent; }
		set { m_parent = value; }
	}
	///
	/// PATHFINDING		END
	///

	/////////////////////////////////////////////////////////////////////////////////

	public abstract	void	OnNodeReached( Player player );

}
