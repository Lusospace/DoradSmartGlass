using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.XR.Cardboard;
using TMPro;

public class Startup : MonoBehaviour
{
    //private CardboardStartup cardboardStartup;
    public TMP_Text screenDebug;
    // Start is called before the first frame update
    void Awake()
    {
        //cardboardStartup = new CardboardStartup();
    }
    void Start()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        Debug.Log("Screen Resolution: " + screenWidth + "x" + screenHeight);
        screenDebug.text = "Screen Resolution: " + screenWidth + "x" + screenHeight;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.brightness = 1.0f;

        //FullScreenMode fulscreenmode = new FullScreenMode();

        Screen.SetResolution(screenWidth, screenHeight, FullScreenMode.ExclusiveFullScreen, 0);
        //Api.SaveDeviceParams("http://google.com/cardboard/cfg?p=CglMdXNvU3BhY2USCkRvcmFkR2xhc3MdMQgsPSWPwnU9KhAAAAAAAAAAAAAAAAAAAAAAWAA1KVwPPToICtcjPArXIzxQAGAA");

        // Checks if the device parameters are stored and scans them if not.
        if (!Api.HasDeviceParams())
        {
            Api.ScanDeviceParams();
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (Api.IsGearButtonPressed)
        {
            Api.ScanDeviceParams();
        }

        if (Api.IsCloseButtonPressed)
        {
            Application.Quit();
        }

        if (Api.IsTriggerHeldPressed)
        {
            Api.Recenter();
        }

        if (Api.HasNewDeviceParams())
        {
            Api.ReloadDeviceParams();
        }
#if !UNITY_EDITOR
        Api.UpdateScreenParams();
        //Api.Recenter();
#endif
    }
}
