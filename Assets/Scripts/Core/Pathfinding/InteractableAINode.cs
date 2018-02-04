using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Pathfinding {

	internal interface IAINode {

		float					Cost			{ get; set; }
		float					Heuristic		{ get; set; }
		InteractableAINode[]	Neighbours		{ get; set; }
	}

}

[RequireComponent( typeof ( HighLighter ) )]
public abstract class InteractableAINode : Interactable, AI.Pathfinding.IAINode {
	///
	/// PATHFINDING		START
	///
	private	float					m_Cost			= float.MaxValue;
	public	float					Cost
	{
		get { return m_Cost; }
	}
	float	AI.Pathfinding.IAINode.Cost
	{
		get { return m_Cost; }
		set { m_Cost = value; }
	}

	private	float					m_Heuristic		= 0f;
	float	AI.Pathfinding.IAINode.Heuristic
	{
		get { return m_Heuristic; }
		set { m_Heuristic = value; }
	}


	private	InteractableAINode[]	m_Neighbours		= null;
	public	InteractableAINode[]	Neighbours
	{
		get { return m_Neighbours; }
	}
	InteractableAINode[]	AI.Pathfinding.IAINode.Neighbours
	{
		get { return m_Neighbours; }
		set { m_Neighbours = value; }
	}

	///
	/// PATHFINDING		END
	///

}
