using UnityEngine;
using System;

public class BackgroundService : MonoBehaviour
{
    /*private static readonly string SERVICE_CHANNEL_ID = "com.LusoSpace.DoradGlass.service";
    private static readonly string SERVICE_CHANNEL_NAME = "DoradGlass App Background Service";

    private AndroidJavaObject service;

    void Start()
    {
        // Create notification channel for the service
        AndroidJavaClass notificationManager = new AndroidJavaClass("android.app.NotificationManager");
        AndroidJavaClass contextClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject context = contextClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject notificationChannel = new AndroidJavaObject("android.app.NotificationChannel", SERVICE_CHANNEL_ID, SERVICE_CHANNEL_NAME, 0);
        notificationManager.Call("createNotificationChannel", notificationChannel);

        // Create foreground service
        AndroidJavaClass serviceClass = new AndroidJavaClass("com.unity3d.player.UnityPlayerService");
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", context, serviceClass);
        context.Call("startForegroundService", intent);

        // Get service object
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter", SERVICE_CHANNEL_ID);
        AndroidJavaObject receiver = new AndroidJavaObject("com.unity3d.player.UnityPlayer$ActivityLifecycleCallbacks");
        activity.Call("registerReceiver", receiver, intentFilter);
        AndroidJavaObject serviceBinder = receiver.Call<AndroidJavaObject>("peekService", context, intent);
        service = serviceBinder.Call<AndroidJavaObject>("getService");
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // Start foreground service
            AndroidJavaClass contextClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context = contextClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass serviceClass = new AndroidJavaClass("com.unity3d.player.UnityPlayerService");
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", context, serviceClass);
            context.Call("startForegroundService", intent);
        }
    }

    void OnDestroy()
    {
        // Stop foreground service
        service.Call("stopForeground", true);
    }*/
}
