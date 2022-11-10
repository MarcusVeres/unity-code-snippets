using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRadialMenuInputManager : UltimateRadialMenuInputManager
{
	// Rewired 
	[Tooltip( "Player ID" )]
	public int playerId = 0;
	private Player player = null;

	protected override void Start()
	{
		base.Start();
		player = ReInput.players.GetPlayer( playerId );
	}

	public Vector2 MousePosition
	{
		get;
		set;
	}

	bool interactButtonDown;
	public bool InteractButtonDown
	{
		get
		{
			if( interactButtonDown )
			{
				interactButtonDown = false;
				return true;
			}

			return interactButtonDown;
		}
		set
		{
			interactButtonDown = value;
		}
	}

	bool interactButtonUp;
	public bool InteractButtonUp
	{
		get
		{
			if( interactButtonUp )
			{
				interactButtonUp = false;
				return true;
			}

			return interactButtonUp;
		}
		set
		{
			interactButtonUp = value;
		}
	}

	bool enableButtonDown;
	public bool EnableButtonDown
	{
		get
		{
			if( enableButtonDown )
			{
				enableButtonDown = false;
				return true;
			}

			return enableButtonDown;
		}
		set
		{
			enableButtonDown = value;
		}
	}

	bool enableButtonUp;
	public bool EnableButtonUp
	{
		get
		{
			if( enableButtonUp )
			{
				enableButtonUp = false;
				return true;
			}

			return enableButtonUp;
		}
		set
		{
			enableButtonUp = value;
		}
	}


	public override void ControllerInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		// Store a boolean to check if the input from the mouse & keyboard has been caught.
		bool inputModifiedThisFrame = false;

		// If any of the bool variables have been modified, then the mouse and keyboard is still the active input, so set the bool to true.
		if( enableMenu || disableMenu || inputDown || inputUp )
			inputModifiedThisFrame = true;

		// Store the horizontal and vertical axis of the targeted joystick axis.
		Vector2 controllerInput = Vector2.zero;

		// Store the horizontal and vertical axis of the targeted joystick axis.
		controllerInput = new Vector2( Input.GetAxis( horizontalAxisController ), Input.GetAxis( verticalAxisController ) );
		// EDIT: Change this to your rewired input.

		// If the activation action is set to being the press of a button on the controller...
		inputDown = player.GetButtonDown( "Validate" );
		inputUp = player.GetButtonUp( "Validate" );

		// EDIT: If you want to handle the enable/disable, do that here.

		// If the input had not been modified from the Mouse & Keyboard function before this one, then check any of the bool variables. If they have changed, set the current input device to controller for reference.
		if ( !inputModifiedThisFrame && ( enableMenu || disableMenu || inputDown || inputUp ) )
			CurrentInputDevice = InputDevice.Controller;

		// If the controller input is not zero...
		if( controllerInput != Vector2.zero )
		{
			// Set the current input device for reference.
			CurrentInputDevice = InputDevice.Controller;

			// If the user wants to invert the horizontal axis, then multiply by -1.
			if( invertHorizontal )
				controllerInput.x *= -1;

			// If the user wants to invert the vertical axis, then do that here.
			if( invertVertical )
				controllerInput.y *= -1;

			// Set the input to what was calculated.
			input = controllerInput;

			// Since this is a controller, we want to make sure that our input distance feels right, so here we will temporarily store the distance before modification.
			float tempDist = Vector2.Distance( Vector2.zero, controllerInput );

			// If the temporary distance is greater than the minimum range, then the distance doesn't matter. All we want to send to the radial menu is that it is perfectly in range, so make the distance exactly in the middle of the min and max.
			if( tempDist >= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.minRange )
				distance = Mathf.Lerp( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange, UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange, 0.5f );
		}
	}

	public override void CustomInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		inputDown = InteractButtonDown;
		inputUp = InteractButtonUp;

		// If the keyboard enable button is assigned, and the menu is not in world space...
		if( enableMenuSetting != EnableMenuSetting.Manual )
		{
			enableMenu = EnableButtonDown;
			disableMenu = EnableButtonUp;
		}

		// If the previous mouse input is the same as the current mouse position, the last known input device was not the mouse, and no input was captured from the mouse and keyboard, then return.
		if( UltimateRadialMenuInformations[ radialMenuIndex ].previousMouseInput == MousePosition && CurrentInputDevice != InputDevice.Mouse && !inputDown && !inputUp && !enableMenu && !disableMenu )
			return;

		// Set the current input device for reference.
		CurrentInputDevice = InputDevice.Mouse;

		// Store the current mouse position for reference on the next frame.
		UltimateRadialMenuInformations[ radialMenuIndex ].previousMouseInput = MousePosition;

		// If this radial menu is world space then send in the information to raycast from.
		if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
			RaycastWorldSpaceRadialMenu( ref input, ref distance, MousePosition, radialMenuIndex );
		// Else the radial menu is on the screen, so process mouse input.
		else
		{
			// Figure out the position of the input on the canvas. ( mouse position / canvas scale factor ) - ( half the canvas size );
			Vector2 inputPositionOnCanvas = ( MousePosition / UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.scaleFactor ) - ( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.GetComponent<RectTransform>().sizeDelta / 2 );

			// Apply our new calculated input. ( input position - local position of the menu ) / ( half the menu size );
			input = ( inputPositionOnCanvas - ( Vector2 )UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition ) / ( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.sizeDelta.x / 2 );

			// Configure the distance of the mouse position from the Radial Menu's base position.
			distance = Vector2.Distance( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition, inputPositionOnCanvas );
		}
	}
}
