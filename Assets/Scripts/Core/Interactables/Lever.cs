﻿
using UnityEngine;


public class Lever : UsableObject {

	private	Animator	m_Animator	= null;


	//////////////////////////////////////////////////////////////////////////
	// START ( Override )
	protected override void Start()
	{
		base.Start();

		m_Animator	= GetComponent<Animator>();
	}


	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( Override )
	public override void OnInteraction()
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

		m_IsActivated = !m_IsActivated;
	}

	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( Override ) ( Player Interaction )
	public override void OnInteraction( Player player )
	{

	}
}
