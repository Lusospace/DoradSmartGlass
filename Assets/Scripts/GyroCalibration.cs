using UnityEngine;

public class GyroCalibration : MonoBehaviour
{
    private bool isCalibrating = false;
    private Quaternion gyroOffset;

    private void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            isCalibrating = true;
        }
        else
        {
            Debug.LogError("Gyroscope is not supported on this device!");
        }
    }

    private void Update()
    {
        if (isCalibrating)
        {
            CalibrateGyro();
        }
        else
        {
            // Use the calibrated gyro data in your application
            Quaternion gyroRotation = gyroOffset * Input.gyro.attitude;
            // ...
        }
    }

    private void CalibrateGyro()
    {
        if (Input.gyro.enabled)
        {
            gyroOffset = Quaternion.Inverse(Input.gyro.attitude);
            isCalibrating = false;
        }
    }
}
