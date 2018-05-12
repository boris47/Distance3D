using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : AINode {

	public	float			m_Speed					= 0f;

	public	const float		LERPED_DIST_MULTI		= 0.4f;

	private	PlatformDock	m_Dock1					= null;
	private	PlatformDock	m_Dock2					= null;
	private	Transform		m_Plate					= null;


	// PLATFORM MOVEMENT
	private	enum MotionType {
		LINEAR, LERPED
	}
	[ SerializeField ]
	private	MotionType	m_InterpolationType			= MotionType.LINEAR;

	public	bool		IsMoving
	{
		get; set;
	}
	private	Vector3		m_NextWaypoint				= Vector3.zero;
	private	Vector3		m_CurrentPosition			= Vector3.zero;
	private	int			m_CurrentWaypointIdx		= 0;
	private	float		m_InterpolantGlobal			= 0f;
	[SerializeField]
	List<Vector3>		m_WaypointsPositions		= null;

	private	Player		m_AttachedPlayer			= null;




	//////////////////////////////////////////////////////////////////////////
	// AWAKE
	private void Awake()
	{

		m_Dock1 = transform.parent.Find( "Dock1" ).GetComponent<PlatformDock>();
		m_Dock2 = transform.parent.Find( "Dock2" ).GetComponent<PlatformDock>();
		m_Plate	= transform.parent.Find( "Plate" );
		Transform waypointsContainer = transform.parent.Find( "WayPoints" );

		if ( waypointsContainer.childCount < 1 && m_InterpolationType == MotionType.LERPED )
		{
			print( "Platform " + name + " cannot have lerped motion because non enough waypoints, minimal is 1 waypoint... \n setting to linear" );
			m_InterpolationType	 = MotionType.LINEAR;
		}

		m_WaypointsPositions.Add( m_Dock1.transform.position );
		m_WaypointsPositions.Add( m_Dock1.transform.position );

		foreach ( Transform t in waypointsContainer )
		{
			m_WaypointsPositions.Add( t.position );
		}
		m_WaypointsPositions.Add( m_Dock2.transform.position );
		m_WaypointsPositions.Add( m_Dock2.transform.position );

		// set platform porition on Dock 1
		transform.position = new Vector3( m_Dock1.transform.position.x, transform.position.y, m_Dock1.transform.position.z );

		( m_Dock1 as IPlatformDock ).PlatformFather = this;
		( m_Dock2 as IPlatformDock ).PlatformFather = this;

		( m_Dock1 as IPlatformDock ).Attached = true;
		( m_Dock2 as IPlatformDock ).Attached = false;
	}


	//////////////////////////////////////////////////////////////////////////
	// MovePlatform
	public	void	MovePlatform()
	{
		if ( IsMoving == true )
			return;

		if ( m_InterpolationType == MotionType.LINEAR )
		{
			m_CurrentWaypointIdx = 0;
			SetNextWaypoints();
		}

		IsMoving = true;
	}


	//////////////////////////////////////////////////////////////////////////
	// OnPathCompleted
	private	void	OnPathCompleted()
	{
		// Get new neighbours
		AI.Pathfinding.GraphMaker.Instance.UpdateNeighbours( this, isUpdate: true );

		// restore internal state
		m_InterpolantGlobal = 0f;
		m_CurrentWaypointIdx = 0;
		IsMoving = false;
		m_WaypointsPositions.Reverse();

		// toogle docks state
		( m_Dock1 as IPlatformDock ).Attached = !( m_Dock1 as IPlatformDock ).Attached;
		( m_Dock2 as IPlatformDock ).Attached = !( m_Dock2 as IPlatformDock ).Attached;

		// Detach Player if found
		Player player = transform.GetComponentInChildren<Player>();
		if ( player != null )
		{
			player.transform.SetParent( null );
			player.IsInteractable = true;
		}

		// re-set player as interactable
		if ( Player.CurrentPlayer == null )
			Player.CurrentPlayer = m_AttachedPlayer;
	}
	

	//////////////////////////////////////////////////////////////////////////
	// SetNextWaypoints
	private	void	SetNextWaypoints()
	{
//		print( m_CurrentWaypointIdx );
		if ( m_CurrentWaypointIdx == m_WaypointsPositions.Count )
		{
			OnPathCompleted();
			return;
		}

		m_CurrentPosition	= transform.position;
		m_NextWaypoint		= m_WaypointsPositions[m_CurrentWaypointIdx];

		m_CurrentWaypointIdx ++;
	}


	//////////////////////////////////////////////////////////////////////////
	// Move
	private	void	Move()
	{
		// LINEAR
		if ( m_InterpolationType == MotionType.LINEAR )
		{
			m_InterpolantGlobal += Time.deltaTime * m_Speed;
			Vector3 position = Vector3.Lerp( m_CurrentPosition, m_NextWaypoint, m_InterpolantGlobal );
			transform.position = position;

			if ( m_InterpolantGlobal >= 1f )
			{
				m_InterpolantGlobal = 0f;
				SetNextWaypoints();
			}
		}

		// LERPED
		if ( m_InterpolationType == MotionType.LERPED )
		{
			m_InterpolantGlobal += Time.deltaTime * m_Speed * ( LERPED_DIST_MULTI / m_WaypointsPositions.Count );
			Vector3 position = Interp( ref m_WaypointsPositions, m_InterpolantGlobal );
			transform.position = position;

			if ( m_InterpolantGlobal >= 1f )
			{
				m_InterpolantGlobal = 0f;
				OnPathCompleted();
			}
		}
	}

	
	//////////////////////////////////////////////////////////////////////////
	// OnNodeReached ( Override )
	public override void OnNodeReached( Player player )
	{
		player.transform.SetParent( transform );
		player.IsInteractable = false;
		Player.CurrentPlayer = null;
		m_AttachedPlayer = player;
		MovePlatform();
	}


	private Vector3 Interp( ref List<Vector3> wayPoints, float t )
	{
		int numSections = wayPoints.Count - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
		
		Vector3 a = wayPoints[ currPt + 0 ];
		Vector3 b = wayPoints[ currPt + 1 ];
		Vector3 c = wayPoints[ currPt + 2 ];
		Vector3 d = wayPoints[ currPt + 3 ];
		
		return .5f * 
		(
			( -a + 3f * b - 3f * c + d )		* ( u * u * u ) +
			( 2f * a - 5f * b + 4f * c - d )	* ( u * u ) +
			( -a + c )							* u +
			2f * b
		);
	}


	//////////////////////////////////////////////////////////////////////////
	// UNITY
	//////////////////////////////////////////////////////////////////////////

	//////////////////////////////////////////////////////////////////////////
	// Update
	private void Update()
	{
		if ( IsMoving == true )
		{
			Move();
		}
	}

	/*
	private void OnDrawGizmos()
	{
		Transform waypointsContainer = transform.parent.Find( "WayPoints" );
		List<Vector3> waypointsPositions = new List<Vector3>();

		foreach( Transform t in waypointsContainer )
			waypointsPositions.Add( t.position );

		for ( int i = 0; i < m_WaypointsPositions.Count - 1; i++ )
		{
			Gizmos.DrawLine( m_WaypointsPositions[i], m_WaypointsPositions[i+1] );

		}
	}
	*/
}
