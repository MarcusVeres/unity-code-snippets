using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindComponentsInHierarchy : MonoBehaviour
{
    // public string componentName = "";

    // TODO :: Dynamic name 
    // List<Canvas> components = new List<Canvas>();
    List<UltimateRadialMenuInputManager> components = new List<UltimateRadialMenuInputManager>();


    // Start is called before the first frame update
    void Start()
    {
        
        transform.root.transform.GetComponentsInChildren( components );
        
        if( components.Count > 0 )
        {
            for ( int i = 0; i < components.Count; i++ )
            {
                Debug.Log( "Component(s) found: " + components[ i ].name );
            }
        }
        else
        {
            Debug.LogWarning( "No components were found." );
        }

    }

}
