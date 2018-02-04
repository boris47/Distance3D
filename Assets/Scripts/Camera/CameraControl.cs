
using UnityEngine;

public partial interface ICameraControl {

	Transform		Target			{ get; set; }
	Camera			MainCamera		{ get; }

}

public partial class CameraControl : MonoBehaviour, ICameraControl {

	public static	ICameraControl		Instance		= null;

	public	const	float	CLAMP_MAX_X_AXIS			= 80.0f;
	public	const	float	CLAMP_MIN_X_AXIS			= 20.0f;

//	[SerializeField][Header("Target")]
	public	Transform		Target
	{
		get; set;
	}


	[SerializeField][Header("Camera TPS Settings")]
	private	Vector2			m_TPSOffset					= Vector2.zero;

	[SerializeField][Range( 0.01f, 100.0f )]
	private	float			m_MinOffset					= 5f;
	[SerializeField][Range( 0.01f, 100.1f )]
	private	float			m_MaxOffset					= 20f;
	[SerializeField][Range( 3f, 1000f )]
	private	float			m_MaxTraslation				= 10f;
	[SerializeField][Range( 0.1f, 10f )]
	private	float			m_TraslationSensitivity		= 5f;
	[SerializeField][Range( 0.1f, 20f )]
	private	float			m_RotationSensitivity		= 5f;

	[SerializeField][Range( 0.2f, 20.0f )]
	private	float			m_MouseSensitivity			= 1.0f;


	private enum MouseButtons : int {
		LEFT, RIGHT, MIDDLE
	}
	[Header("Inputs")]
	[SerializeField]
	private	MouseButtons	m_RotationButton			= MouseButtons.RIGHT;
	[SerializeField]
	private	MouseButtons	m_InteractionButton			= MouseButtons.LEFT;
	[SerializeField]
	private	MouseButtons	m_TraslationButton			= MouseButtons.MIDDLE;


	[Header("Optionals")]
	[SerializeField]
	private bool			m_SmoothedRotation			= true;
	[SerializeField]
	private	bool			m_SmoothedPosition			= true;

	[SerializeField][Range( 1.0f, 10.0f )]
	private float			m_RotationSmoothFactor		= 1.0f;
	[SerializeField][Range( 1.0f, 10.0f )]
	private float			m_TraslationSmoothFactor	= 1.0f;


	public	Camera			MainCamera
	{
		get;
		private set;
	}

	private float			m_CurrentRotation_X_Delta	= 1.0f;
	private float			m_CurrentRotation_Y_Delta	= 1.0f;

	private float			m_CameraOffset				= 5.0f;
	private float			m_CurrentCameraOffset		= 5.0f;
	private	float			m_CameraFPS_Shift			= 0.0f;

	private	Vector3			m_CurrentDirection			= Vector3.zero;
	private	Vector3			m_Traslation				= Vector3.zero;


	//////////////////////////////////////////////////////////////////////////
	// AWAKE
	private void Awake()
	{
		Instance = this;

		MainCamera = transform.GetComponent<Camera>();

		m_CurrentDirection = transform.rotation.eulerAngles;
	}

	//////////////////////////////////////////////////////////////////////////
	// START
	private void Start()
	{

	}


	//////////////////////////////////////////////////////////////////////////
	// UNITY
	//////////////////////////////////////////////////////////////////////////

	//////////////////////////////////////////////////////////////////////////
	// Update
	private void Update()
	{
		
		if ( m_MinOffset >= m_MaxOffset )
			m_MaxOffset = m_MinOffset + 0.1f;

		m_CameraOffset = Mathf.Clamp( m_CameraOffset + -Input.mouseScrollDelta.y , m_MinOffset, m_MaxOffset );

		// evaluate interactions
		this.CheckInteractions();
	}


	//////////////////////////////////////////////////////////////////////////
	// LateUpdate
	private void LateUpdate()
	{
		float Axis_X_Delta = Input.GetAxis ( "Mouse X" ) * m_MouseSensitivity;
		float Axis_Y_Delta = Input.GetAxis ( "Mouse Y" ) * m_MouseSensitivity;

		if ( Input.GetKeyDown( KeyCode.F ) && Player.CurrentPlayer != null )
		{
			Target = Player.CurrentPlayer.transform;
		}


		////////////////////////////////////////////////////////////////////////////////
		// Rotation WITHOUT target
		if ( Input.GetMouseButton( ( int ) m_RotationButton ) )
		{
			float rotationSensitivity = 20f / m_RotationSensitivity;
			Axis_X_Delta /= rotationSensitivity;
			Axis_Y_Delta /= rotationSensitivity;

			if ( m_SmoothedRotation )
			{
				m_CurrentRotation_X_Delta = Mathf.Lerp( m_CurrentRotation_X_Delta, Axis_X_Delta, Time.deltaTime * ( 100f / m_RotationSmoothFactor ) );
				m_CurrentRotation_Y_Delta = Mathf.Lerp( m_CurrentRotation_Y_Delta, Axis_Y_Delta, Time.deltaTime * ( 100f / m_RotationSmoothFactor ) );
			}
			else
			{
				m_CurrentRotation_X_Delta = Axis_X_Delta;
				m_CurrentRotation_Y_Delta = Axis_Y_Delta;
			}

			if ( m_CurrentRotation_X_Delta != 0.0f || m_CurrentRotation_Y_Delta != 0.0f )
			{
				m_CurrentDirection.x = Mathf.Clamp( m_CurrentDirection.x - m_CurrentRotation_Y_Delta, CLAMP_MIN_X_AXIS, CLAMP_MAX_X_AXIS );
				m_CurrentDirection.y = m_CurrentDirection.y + m_CurrentRotation_X_Delta;
			}

			// rotation with effect added
			transform.rotation = Quaternion.Euler( m_CurrentDirection );
		}


		////////////////////////////////////////////////////////////////////////////////
		// Position WITH target ( rotation around target )
		m_CurrentCameraOffset = Mathf.Lerp( m_CurrentCameraOffset, m_CameraOffset, Time.deltaTime * 6f );
		if ( Target != null )
		{
			Vector3 positionRelativeToTarget = Target.position - ( transform.forward * m_CurrentCameraOffset );
			positionRelativeToTarget += transform.TransformDirection( m_TPSOffset );

			if ( m_SmoothedPosition )
				transform.position = Vector3.Lerp( transform.position, positionRelativeToTarget, Time.deltaTime * 8f );
			else
			{
				transform.position = positionRelativeToTarget;
			}
		}


		////////////////////////////////////////////////////////////////////////////////
		// Position WITHOUT target ( traslation )
		{	
			Vector3 destTraslation = Vector3.zero;
			Vector3 vCamForward = Vector3.Scale( transform.forward, new Vector3( 1.0f, 0.0f, 1.0f ) ).normalized;
			float	traslationSensitivity = 10f / m_TraslationSensitivity;
			if ( Input.GetMouseButton( ( int ) m_TraslationButton ) )
			{
				Target = null;
				destTraslation = ( -Axis_Y_Delta/traslationSensitivity * vCamForward ) + ( -Axis_X_Delta/traslationSensitivity * transform.right );
			}
			else
			{
				const float sensitivity = 0.2f;
				if ( Input.GetKey( KeyCode.W ) )
				{
					destTraslation = (  sensitivity/traslationSensitivity * vCamForward );
					Target = null;
				}
				else
				if ( Input.GetKey( KeyCode.S ) )
				{
					destTraslation = ( -sensitivity/traslationSensitivity * vCamForward );
					Target = null;
				}

				if ( Input.GetKey( KeyCode.A ) )
				{
					destTraslation += ( -sensitivity/traslationSensitivity * transform.right );
					Target = null;
				}
				else
				if ( Input.GetKey( KeyCode.D ) )
				{
					destTraslation += (  sensitivity/traslationSensitivity * transform.right );
					Target = null;
				}

			}

			if ( Target == null )
			{
				if ( m_SmoothedPosition )
				{
					m_Traslation = Vector3.Lerp( m_Traslation, destTraslation, Time.deltaTime * ( 100f / m_TraslationSmoothFactor ) );
				}
				else
				{
					m_Traslation = destTraslation;
				}

				Vector3 newPosition = transform.position + m_Traslation;
				newPosition.y = m_CurrentCameraOffset;
				float magnitude = newPosition.sqrMagnitude;
				if ( magnitude < m_MaxTraslation * m_MaxTraslation )
				{
					transform.position = newPosition;
				}
			}
		}
		


		
	}

}
