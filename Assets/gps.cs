using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gps : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text text;
    void Start()
    {
        if (Input.location.isEnabledByUser)
        {
            Input.location.Start();
        }
        else
        {
            DisplayError("GPS is not enabled on the device.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.location.status != LocationServiceStatus.Running)
        {
            DisplayError("There is a problem with the location service: " + Input.location.status);
            return;
        }

        float latitude = Input.location.lastData.latitude;
        float longitude = Input.location.lastData.longitude;
        text.text = "Latitude: " + latitude + ", Longitude: " + longitude;
    }
    private void DisplayError(string message)
    {
        Debug.LogError(message);
        LogcatLogger.Log(message);
        text.text = message;
    }
}
