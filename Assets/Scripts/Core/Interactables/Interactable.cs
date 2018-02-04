using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent( typeof ( HighLighter ) )]
public abstract class Interactable : MonoBehaviour {

	private	HighLighter m_HighLighter = null;


	//////////////////////////////////////////////////////////////////////////
	// START
	protected virtual void Start()
	{
		m_HighLighter = GetComponent<HighLighter>();
	}


	//////////////////////////////////////////////////////////////////////////
	// OnInteraction ( abstract )
	public	abstract	void	OnInteraction();

	
	//////////////////////////////////////////////////////////////////////////
	// UNITY
	//////////////////////////////////////////////////////////////////////////
	
	//////////////////////////////////////////////////////////////////////////
	// OnMouseEnter
	protected	virtual void OnMouseEnter()
	{
		CameraControl.Instance.CurrentInteractable = this;
		m_HighLighter.Highlight();
	}


	//////////////////////////////////////////////////////////////////////////
	// OnMouseExit
	protected	virtual void OnMouseExit()
	{
		CameraControl.Instance.CurrentInteractable = null;
		m_HighLighter.Unhighlight();
	}


}
