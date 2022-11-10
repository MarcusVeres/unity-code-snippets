using Rewired;
using UnityEngine;

public class RadialMenuCustomInputQS : MonoBehaviour
{
	public CustomRadialMenuInputManager customInput;
	
	// Rewired 
	[Tooltip( "Player ID" )]
	public int playerId = 0;
	private Player player = null;

	[Tooltip("Player Mouse Cursor")]
	public Transform rewiredCursor = null;

    private void Start()
    {
		player = ReInput.players.GetPlayer( playerId );
	}

    private void Update()
	{
		if ( rewiredCursor != null )
		{
			customInput.MousePosition = new Vector2( rewiredCursor.position.x , rewiredCursor.position.y );
		}
		else
		{
			customInput.MousePosition = Input.mousePosition;
		}

		// customInput.InteractButtonDown = Input.GetMouseButtonDown( 0 );
		// customInput.InteractButtonUp = Input.GetMouseButtonUp( 0 );
		// customInput.EnableButtonDown = Input.GetKeyDown( KeyCode.Tab );
		// customInput.EnableButtonUp = Input.GetKeyUp( KeyCode.Tab );

		customInput.InteractButtonDown = player.GetButtonDown( "Validate" ); // || Input.GetMouseButtonUp( 0 );
		customInput.InteractButtonUp = player.GetButtonDown( "Validate" ); //  || Input.GetMouseButtonUp( 0 );
		customInput.EnableButtonDown = player.GetButtonDown( "Radial Menu" );
		customInput.EnableButtonUp = player.GetButtonDown( "Radial Menu" );
	}
}
