
using UnityEngine;
using System.Collections;
using System;

public class Player : Interactable {

	public	static	Player	CurrentPlayer				= null;

	public	float			m_Speed						= 2f;
	

	// INTERACTION
	private	bool			HasOverrideState
	{
		get; set;
	}



	// NAVIGATION
	private struct Navigation
	{
		public	bool					HasPath;
		public	AINode[]				Path;
		public	int						NodeIdx;
		public	System.Action			Action;
	}
	private	Navigation		m_Movement					= default ( Navigation );
	private	Vector3			m_MovementStartPosition		= Vector3.zero;
	private	float			m_MovementInterpolant		= 0f;


	//////////////////////////////////////////////////////////////////////////
	// START
	protected override void Start()
	{
		base.Start();
	}


	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( Override )
	public void OnInteraction()
	{
		if ( CameraControl.Instance.Target != null && CameraControl.Instance.Target != transform )
			CameraControl.Instance.Target = transform;
		// player selection
		CurrentPlayer = this;

	}


	//////////////////////////////////////////////////////////////////////////
	// MoveOnPlatform
	public	void	MoveOnPlatform( Platform platform )
	{
		HasOverrideState = true;
	}


	//////////////////////////////////////////////////////////////////////////
	// Move
	public	void	Move( Interactable interactable )
	{
		// start node
		AINode startNode = AI.Pathfinding.GraphMaker.Instance.GetNearestNode( transform.position );

		// final node
		AINode finalNode = AI.Pathfinding.GraphMaker.Instance.GetNearestNode( interactable.transform.position );

		// path finding
		AINode[] path	= AI.Pathfinding.AStarSearch.Instance.FindPath( startNode, finalNode );

		if ( path == null || path.Length < 1 )
		{
			m_Movement.HasPath = false;
			return;
		}

		m_MovementStartPosition = transform.position;
		m_MovementInterpolant = 0f;

		m_Movement = new Navigation();

		if ( interactable is Lever || interactable is Openable )
		{
			path[ path.Length - 1 ] = null;
		}

		m_Movement.Action = delegate { CheckForUsage( interactable ); };
		m_Movement.Path = path;
		m_Movement.NodeIdx = 0;
		m_Movement.HasPath = true;
	}


	//////////////////////////////////////////////////////////////////////////
	// CheckForUsage
	private	void	CheckForUsage( Interactable interactable )
	{
		if ( interactable is UsableObject )
			( interactable as UsableObject ).OnInteraction( this );
	}


	//////////////////////////////////////////////////////////////////////////
	// UNITY
	//////////////////////////////////////////////////////////////////////////

	//////////////////////////////////////////////////////////////////////////
	// Update
	private	void	Update()
	{

		if ( m_Movement.HasPath == false )
			return;

		if ( m_MovementInterpolant < 1f )
		{
			m_MovementInterpolant += Time.deltaTime * m_Speed;

			Vector3 finalposition = m_Movement.Path[ m_Movement.NodeIdx ].transform.position;
			Vector3 position = Vector3.Lerp( m_MovementStartPosition, finalposition, m_MovementInterpolant );
			position.y = m_MovementStartPosition.y;
			transform.position = position;
		}
		else // arrived at node
		{
			m_Movement.NodeIdx ++;

			// Arrived
			if ( m_Movement.NodeIdx == m_Movement.Path.Length || m_Movement.Path[ m_Movement.NodeIdx ] == null )
			{
				m_MovementInterpolant = 0f;
				m_Movement.HasPath = false;
				if ( m_Movement.Action != null )
					m_Movement.Action();
				return;
			}

			m_MovementStartPosition = transform.position;
			m_MovementInterpolant = 0f;
		}
	}

}
