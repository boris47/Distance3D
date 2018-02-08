
using System;
using UnityEngine;

public abstract class UsableObject : Interactable {

	[ SerializeField ]
	protected	GameEvent					m_OnUse				= null;

	[ SerializeField ]
	protected	GameEvent					m_OnReset			= null;

	[ SerializeField ]
	protected	bool						m_IsActive			= true;
	public		bool						IsActive
	{
		get { return m_IsActive; }
		set { m_IsActive = value; }
	}

	[ SerializeField ]
	protected	bool						m_IsActivated		= false;
	public		bool						IsActivated
	{
		get { return m_IsActivated; }
		set { m_IsActivated = value; }
	}


	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( abstract )
	public	virtual	void	OnInteraction() { }

	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( abstract )
	public	abstract	void	OnInteraction( Player player );

}
