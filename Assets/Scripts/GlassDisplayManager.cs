using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassDisplayManager : MonoBehaviour
{
    public Camera cameraC;
    public Camera cameraL;
    public Camera cameraR;
    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Debug.Log("displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // Check if additional displays are available and activate each.

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.C))
        {
            //Swap enabled state to opposite one provided that only is on at a time
            
            camera2.enabled = !camera2.enabled;
        }*/
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Center Camera");
            //Camera.main.enabled = !Camera.main.enabled;
            cameraC.enabled = !cameraC.enabled;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Left Camera");
            cameraL.enabled = !cameraL.enabled;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Right Camera");
            cameraR.enabled = !cameraR.enabled;
        }
    }
}
