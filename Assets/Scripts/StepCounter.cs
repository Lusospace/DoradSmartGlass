using UnityEngine;
using System.Collections;
using TMPro;
using System;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;
public class StepCounter : MonoBehaviour
{
    private AndroidJavaObject sensorManager;
    private AndroidJavaObject stepCounterSensor;
    private int stepCount;
    public TMP_Text stepText;

    void Start()
    {
       
        // Get the sensor manager and the step counter sensor
        AndroidJavaClass contextClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject context = contextClass.GetStatic<AndroidJavaObject>("currentActivity");
        sensorManager = context.Call<AndroidJavaObject>("getSystemService", "sensor");
        stepCounterSensor = sensorManager.Call<AndroidJavaObject>("getDefaultSensor", 19); // TYPE_STEP_COUNTER = 19
    }

    void OnEnable()
    {
        // Register for sensor updates
        sensorManager.Call("registerListener", new StepCounterListener(this), stepCounterSensor, 0);
    }

    void OnDisable()
    {
        // Unregister from sensor updates
        sensorManager.Call("unregisterListener", new StepCounterListener(this));
    }

    public void OnStepDetected()
    {
        // Update the step count and log it
        stepCount++;
        Debug.Log("Step count: " + stepCount);
        stepText.text = "Step count: " + stepCount;
    }
}

public class StepCounterListener : AndroidJavaProxy
{
    private StepCounter stepCounter;

    public StepCounterListener(StepCounter stepCounter) : base("android.hardware.SensorEventListener")
    {
        this.stepCounter = stepCounter;
    }

    public void onAccuracyChanged(AndroidJavaObject sensor, int accuracy)
    {
        // Not implemented
    }

    public void onSensorChanged(AndroidJavaObject eventObj)
    {
        int sensorType = Convert.ToInt32(eventObj.Call<int>("sensor"));

        if (sensorType == 19) // TYPE_STEP_COUNTER = 19
        {
            int stepCount = eventObj.Call<int>("values",0);
            stepCounter.OnStepDetected();
        }
    }
}

