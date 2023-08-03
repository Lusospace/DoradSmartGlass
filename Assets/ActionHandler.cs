using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    private float lastPressTime = 0f;
    private int pressCount = 0;
    private float doublePressInterval = 0.5f; // Adjust this value for your desired double press interval
    private float triplePressInterval = 1f;   // Adjust this value for your desired triple press interval

    void Update()
    {
        // Check if the device is Android
        if (Application.platform == RuntimePlatform.Android)
        {
            // Use Input.GetAxis("Vertical") to capture volume button presses
            float volumeAxis = Input.GetAxis("Vertical");

            if (volumeAxis < 0)
            {
                // Volume down button pressed
                HandleButtonPress();
            }
        }
        else
        {
            // Use spacebar for testing in the Unity Editor
            if (Input.GetKeyDown(KeyCode.Space))
            {
                  HandleButtonPress();
            }
        }
    }

    private void HandleButtonPress()
    {
        float timeSinceLastPress = Time.time - lastPressTime;

        if (timeSinceLastPress < doublePressInterval)
        {
            // Double press detected
            pressCount = 2;
        }
        else if (timeSinceLastPress < triplePressInterval)
        {
            // Triple press detected
            pressCount = 3;
        }
        else
        {
            // Single press detected
            pressCount = 1;
        }

        // Update the last press time
        lastPressTime = Time.time;

        // Handle the different types of button presses
        switch (pressCount)
        {
            case 1:
                Debug.Log("Single Press!");
                this.GetComponentInParent<LocationTracker>().SetActivity(false);
                Debug.Log("Location Tracker stopped");
                LogcatLogger.Log("Location Tracker stopped");
                break;
            case 2:
                Debug.Log("Double Press!");
                break;
            case 3:
                Debug.Log("Triple Press!");
                break;
        }
    }
}
