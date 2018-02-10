
using UnityEngine;


public class Lever : UsableObject, IUsableObject {

	private	Animator	m_Animator				= null;


	//////////////////////////////////////////////////////////////////////////
	// START ( Override )
	protected override void Start()
	{
		base.Start();

		m_Animator	= GetComponent<Animator>();
	}


	//////////////////////////////////////////////////////////////////////////
	// OnEndAnimation
	public	void	OnEndAnimation()
	{
		m_IsActive = true;
	}


	//////////////////////////////////////////////////////////////////////////
	// OnInteractionInternal
	public void OnInteractionInternal()
	{
		if ( m_IsActive == false )
			return;

		if ( m_IsActivated == false )
		{
			m_Animator.Play( "OnAction" );

			if ( m_OnUse != null && m_OnUse.GetPersistentEventCount() > 0 )
				m_OnUse.Invoke( null );
		}
		else
		{
			m_Animator.Play( "OnReset" );
			if ( m_OnReset != null && m_OnReset.GetPersistentEventCount() > 0 )
				m_OnReset.Invoke( null );
		}

		m_IsActive = false;
		m_IsActivated = !m_IsActivated;
	}

	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( Override )
	public override void OnInteraction()
	{
		OnInteractionInternal();
	}

	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( Override ) ( Player Interaction )
	public override void OnInteraction( Player player )
	{
		OnInteractionInternal();
	}
}
