using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

public class AccelerationToVelocity : MonoBehaviour
{
    private Vector3 lastAcceleration;
    private float lastTime;
    private float velocity;
    public TMP_Text mps;

    void Start()
    {
        lastAcceleration = Input.acceleration;
        lastTime = Time.time;
    }
    private void OnEnable()
    {
        InputSystem.EnableDevice(Accelerometer.current);
        InputSystem.EnableDevice(Gyroscope.current);
        InputSystem.EnableDevice(MagneticFieldSensor.current);
    }
    private void OnDisable()
    {
        InputSystem.DisableDevice(Accelerometer.current);
        InputSystem.DisableDevice(Gyroscope.current);
        InputSystem.DisableDevice(MagneticFieldSensor.current);
    }

    void Update()
    {
        Vector3 acceleration = Accelerometer.current.acceleration.ReadValue();
        float time = Time.time;
        float deltaTime = time - lastTime;
        lastTime = time;

        velocity += (lastAcceleration.y + acceleration.y) / 2f * deltaTime;
        lastAcceleration = acceleration;

        float speed = velocity * Time.deltaTime;
        Debug.Log("Speed: " + speed + " m/s");
        mps.text = "Speed: " + speed + " m/s";
    }
}
