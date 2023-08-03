using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;
using UnityEngine.UI;

public class sensorTest : MonoBehaviour
{

    [SerializeField]
    public TMP_Text accelerometerField;
    [SerializeField]
    public TMP_Text gyroscopeField;
    [SerializeField]
    public TMP_Text magneticField;
    GameObject CamParent;
    Rigidbody RB;

    // Start is called before the first frame update
    void Start()
    {
        CamParent = new GameObject("Main Camera");
        Input.gyro.enabled = true;
        RB = gameObject.GetComponent<Rigidbody>();
    }
    Quaternion GyroToUnity(Quaternion quat)
    {
        return new Quaternion(quat.x, quat.z, quat.y, -quat.w);
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

    // Update is called once per frame
    void Update()
    {
        /*float interval = 1.0f;
        float timeElapsed = 0;
        timeElapsed += Time.deltaTime;
        while (timeElapsed >= interval)
        {*/
            //timeElapsed -= interval;
            accelerometerField.text = Accelerometer.current.acceleration.ReadValue().ToString();
            Debug.Log(Accelerometer.current.acceleration.ReadValue().ToString());
            gyroscopeField.text = Gyroscope.current.angularVelocity.ReadValue().ToString();
            Debug.Log(Gyroscope.current.angularVelocity.ReadValue().ToString());
            magneticField.text = MagneticFieldSensor.current.magneticField.ReadValue().ToString();
            Debug.Log(MagneticFieldSensor.current.magneticField.ReadValue().ToString());
        /*}*/
        

    }
    void Fixed_Update()
    {
        
       /* transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f));
        Vector3 relativeForce;
        if (yourQuat.eulerAngles.x > 315 || yourQuat.eulerAngles.x < 90)
            relativeForce = Vector3.forward;
        else if (yourQuat.eulerAngles.x < 315)
            relativeForce = Vector3.back;
        else
            relativeForce = Vector3.zero;

        RB.AddRelativeForce(relativeForce);*/
    }

    public class AccelerometerToVelocityConverter
    {
        private double _lastTimestamp;
        private double _lastAcceleration;
        private double _velocity;

        public void Convert(double timestamp, double acceleration)
        {
            // Calculate the time interval since the last reading
            double dt = timestamp - _lastTimestamp;

            // Calculate the average acceleration over the time interval
            double avgAcceleration = (_lastAcceleration + acceleration) / 2;

            // Integrate the acceleration to obtain the velocity
            _velocity += avgAcceleration * dt;

            // Update the timestamp and acceleration for the next reading
            _lastTimestamp = timestamp;
            _lastAcceleration = acceleration;
        }

        public double GetVelocity()
        {
            return _velocity;
        }
    }
}
