using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Attach this script to a GameObject in your Unity scene

public class GPSManager : MonoBehaviour
{
    public TMP_Text latitudeText;
    public TMP_Text longitudeText;
    public TMP_Text accuracyText;

    public float accuracyThreshold = 10f; // Customize the accuracy threshold for triggering an action

    private bool isLocationInitialized = false;

    void Start()
    {
        Input.location.Start();
        // Check if GPS is available on the device
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS is not enabled on the device.");
            return;
        }

        // Start the location service
        
        isLocationInitialized = true;
    }

    void Update()
    {
        // Check if location service is running and initialized
        if (isLocationInitialized && Input.location.status == LocationServiceStatus.Running)
        {
            // Get the current GPS location data
            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;
            float accuracy = Input.location.lastData.horizontalAccuracy;

            // Update the UI text fields with the GPS data
            latitudeText.text = "Latitude: " + latitude.ToString();
            longitudeText.text = "Longitude: " + longitude.ToString();
            accuracyText.text = "Accuracy: " + accuracy.ToString() + " m";

            // Check if accuracy meets the threshold for triggering an action
            if (accuracy <= accuracyThreshold)
            {
                PerformAction(); // Call a custom method to perform the desired action
            }
        }
    }

    private void OnDisable()
    {
        // Stop the location service when the script is disabled or the application is closed
        if (isLocationInitialized)
        {
            Input.location.Stop();
            isLocationInitialized = false;
        }
    }

    private void PerformAction()
    {
        Debug.Log("GPS accuracy threshold reached. Performing action...");
        // Implement your custom logic or action here based on the GPS data
    }
}

