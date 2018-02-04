
using UnityEngine;


public class Lever : InteractableAINode {

	[ SerializeField ]
	private	GameEvent	OnUse		= null;

	[ SerializeField ]
	private	GameEvent	OnReset		= null;

	private	bool		m_Used		= false;
	private	Animator	m_Animator	= null;



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
	{
		if ( m_Used == false )
		{
			m_Animator.Play( "OnAction" );

			if ( OnUse != null && OnUse.GetPersistentEventCount() > 0 )
				OnUse.Invoke();
		}
		else
		{
			m_Animator.Play( "OnReset" );

			if ( OnReset != null && OnReset.GetPersistentEventCount() > 0 )
				OnReset.Invoke();
		}

		m_Used = !m_Used;
	}
}
