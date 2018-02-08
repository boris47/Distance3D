using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public interface IPlatformDock {

	Platform	PlatformFather	{ set; }
	bool		Attached		{ get; set; }

}

public class PlatformDock : UsableObject { //, IPlatformDock {
	/*
	private		Platform		m_PlatformFather		= null;
	Platform	IPlatformDock.PlatformFather
	{
		set { m_PlatformFather = value; }
	}

	private		bool			m_HasPlatformAttached	= false;
	bool		IPlatformDock.Attached
	{
		get { return m_HasPlatformAttached; }
		set { m_HasPlatformAttached = value; }
	}



	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( Override ) ( Player Interaction )
	public override void OnInteraction( Player player )
	{
		// player is on dock, link to platform if is there
		if ( m_HasPlatformAttached )
		{
			// move player on platform
			m_PlatformFather.OnPlayerArrivedOnDock( this );
			player.Move( m_PlatformFather );
		}
		else
		{
			// nothing
		}
	}

	
	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( Override )
	public override void OnInteraction()
	{

	}

}

	*/