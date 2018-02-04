
using UnityEngine;

public class HighLighter : MonoBehaviour {

	public	void	Highlight()
	{
		foreach( Renderer r in GetComponentsInChildren<Renderer>() )
		{
			r.material.color = Color.yellow;
		}
	}

	public	void	Unhighlight()
	{
		foreach( Renderer r in GetComponentsInChildren<Renderer>() )
		{
			r.material.color = Color.white;
		}
	}

}
