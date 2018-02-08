using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : AINode {

	public	float	m_Speed = 0f;

	[ SerializeField ]
	private	Transform[]	m_WayPoints			= null;

	[ SerializeField ][ Range( 2, 4 ) ]
	private	int			m_InterpolationStep	= 2;

	private	bool		m_IsMoving			= false;
	private	int			m_WaypointsMovStep	= 0;
	private	Vector3[]	m_CurrentWaypoints	= null;
	private	float		m_Interpolant		= 0f;
	private	bool		m_HasArrived		= true;

	// On transition end
	//		AI.Pathfinding.GraphMaker.Instance.UpdaeNeighbours( this );

	private void Awake()
	{
		m_HasArrived = false;
		SetNextWaypoints();
	}


	
	public	bool	SetNextWaypoints()
	{
		print( m_WaypointsMovStep + m_InterpolationStep );

		if ( m_WaypointsMovStep + m_InterpolationStep > m_WayPoints.Length )
			return false;

		if ( m_InterpolationStep == 2 )
		{
			m_CurrentWaypoints = new Vector3[] {
				transform.position,
				m_WayPoints[m_WaypointsMovStep].position,
			};
		}

		if ( m_InterpolationStep ==  3 )
		{
			m_CurrentWaypoints = new Vector3[] {
				transform.position,
				m_WayPoints[m_WaypointsMovStep].position,
				m_WayPoints[m_WaypointsMovStep + 1].position,
			};
		}

		if ( m_InterpolationStep ==  4 )
		{
			m_CurrentWaypoints = new Vector3[] {
				transform.position,
				m_WayPoints[m_WaypointsMovStep].position,
				m_WayPoints[m_WaypointsMovStep + 1].position,
				m_WayPoints[m_WaypointsMovStep + 2].position,
			};
		}

		m_WaypointsMovStep ++;;
		return true;
	}
	

	public	bool	MoveAlongWayPoints()
	{
		if ( m_Interpolant >= 1f )
		{
			m_Interpolant = 0f;
			m_IsMoving = false;
			return false;
		}

		m_Interpolant += Time.deltaTime * m_Speed;

		if ( m_InterpolationStep == 2 )
		{
			Vector3 a = m_CurrentWaypoints[0];
			Vector3 b = m_CurrentWaypoints[1];

			Vector3 position = Vector3.Lerp( a, b, m_Interpolant );
			transform.position = position;
		}

		if ( m_InterpolationStep == 3 )
		{
			Vector3 a = m_CurrentWaypoints[0];
			Vector3 b = m_CurrentWaypoints[1];
			Vector3 c = m_CurrentWaypoints[2];

			Vector3 position = QuadraticLerp( a, b, c, m_Interpolant );
			transform.position = position;
		}

		if ( m_InterpolationStep == 4 )
		{
			Vector3 a = m_CurrentWaypoints[0];
			Vector3 b = m_CurrentWaypoints[1];
			Vector3 c = m_CurrentWaypoints[2];
			Vector3 d = m_CurrentWaypoints[3];

			Vector3 position = CubicLerp( a, b, c, d, m_Interpolant );
			transform.position = position;
		}

		return true;
	}


	private void Update()
	{

		if ( m_HasArrived == false && MoveAlongWayPoints() == false )
		{
			if ( SetNextWaypoints() == false )
			{
				m_HasArrived = true;
				print( "arrived" );
			}
		}

	}






	private	Vector3 QuadraticLerp( Vector3 a, Vector3 b, Vector3 c, float interpolant )
	{
		Vector3 p0 = Vector3.Lerp( a, b, interpolant );
		Vector3 p1 = Vector3.Lerp( b, c, interpolant );
		return Vector3.Lerp( p0, p1, interpolant );
	}

	private	Vector3 CubicLerp( Vector3 a, Vector3 b, Vector3 c, Vector3 d, float interpolant )
	{
		Vector3 p0 = QuadraticLerp( a, b, c, interpolant );
		Vector3 p1 = QuadraticLerp( b, c, d, interpolant );
		return Vector3.Lerp( p0, p1, interpolant );
	}



	/*
	[ SerializeField ]
	private		float				m_MovementSpeed			= 2f;

	[ SerializeField ]
	private		PlatformDock		m_AttachedDock			= null;


	private		PlatformDock		m_Dock1					= null;
	private		PlatformDock		m_Dock2					= null;



	private		IEnumerator			m_PlayerTransition		= null;

	
	//////////////////////////////////////////////////////////////////////////
	// AWAKE
	private void	Awake()
	{
		m_Dock1 = transform.GetChild( 0 ).GetComponent<PlatformDock>();
		m_Dock2 = transform.GetChild( 1 ).GetComponent<PlatformDock>();

		( m_Dock1 as IPlatformDock ).PlatformFather = this;
		( m_Dock2 as IPlatformDock ).PlatformFather = this;

		if ( m_AttachedDock == null )
		{
			( m_Dock1 as IPlatformDock ).Attached = true;
		}
		else
		{
			( m_AttachedDock as IPlatformDock ).Attached = true;
		}

		// must set platform as really dock to a dock

	}
	

	//////////////////////////////////////////////////////////////////////////
	// START
	protected override void Start()
	{
//		IsInteractable = false;
		base.Start();
	}
	
	//////////////////////////////////////////////////////////////////////////
	// OnPlayerArrivedOnDock
	public	void	OnPlayerArrivedOnDock( PlatformDock dock )
	{

	}


	//////////////////////////////////////////////////////////////////////////
	// PlayerTransition ( Cooutine )
	private	IEnumerator	PlayerTransition( PlatformDock dock )
	{
		// Take player control

		yield return null;
	}


	//////////////////////////////////////////////////////////////////////////
	// UNITY
	//////////////////////////////////////////////////////////////////////////

	//////////////////////////////////////////////////////////////////////////
	// Update
	private void	Update()
	{
		
	}
	*/
}
