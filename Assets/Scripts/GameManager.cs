using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameEvent      : UnityEngine.Events.UnityEvent { }

public class GameManager : MonoBehaviour {

	public	static	GameManager Instance = null;



	//////////////////////////////////////////////////////////////////////////
	// AWAKE
	private void Awake()
	{
		Instance = this;
	}



}
