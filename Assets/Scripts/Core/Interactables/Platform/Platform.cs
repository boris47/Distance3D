using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : AINode, IUsableObject {

	public	float			m_Speed					= 0f;


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
			print( "Platform " + name + " cannot have lerped motion because non enough waypoints, minimal is 4 waypoints... \n setting to linear" );
			m_InterpolationType	 = MotionType.LINEAR;
		}

		m_WaypointsPositions.Add( m_Dock1.transform.position );
		m_WaypointsPositions.Add( m_Dock1.transform.position );
//		print( "added " + m_Dock1.name );
		foreach ( Transform t in waypointsContainer )
		{
			m_WaypointsPositions.Add( t.position );
//			print( "added " + t.name );
		}
		m_WaypointsPositions.Add( m_Dock2.transform.position );
		m_WaypointsPositions.Add( m_Dock2.transform.position );
//		print( "added " + m_Dock2.name );


		( m_Dock1 as IPlatformDock ).PlatformFather = this;
		( m_Dock2 as IPlatformDock ).PlatformFather = this;

		( m_Dock1 as IPlatformDock ).Attached = true;
		( m_Dock2 as IPlatformDock ).Attached = false;
	}


	//////////////////////////////////////////////////////////////////////////
	// OnInteraction From IUsableObject
	void IUsableObject.OnInteraction( Player player )
	{
		if ( player == null )
			return;

		player.transform.SetParent( transform );
		MovePlatform();
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
		AI.Pathfinding.GraphMaker.Instance.UpdaeNeighbours( this );
		m_InterpolantGlobal = 0f;
		m_CurrentWaypointIdx = 0;
		IsMoving = false;
		m_WaypointsPositions.Reverse();
		( m_Dock1 as IPlatformDock ).Attached = !( m_Dock1 as IPlatformDock ).Attached;
		( m_Dock2 as IPlatformDock ).Attached = !( m_Dock2 as IPlatformDock ).Attached;
		if ( transform.childCount > 0 )
			transform.GetChild(0).SetParent( null );
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
			m_InterpolantGlobal += Time.deltaTime * m_Speed * 0.1f;
			Vector3 position = Interp( m_WaypointsPositions, m_InterpolantGlobal );
			transform.position = position;

			if ( m_InterpolantGlobal >= 1f )
			{
				m_InterpolantGlobal = 0f;
				OnPathCompleted();
			}
		}
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
















	

	private Vector3 Interp( List<Vector3> wayPoints, float t )
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
