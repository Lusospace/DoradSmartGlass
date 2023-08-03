//#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCameraMovementInputs : MonoBehaviour
{

    [Header("Camera Movement Inputs")]
    [Tooltip("Sensitivity when using the mouse.")]
    public float mouseLookSensitivity = 1.0f;
    [Tooltip("Use Z and X to lean the Camera.")]
    public float leanSpeed = 2.0f;
    [Tooltip("Use W,A,S,D to move around. Q and E to move Up and Down.")]
    public float translationSpeed = 10.0f;
    [Tooltip("Lock Y axis to simulate FPS controls")]
    public bool lockYAxis = false;
    [Tooltip("Shortcut key to unlock Y Axis Lock")]
    public KeyCode lockYAxisTogglerKey = KeyCode.F12;
    [Tooltip("If Y axis is locked, used this value as the height")]
    public float lockYValue = 1.2f;
    [Tooltip("Locks the mouse pointer to the game window on right click.")]
    public bool lockMouseOnClick = true;

    private Camera _cam;
    private Pose _cameraPose;
    private Vector3 _eulerAngles;
    private bool _wasMouseDownLastFrame;
    private Vector3 _lastMousePosition;
    private float _initY;

    private static ARCameraMovementInputs _instance;

    public static ARCameraMovementInputs Instance { get { _instance = _instance == null ? FindObjectOfType(typeof(ARCameraMovementInputs)) as ARCameraMovementInputs : _instance; return _instance; } }


    void Start()
    {
#if !UNITY_EDITOR
			Destroy(this);
#endif
        _cam = Camera.main;
    }

    void Update()
    {
#if UNITY_EDITOR
        UpdateEmulation();
        ListenToggleYLock();
#endif
    }

    /// <summary>
    /// INTERNAL USE: Update the Tango emulation state for pose data.
    /// 
    /// Make sure this is only called once per frame.
    /// </summary>
    void UpdateEmulation()
    {
        float turnSpeed = 10f;
        var forward = _cam.transform.rotation * Vector3.forward;
        var right = _cam.transform.rotation * Vector3.right;
        var up = _cam.transform.rotation * Vector3.up;

        _eulerAngles = transform.localEulerAngles;

        float timeDeltaTime = Mathf.Clamp(Time.deltaTime, 0f, 0.02f);

        if (Input.GetKey(KeyCode.W))
            _cam.transform.position += forward * timeDeltaTime * translationSpeed;

        if (Input.GetKey(KeyCode.S))
            _cam.transform.position -= forward * timeDeltaTime * translationSpeed;

        if (Input.GetKey(KeyCode.A))
            _cam.transform.position -= right * timeDeltaTime * translationSpeed;

        if (Input.GetKey(KeyCode.D))
            _cam.transform.position += right * timeDeltaTime * translationSpeed;

        if (Input.GetKey(KeyCode.Q))
            _cam.transform.position -= up * timeDeltaTime * translationSpeed;

        if (Input.GetKey(KeyCode.E))
            _cam.transform.position += up * timeDeltaTime * translationSpeed;

        if (Input.GetKey(KeyCode.Z))
            _eulerAngles.z += timeDeltaTime * turnSpeed * leanSpeed;

        if (Input.GetKey(KeyCode.X))
            _eulerAngles.z -= timeDeltaTime * turnSpeed * leanSpeed;

        if (Input.GetKey(KeyCode.LeftArrow))
            _eulerAngles.y -= timeDeltaTime * turnSpeed * leanSpeed;
        if (Input.GetKey(KeyCode.RightArrow))
            _eulerAngles.y += timeDeltaTime * turnSpeed * leanSpeed;

        if (Input.GetMouseButton(1))
        {
            if (!_wasMouseDownLastFrame)
                _lastMousePosition = Input.mousePosition;

            if (lockMouseOnClick)
            {
                _eulerAngles.y += Input.GetAxis("Mouse X") * turnSpeed;
                _eulerAngles.x += -Input.GetAxis("Mouse Y") * turnSpeed;
            }
            else
            {
                var deltaPosition = Input.mousePosition - _lastMousePosition;
                _eulerAngles.y += timeDeltaTime * turnSpeed * deltaPosition.x * mouseLookSensitivity;
                _eulerAngles.x -= timeDeltaTime * turnSpeed * deltaPosition.y * mouseLookSensitivity;
            }

            _lastMousePosition = Input.mousePosition;
            _wasMouseDownLastFrame = true;

            if (lockMouseOnClick)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        else
        {
            _wasMouseDownLastFrame = false;
            Cursor.lockState = CursorLockMode.None;
        }

        if (lockYAxis)
        {
            _cam.transform.position = new Vector3(_cam.transform.position.x, Input.GetKey(KeyCode.LeftControl) ? lockYValue / 2f : lockYValue, _cam.transform.position.z);
        }
    }

    /// <summary>
    /// Shortcut to toggle Y position lock.
    /// </summary>
    private void ListenToggleYLock()
    {
        if (Input.GetKeyDown(lockYAxisTogglerKey))
            lockYAxis = !lockYAxis; 
    }

    private void LateUpdate()
    {
        //if (ARManager.Instance.FoundImageTarget == null)
        //{
        _cam.transform.rotation = Quaternion.Euler(_eulerAngles);
        //}
    }

    public Pose GetPose()
    {
        return _cameraPose;
    }

    public void SetPose(Pose pose)
    {
        _cameraPose = pose;
    }

    public Camera GetCamera()
    {
        return _cam;
    }

    public void SetCamera(Camera camera)
    {
        _cam = camera;
    }
}

//#endif