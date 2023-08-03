using UnityEngine;

public class BluetoothAccessory : MonoBehaviour
{
    private const string AccessoryClass = "android.hardware.usb.action.USB_ACCESSORY_ATTACHED";
    private const string PackageManagerClass = "android.content.pm.PackageManager";

    private AndroidJavaObject currentActivity;
    private AndroidJavaObject packageManager;

    private void Start()
    {
        // Get the current activity
        currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");

        // Get the package manager
        packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

        // Check if the accessory class is supported
        if (packageManager.Call<bool>("hasSystemFeature", AccessoryClass))
        {
            // Define the accessory
            var accessoryFilter = new AndroidJavaObject("android.content.IntentFilter", AccessoryClass);
            AndroidJavaObject componentName = new AndroidJavaObject("android.content.ComponentName", Application.identifier, ".MainActivity");
            int stateEnabled = new AndroidJavaClass("android.content.pm.PackageManager").GetStatic<int>("COMPONENT_ENABLED_STATE_ENABLED");
            AndroidJavaClass packageManagerClass = new AndroidJavaClass("android.content.pm.PackageManager");
            packageManagerClass.CallStatic("setComponentEnabledSetting", componentName, stateEnabled, 0);

            // Register the accessory
            currentActivity.Call("registerReceiver", new BluetoothAccessoryReceiver(), accessoryFilter);
        }
    }

    private class BluetoothAccessoryReceiver : AndroidJavaProxy
    {
        public BluetoothAccessoryReceiver() : base("android.content.BroadcastReceiver") { }

        public void onReceive(AndroidJavaObject context, AndroidJavaObject intent)
        {
            Debug.Log("Accessory attached!");
        }
    }
}
