using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Android;

public class ConnectionManager : MonoBehaviour
{
    public TMP_Text msg;
    string device_name;

    // Start is called before the first frame update
    void Start()
    {
        Bluetooth.Instance().EnableBluetooth();
        Bluetooth.Instance().Discoverable();
        device_name = Bluetooth.Instance().DeviceName();
        
        //BluetoothDevice device = BluetoothAdapter.DefaultAdapter.GetRemoteDevice("00:11:22:33:44:55"); // replace with the device's MAC address

        // Set a PIN for the device
        string pin = "1234"; // replace with your desired PIN
        //device.SetPin(Encoding.ASCII.GetBytes(pin));

        // Confirm the pairing without user interaction
        //device.SetPairingConfirmation(true);
    }

    // Update is called once per frame
    void Update()
    {
        msg.text = "This device is now descoverable as :"+device_name;
    }
}
