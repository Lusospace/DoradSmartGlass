using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Bluetooth
{
    public class BluetoothServerSocket
    {
        static AndroidJavaClass _JavaClass = null;

        static AndroidJavaClass JavaClass
        {
            get
            {
                if (_JavaClass == null)
                {
                    _JavaClass = new AndroidJavaClass("android.bluetooth.BluetoothServerSocket");
                }
                return _JavaClass;
            }
        }

        AndroidJavaObject JavaObject;

        public BluetoothServerSocket(AndroidJavaObject ojb)
        {
            JavaObject = ojb;
        }

        public static BluetoothServerSocket Accept()
        {
            var obj = JavaClass.CallStatic<AndroidJavaObject>("accept");

            return new BluetoothServerSocket(obj);
        }

        public static BluetoothServerSocket Close()
        {
            var obj = JavaClass.CallStatic<AndroidJavaObject>("close");

            return new BluetoothServerSocket(obj);
        }
    }
}
