using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPlatformDock {

	Platform	PlatformFather	{ set; }
	bool		Attached		{ get; set; }

}

public class PlatformDock : UsableObject, IPlatformDock {
	
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
		print("received");
		if ( m_HasPlatformAttached == false && m_PlatformFather.IsMoving == false )
			m_PlatformFather.MovePlatform();
	}

	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( Override ) ( From Camera Interaction )
	public override void OnInteraction()
	{
	}

}

	