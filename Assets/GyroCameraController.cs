using UnityEngine;

public class GyroCameraController : MonoBehaviour
{
    private Quaternion initialRotation;
    public float sensitivity = 1.0f;
    private Gyroscope gyro;

    void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            // Set the initial rotation to match the camera's current orientation
            initialRotation = transform.rotation;
        }
    }

    void Update()
    {
        if (gyro != null)
        {
            // Apply rotation based on the gyroscope input
            Quaternion gyroRotation = gyro.attitude;
            Quaternion adjustedRotation = (initialRotation * gyroRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, adjustedRotation, sensitivity * Time.deltaTime);
        }
    }
}
