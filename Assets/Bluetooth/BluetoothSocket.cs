using UnityEngine;

namespace Assets.Bluetooth
{
    public class BluetoothSocket
    {
        AndroidJavaObject JavaObject;
        private bool isConnected = false;

        public BluetoothSocket(AndroidJavaObject obj)
        {
            JavaObject = obj;
        }

        public void connect()
        {
            
            JavaObject.Call("connect");
            isConnected = true;
            
            
        }

        public void close()
        {
            JavaObject.Call("close");
            isConnected = false;
        }
        public bool IsConnected()
        {
            return isConnected; // Return the connection state
        }

        public void SetIsConnected(bool val)
        {
            isConnected = val;
        }

        public BtStream getInputStream()
        {
            var istream = JavaObject.Call<AndroidJavaObject>("getInputStream");
            return new BtStream(istream, true);
        }

        public BtStream getOutputStream()
        {
            var ostream = JavaObject.Call<AndroidJavaObject>("getOutputStream");
            return new BtStream(ostream, false);
        }

    }
}
