using Assets.Bluetooth;
using Assets.Utilities;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoradBluetoothService : MonoBehaviour
{
    public const int STATE_NONE = 0;
    public const int STATE_LISTEN = 1;
    public const int STATE_CONNECTING = 2;
    public const int STATE_CONNECTED = 3;

    public StreamReader reader = null;
    BtStream istream;
    BtStream ostream;
    private string deviceName;
    BluetoothSocket socket;
    string receivedmsg { get; set; }
    bool isListening = false;
    public TMP_Text pairingText;
    // Define a delegate for the DataReceived event
    public delegate void DataReceivedHandler(object sender, string data);

    // Define the DataReceived event using the delegate
    public event DataReceivedHandler DataReceived;


    private Coroutine readCoroutine;
    void Start()
    {
        //deviceName = "Redmi Note 11";
        deviceName = "View2 Go";
        //deviceName = "C1_Max";
    }
    public void CreateBluetoothConnection()
    {
        var btAdapter = BluetoothAdapter.getDefaultAdapter();
        Debug.Log("btAdapter.isEnabled() = {0}" + btAdapter.isEnabled());

        BluetoothDevice btDevice = null;
        //if (btAdapter.isEnabled() != false) { 
            foreach (var device in btAdapter.getBondedDevices())
            {

                Debug.Log("bond " + device.getName());
                if (device.getName() == deviceName)
                {
                    Debug.Log("found device");
                    btDevice = device;
                    break;
                }
            }

            if (btDevice == null)
            {
                Debug.Log("No btDevice device found");
                return;
            }

            try
            {
                var uuid = UUID.fromString("fa87c0d0-afac-11de-8a39-0800200c9a66");
                Debug.Log("uuid " + uuid);
                try
                {
                    socket = btDevice.createRfcommSocketToServiceRecord(uuid);
                }
                catch (System.Exception e)
                {
                    Debug.Log("socket " + e);
                }
                Debug.Log("socket " + socket);

                socket?.connect();

                istream = socket.getInputStream();
                reader = new StreamReader(istream);
                istream.DataReceived += OnDataReceived;
                pairingText.text = "Waiting for configuration, connected to " + deviceName;
                Debug.Log("wrote");
            }
            catch (System.Exception e)
            {
                reader = null;
                Debug.Log("Start Exception: " + e);
                socket.SetIsConnected(false);
                try
                {
                    socket?.close();
                }
                catch (System.Exception ex)
                {

                    Debug.Log("Start Exception: " + ex);
                }
            }
        //}
    }
    public void SendData(string msg)
    {
        ostream = socket?.getOutputStream();
        var writer = new StreamWriter(ostream);

        Debug.Log("writer " + writer);

        writer.Write(msg);
        writer.Flush();

    }

    void Update()
    {
        /*// Check the connection status and try to reconnect if necessary
        if (socket == null || !IsConnected())
        {
            //socket.SetIsConnected(false);
            Debug.Log("Connection lost or not established. Reconnecting...");
            reader = null;
            CreateBluetoothConnection();
        }
        else
        {
            reader?.ReadLine();
        }*/
    }
    public void StartReadCouroutine()
    {
        try
        {
            readCoroutine = StartCoroutine(ReadCoroutine());
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
        
    }
    private IEnumerator ReadCoroutine()
    {

            float interval = 0.1f; // Adjust this interval as needed
            while (true)
            {
                // Check the connection status and try to reconnect if necessary
                if (socket == null || !IsConnected())
                {
                    Debug.Log("Connection lost or not established. Reconnecting...");
                    reader = null;
                    CreateBluetoothConnection();
                }
                else
                {
                    
                    try
                    {
                        //reader?.ReadLine();
                        if (reader != null)
                        Debug.Log("line: " + reader.ReadLine());
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Read Exception: " + e);
                    }

                }

            // Wait for a specific interval before continuing
            yield return new WaitForSeconds(interval);
        }
        
    } 
     

    void OnDataReceived(object sender, string data)
    {
        // Handle the received data here
        receivedmsg = data;
        Debug.Log("Received data: " + data);

        // Raise the DataReceived event to notify subscribers (e.g., ConfigurationManager)
        DataReceived?.Invoke(sender, data);
    }
    // Check if the Bluetooth socket is connected
    bool IsConnected()
    {
        if (socket != null)
        {
            try
            {
                return socket.IsConnected();
            }
            catch (System.Exception e)
            {
                Debug.Log("IsConnected Exception: " + e);
                socket.SetIsConnected(false);
            }
        }
        return false;
    }
    public string GetReceivedMsg()
    {
        return receivedmsg;
    }
    void OnDestroy()
    {
        // Close the reader and the socket when the service is destroyed.
        try
        {
            // Unsubscribe from the event when this GameObject is destroyed
            istream.DataReceived -= OnDataReceived;
            isListening = false;
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }

            if (ostream != null)
            {
                ostream.Close();
                ostream.Dispose();
            }

            if (socket != null)
            {
                socket.SetIsConnected(false);
                socket.close();
                socket = null;
                
            }
            if (readCoroutine != null)
            {
                StopCoroutine(readCoroutine);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("OnDestroy Exception: " + e);
        }
    }

    
}
