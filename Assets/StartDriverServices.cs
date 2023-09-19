using UnityEngine;
using UnityEngine.Android;

public class StartDriverServices : MonoBehaviour
{
    // Function to call the Java method
    public void CallStartService()
    {
        Debug.Log("Calling StartService");
        AndroidJavaClass javaClass = new AndroidJavaClass("com.example.ssd1333test.StartForegroundService");
        javaClass.CallStatic("startService", new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"));
    }

    public void CallSendImagetoService()
    {
        Debug.Log("Calling SendImagetoService");
        AndroidJavaClass javaClass = new AndroidJavaClass("com.example.ssd1333test.StartForegroundService");
        javaClass.CallStatic("sendImagetoService", new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"));
    }

    void Start()
    {
        // Request necessary permissions for starting services (if not already granted)

        CallStartService();
        CallSendImagetoService();
    }
}
