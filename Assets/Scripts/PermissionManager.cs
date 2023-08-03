using UnityEngine;
using System;

public class PermissionManager : MonoBehaviour
{
    void Start()
    {
#if !UNITY_EDITOR
        // Check if permission is already granted
        if (!HasPermission("android.permission.WRITE_EXTERNAL_STORAGE"))
        {
            // Activate permission without user confirmation
            ActivatePermission("android.permission.WRITE_EXTERNAL_STORAGE");
        }
        if (!HasPermission("android.permission.BLUETOOTH"))
        {
            // Activate permission without user confirmation
            ActivatePermission("android.permission.BLUETOOTH");
        }
        if (!HasPermission("android.permission.BLUETOOTH_ADMIN"))
        {
            // Activate permission without user confirmation
            ActivatePermission("android.permission.BLUETOOTH_ADMIN");
        }
       
        if (!HasPermission("android.permission.BLUETOOTH_SCAN"))
        {
            // Activate permission without user confirmation
            ActivatePermission("android.permission.BLUETOOTH_SCAN");
        }
        if (!HasPermission("android.permission.BLUETOOTH_ADVERTISE"))
        {
            // Activate permission without user confirmation
            ActivatePermission("android.permission.BLUETOOTH_ADVERTISE");
        }
        if (!HasPermission("android.permission.BLUETOOTH_CONNECT"))
        {
            // Activate permission without user confirmation
            ActivatePermission("android.permission.BLUETOOTH_CONNECT");
        }
        if (!HasPermission("android.permission.BLUETOOTH_CONNECT"))
        {
            // Activate permission without user confirmation
            ActivatePermission("android.permission.BLUETOOTH_CONNECT");
        }
        if (!HasPermission("android.permission.ACCESS_COARSE_LOCATION"))
        {
            // Activate permission without user confirmation
            ActivatePermission("android.permission.ACCESS_COARSE_LOCATION");
        }
        if (!HasPermission("android.permission.ACCESS_FINE_LOCATION"))
        {
            // Activate permission without user confirmation
            ActivatePermission("android.permission.ACCESS_FINE_LOCATION");
        }
        if (!HasPermission("com.google.android.things.permission.USE_PERIPHERAL_IO"))
        {
            // Activate permission without user confirmation
            ActivatePermission("com.google.android.things.permission.USE_PERIPHERAL_IO");
        }
        if (!HasPermission("android.permission.WAKE_LOCK"))
        {
            // Activate permission without user confirmation
            ActivatePermission("android.permission.WAKE_LOCK");
        }
        if (!HasPermission("android.permission.BLUETOOTH_PRIVILEGED"))
        {
            // Activate permission without user confirmation
            ActivatePermission("android.permission.BLUETOOTH_PRIVILEGED");
        }
#endif
    }

    bool HasPermission(string permission)
    {
        try
        {
            // Get the current application context
            AndroidJavaClass contextClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context = contextClass.GetStatic<AndroidJavaObject>("currentActivity");

            // Check if the permission is granted
            int permissionStatus = context.Call<int>("checkSelfPermission", permission);
            return permissionStatus == 0;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    void ActivatePermission(string permission)
    {
        try
        {
            // Get the current application context
            AndroidJavaClass contextClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context = contextClass.GetStatic<AndroidJavaObject>("currentActivity");

            // Call the permission activation method
            context.Call("requestPermissions", new string[] { permission }, 1);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
