
using UnityEngine;


public class PressurePlate : UsableObject {

	private	Animator	m_Animator		= null;


	//////////////////////////////////////////////////////////////////////////
	// START ( Override )
	protected override void Start()
	{
		base.Start();

		m_Animator = GetComponent<Animator>();
	}


	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( Override )
	public override void OnInteraction()
	{}


	//////////////////////////////////////////////////////////////////////////
	// OnPlateInteraction
	public	void	OnPlateInteraction()
	{
		if ( m_IsActive == false )
			return;

		if ( m_IsActivated == false )
		{
			m_Animator.Play( "OnAction" );

			if ( m_OnUse != null && m_OnUse.GetPersistentEventCount() > 0 )
				m_OnUse.Invoke();
		}
		else
		{
			m_Animator.Play( "OnReset" );

			if ( m_OnReset != null && m_OnReset.GetPersistentEventCount() > 0 )
				m_OnReset.Invoke();
		}

		m_IsActivated = !m_IsActivated;
	}


	//////////////////////////////////////////////////////////////////////////
	// UNITY
	//////////////////////////////////////////////////////////////////////////

	//////////////////////////////////////////////////////////////////////////
	// OnTriggerEnter
	private void OnTriggerEnter( Collider other )
	{
		if ( other.GetComponent<Player>() != null )
			OnPlateInteraction();
	}


	//////////////////////////////////////////////////////////////////////////
	// OnTriggerExit
	private void OnTriggerExit( Collider other )
	{
		if ( other.GetComponent<Player>() != null )
			OnPlateInteraction();
	}

}
