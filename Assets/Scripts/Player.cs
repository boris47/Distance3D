
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
		public	IAINode[]				Path;
		public	Vector3					PrevPosition;
		public	int						NodeIdx;
		public	float					NextNodeDistance;
		public	Action					Action;
		public	Interactable			Interactable;

		public	void	Reset()
		{
			HasPath				= false;
			Path				= null;
			PrevPosition		= Vector3.zero;
			NodeIdx				= -1;
			NextNodeDistance	= -1f;
			Action				= null;
			Interactable		= null;
		}
	}
	[ SerializeField ]
	private	Navigation			m_Movement					= default ( Navigation );
	private	Vector3				m_MovementStartPosition		= Vector3.zero;
	private	IAINode				m_CurrentNode				= null;
	private	IEnumerator			m_InputWaitCO				= null;


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
		transform.position = new Vector3( m_CurrentNode.Position.x, height, m_CurrentNode.Position.z );

		m_Movement = new Navigation();
	}


	//////////////////////////////////////////////////////////////////////////
	// OnMouseEnter ( Override )
	protected override void OnMouseEnter()
	{
		if ( CurrentPlayer == this )
			return;

		// highlight object
		base.OnMouseEnter();
	}


	//////////////////////////////////////////////////////////////////////////
	// SetSelected ( Override )
	protected override void OnMouseExit()
	{
		if ( CurrentPlayer == this )
			return;

		// unhighlight object
		base.OnMouseExit();
	}


	//////////////////////////////////////////////////////////////////////////
	// SetSelected
	public	void	SetSelected( bool state )
	{
		foreach( var a in transform.GetComponentsInChildren<Renderer>() )
		{
			a.material.color = state == false ? Color.white : Color.green;
		}
	}


	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( Override )
	public void OnInteraction()
	{
		if ( CameraControl.Instance.Target != null && CameraControl.Instance.Target != transform )
			CameraControl.Instance.Target = transform;
		// player selection
		if ( CurrentPlayer != null && CurrentPlayer != this )
			CurrentPlayer.SetSelected( false );

		CurrentPlayer = this;
		SetSelected( true );
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
	// WaitForNextInput ( Coroutine )
	private	IEnumerator	WaitForNextInput()
	{
		yield return new WaitForSecondsRealtime( 2f );
		m_InputWaitCO = null;
	}


	//////////////////////////////////////////////////////////////////////////
	// Move
	public	void	Move( Interactable interactable, bool checkOverride = false )
	{
		if ( m_InputWaitCO != null && checkOverride == false )
			return;

		if ( m_InputWaitCO != null )
			StopCoroutine( m_InputWaitCO );

		StartCoroutine( m_InputWaitCO = WaitForNextInput() );

		IAINode startNode	= AI.Pathfinding.GraphMaker.Instance.GetNearestNode( transform.position );
		IAINode endNode		= AI.Pathfinding.GraphMaker.Instance.GetNearestNode( interactable.transform.position );
		IAINode[] path		= AI.Pathfinding.AStarSearch.Instance.FindPath( startNode, endNode );
		m_Movement = new Navigation();

		if ( path == null || path.Length < 1 )
		{
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

		m_Movement.HasPath			= true;
		m_Movement.Path				= path;
		m_Movement.PrevPosition		= transform.position;
		m_Movement.NodeIdx			= 0;
		m_Movement.NextNodeDistance	= ( path[0].Position - transform.position ).sqrMagnitude;
		m_Movement.Action			= delegate { CheckForUsage( interactable ); };
		m_Movement.Interactable		= interactable;

		m_MovementStartPosition = transform.position;
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

		float traveledDistance = ( m_Movement.PrevPosition - transform.position ).sqrMagnitude;

		// CHECK
		if ( traveledDistance >= m_Movement.NextNodeDistance )
		{
			m_Movement.Path[ m_Movement.NodeIdx ].OnNodeReached( this );
			m_Movement.NodeIdx ++;

			// Arrived
			if ( m_Movement.NodeIdx == m_Movement.Path.Length || m_Movement.Path[ m_Movement.NodeIdx ] == null )
			{
				if ( m_Movement.Action != null )
					m_Movement.Action();
				m_Movement.Reset();
				return;
			}
			
			m_Movement.NextNodeDistance = ( m_Movement.Path[ m_Movement.NodeIdx ].Position - transform.position ).sqrMagnitude;
			m_MovementStartPosition = m_Movement.PrevPosition = transform.position;
		}

		// go to node
		Vector3 targetDirection = ( m_Movement.Path[ m_Movement.NodeIdx ].Position - transform.position ).normalized;
		transform.position += ( targetDirection * m_Speed ) * Time.deltaTime;
	}

}
