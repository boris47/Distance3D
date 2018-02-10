
using UnityEngine;
using System.Collections;
using System;

public class Player : Interactable {

	public	static	Player[]	Players						= null;

	public	static	Player		CurrentPlayer				= null;

	public	float				m_Speed						= 2f;

	
	// NAVIGATION
	[Serializable]
	private struct Navigation
	{
		public	bool					HasPath;
		public	AINode[]				Path;
		public	int						NodeIdx;
		public	Action					Action;
		public	Interactable			Interactable;
	}
	[ SerializeField ]
	private	Navigation			m_Movement					= default ( Navigation );
	private	Vector3				m_MovementStartPosition		= Vector3.zero;
	private	float				m_MovementInterpolant		= 0f;
	private	AINode				m_CurrentNode				= null;


	//////////////////////////////////////////////////////////////////////////
	// AWAKE
	private void Awake()
	{
		if ( Players == null )
			Players = FindObjectsOfType<Player>();
	}


	//////////////////////////////////////////////////////////////////////////
	// START
	protected override void Start()
	{
		base.Start();

		m_CurrentNode = AI.Pathfinding.GraphMaker.Instance.GetNearestNode( transform.position );

		float height = transform.position.y;
		transform.position = new Vector3( m_CurrentNode.transform.position.x, height, m_CurrentNode.transform.position.z );
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
	// UpdateNavigation
	public static void	UpdateNavigation()
	{
		foreach( Player player in Players )
		{
			if ( player.m_Movement.HasPath == false )
				continue;

			player.Move( player.m_Movement.Interactable, true );
		}
	}


	//////////////////////////////////////////////////////////////////////////
	// Move
	public	void	Move( Interactable interactable, bool checkOverride = false )
	{
		// start node
		AINode startNode = AI.Pathfinding.GraphMaker.Instance.GetNearestNode( transform.position );

		// final node
		AINode finalNode = AI.Pathfinding.GraphMaker.Instance.GetNearestNode( interactable.transform.position );


		if ( (interactable is IUsableObject ) == false && (finalNode == m_CurrentNode && checkOverride == false ) )
			return;

		m_CurrentNode = finalNode;

		// path finding
		AINode[] path	= AI.Pathfinding.AStarSearch.Instance.FindPath( startNode, finalNode );

		m_Movement = new Navigation();

		if ( path == null || path.Length < 1 )
		{
			m_Movement.HasPath = false;
			return;

		}
		if ( interactable is Lever || interactable is Openable )
		{
			path[ path.Length - 1 ] = null;

			if ( path.Length == 1 && path[ path.Length - 1 ] == null )
			{
				CheckForUsage( interactable );
				return;
			}

		}

		m_Movement.HasPath = true;
		m_Movement.Path = path;
		m_Movement.NodeIdx = 0;
		m_Movement.Action = delegate { CheckForUsage( interactable ); };
		m_Movement.Interactable = interactable;

		m_MovementStartPosition = transform.position;
		m_MovementInterpolant = 0f;
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
			m_Movement.Path[ m_Movement.NodeIdx ].OnNodeReached( this );
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
