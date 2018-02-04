
using UnityEngine;

public partial interface ICameraControl {

	Interactable	CurrentInteractable { get; set; }

}

public partial class CameraControl : ICameraControl {

	public	Interactable	CurrentInteractable
	{
		get;
		set;
	}
	


	//////////////////////////////////////////////////////////////////////////
	// CheckInteractions
	private	void	CheckInteractions()
	{
		if ( Input.GetMouseButtonDown( ( int ) m_InteractionButton ) )
		{

			if ( CurrentInteractable == null )
			{
				Player.CurrentPlayer = null;
				return;
			}


			if ( CurrentInteractable is InteractableAINode )
			{
				if ( Player.CurrentPlayer != null )
					Player.CurrentPlayer.Move( CurrentInteractable );
				return;
			}

			// Player selection
			CurrentInteractable.OnInteraction();
			
		}
	}

}
