using UnityEngine;

public static class LogcatLogger
{
    public static void Log(string message)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass logClass = new AndroidJavaClass("android.util.Log"))
            {
                logClass.CallStatic<int>("i", "Unity", message);
            }
        }
        else
        {
            Debug.Log(message);
        }
    }
}
