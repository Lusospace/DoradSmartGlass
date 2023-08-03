using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WidgetManager : MonoBehaviour
{
    public TMP_Text battery;
    public TMP_Text clock;
    public TMP_Text date;
    public GameObject Battery;
    public GameObject Clock;
    public GameObject Date;
    private GameObject _clock;
    private GameObject _date;
    private GameObject _battery;
    private TMP_Text _battery_text;
    private TMP_Text _date_text;
    private TMP_Text _clock_text;

    // Start is called before the first frame update
    void Start()
    {
        /*_clock = Instantiate(Clock);
        _battery = Instantiate(Battery);
        _date = Instantiate(Date);
        _clock_text = _clock.GetComponentInChildren<TMP_Text>();
        _date_text = _date.GetComponentInChildren<TMP_Text>();
        _battery_text = _date.GetComponentInChildren<TMP_Text>();*/
    }

    // Update is called once per frame
    void Update()
    {
        float percentage = 0;
#if UNITY_ANDROID
        percentage = GetBatteryLevel();
#endif
        battery.text = percentage.ToString()+ "%";
        //_battery_text.text = percentage.ToString() + "%";
        clock.text = System.DateTime.UtcNow.ToString("HH:mm:ss");
        //_clock_text.text = System.DateTime.UtcNow.ToString("HH:mm:ss");
        date.text = System.DateTime.UtcNow.ToString("dd/MM/yyyy");
        //_date_text.text = System.DateTime.UtcNow.ToString("dd/MM/yyyy");
    }
#if UNITY_ANDROID
    public static float GetBatteryLevel()
    {
#if UNITY_IOS
            UIDevice device = UIDevice.CurrentDevice();
            device.batteryMonitoringEnabled = true; // need to enable this first
            Debug.Log("Battery state: " + device.batteryState);
            Debug.Log("Battery level: " + device.batteryLevel);
            return device.batteryLevel*100;
#elif UNITY_ANDROID

        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    if (null != unityPlayer)
                    {
                        using (AndroidJavaObject currActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                        {
                            if (null != currActivity)
                            {
                                using (AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter", new object[] { "android.intent.action.BATTERY_CHANGED" }))
                                {
                                    using (AndroidJavaObject batteryIntent = currActivity.Call<AndroidJavaObject>("registerReceiver", new object[] { null, intentFilter }))
                                    {
                                        int level = batteryIntent.Call<int>("getIntExtra", new object[] { "level", -1 });
                                        int scale = batteryIntent.Call<int>("getIntExtra", new object[] { "scale", -1 });

                                        // Error checking that probably isn't needed but I added just in case.
                                        if (level == -1 || scale == -1)
                                        {
                                            return 50f;
                                        }
                                        return ((float)level / (float)scale) * 100.0f;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        return 100;
#endif
    }
#endif
}
