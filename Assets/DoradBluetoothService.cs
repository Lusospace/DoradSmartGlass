﻿using Assets.Bluetooth;
using Assets.Utilities;
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

    // Define a delegate for the DataReceived event
    public delegate void DataReceivedHandler(object sender, string data);

    // Define the DataReceived event using the delegate
    public event DataReceivedHandler DataReceived;

    void Start()
    {
        //deviceName = "Redmi Note 11";
        deviceName = "C1_Max";

        // Assuming you have already initialized the 'btStream' variable
        
        //createBluetoothConnection();
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

            socket = btDevice.createRfcommSocketToServiceRecord(uuid);
            Debug.Log("socket " + socket);

            socket.connect();

            istream = socket.getInputStream();
            // Move the event subscription here, after istream is assigned
            istream.DataReceived += OnDataReceived;
            /*if (!isListening)
            {
                istream.DataReceived += OnDataReceived;
                isListening = true;
            }*/

            reader = new StreamReader(istream);

            Debug.Log("wrote");
        }
        catch (System.Exception e)
        {
            reader = null;
            Debug.Log("Start Exception: " + e);
            socket.SetIsConnected(false);
            socket.close();
        }
        //}
    }
    public void SendData(string msg)
    {
        ostream = socket.getOutputStream();
        var writer = new StreamWriter(ostream);

        Debug.Log("writer " + writer);

        writer.Write(msg);
        writer.Flush();
           
    }

    void Update()
    {
        // Check the connection status and try to reconnect if necessary
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
        }
        catch (System.Exception e)
        {
            Debug.Log("OnDestroy Exception: " + e);
        }
    }

    
}