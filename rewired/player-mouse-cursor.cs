using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewiredPlayerMouseQS : MonoBehaviour
{
    public EventSystem eventSystem;                                                         // Need EventSystem to interact with a Canvas / UI. 
    public GraphicRaycaster raycaster;                                                      // Raycast for UI elements.

    private GameObject cursorTarget = null;                                                 // What pointer/cursor is hovering over.
    private GameObject previousTarget = null;                                               // What pointer hovered over just before the current target.

    private PointerEventData ped = new PointerEventData( EventSystem.current );             // Initialize pointer event data object (stores information about mouse events).

    public void OnScreenPositionChanged( Vector2 screenPosition )
    {
        // Position the pointer on a Unity UI Canvas.
        // Copied from: https://guavaman.com/projects/rewired/docs/PlayerControllers.html#player-mouse

        RectTransform canvasRectTransform = transform.root.GetComponentInChildren<Canvas>().GetComponent<RectTransform>();
        Rect rootCanvasRect = canvasRectTransform.rect;
        Vector2 viewportPos = Camera.main.ScreenToViewportPoint( screenPosition );
        viewportPos.x = ( viewportPos.x * rootCanvasRect.width ) - canvasRectTransform.pivot.x * rootCanvasRect.width;
        viewportPos.y = ( viewportPos.y * rootCanvasRect.height ) - canvasRectTransform.pivot.y * rootCanvasRect.height;
        ( transform as RectTransform ).anchoredPosition = viewportPos;
    }

    public void OnAxisValueChanged( int axis , float single )
    {
        if ( single == 0 ) { return; }                                                      // Ignore if mouse and joystick are not moving.
                                                                                            
        ped = new PointerEventData( eventSystem );                                          // Docs: "Each touch event creates one of these containing all the relevant information."
        ped.position = new Vector2( transform.position.x , transform.position.y );          // Set the Pointer Event Position to that of the cursor image
        List<RaycastResult> results = new List<RaycastResult>();                            // Create a list of Raycast Results
        raycaster.Raycast( ped , results );                                                 // Raycast using the Graphics Raycaster and mouse click position
                                                                                            
        cursorTarget = null;                                                                // Reset the cursor target 
        foreach ( RaycastResult result in results )                                         // Check if any of the objects are interactable 
        {                                                                                   
            if ( result.gameObject.CompareTag( "UIButton" ) )                               // Only works with buttons tagged "UIButton" - done to ignore overlaid elements.
            {                                                                               
                cursorTarget = result.gameObject;                                           // Store a reference to the object.
                break;                                                                      // Stop looking after we found it.
            }                                                                               
        }                                                                                   
        if ( cursorTarget == previousTarget ) { return; }                                   // If still hovering over the same item, do nothing.
                                                                                            
        ExecuteEvents.Execute( previousTarget , ped , ExecuteEvents.pointerExitHandler );   // Hovering over something new. Clear previous hover. Simulate mouse rollout.
        previousTarget = cursorTarget;                                                      // Current element get stored in previousTarget, for future comparisons.
                                                                                            
        if ( cursorTarget == null ) { return; }                                             // Not hovering over anything; don't need to do anything else.
                                                                                            
        // Debug.Log( "Hit " + cursorTarget.name );                                         // Check what we're colliding with.
        if ( cursorTarget.CompareTag( "UIButton" ) )                                        // Interactable buttons must use this tag.
        {
            ExecuteEvents.Execute( cursorTarget , ped , ExecuteEvents.pointerEnterHandler );// We're hovering over a button. Simulate mouse rollover.
        }
    }

    public void OnButtonStateChange( int button , bool state )
    {
        // Debug.Log( "Button " + button + " state changed to: " + state );                 // Check what button is being pressed.
        if( cursorTarget != null )                                                          // Make sure we're clicking on something.
        {
            ExecuteEvents.Execute( cursorTarget , ped , ExecuteEvents.submitHandler );      // Simulate click event.
        }
    }

}
