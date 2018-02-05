using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent( typeof ( HighLighter ) )]
public abstract class Interactable : MonoBehaviour {

	[ SerializeField ]
	protected	bool						m_IsInteractable		= true;
	public		bool						IsInteractable
	{
		get { return m_IsInteractable; }
		set { m_IsInteractable = value; }
	}

	private	HighLighter m_HighLighter = null;


	//////////////////////////////////////////////////////////////////////////
	// START
	protected virtual void Start()
	{
		m_HighLighter = GetComponent<HighLighter>();
	}

	
	//////////////////////////////////////////////////////////////////////////
	// UNITY
	//////////////////////////////////////////////////////////////////////////
	
	//////////////////////////////////////////////////////////////////////////
	// OnMouseEnter
	protected	virtual void OnMouseEnter()
	{
		if ( m_IsInteractable == false )
			return;

		CameraControl.Instance.CurrentInteractable = this;
		m_HighLighter.Highlight();
	}


	//////////////////////////////////////////////////////////////////////////
	// OnMouseExit
	protected	virtual void OnMouseExit()
	{
		if ( m_IsInteractable == false )
			return;

		CameraControl.Instance.CurrentInteractable = null;
		m_HighLighter.Unhighlight();
	}


}
