using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Assets.Bluetooth
{
    public class BtStream : System.IO.Stream
    {        
        AndroidJavaObject JavaObject;
        bool IsInputStream;
        public string message = null;
        // Define an event for data reception
        public event EventHandler<string> DataReceived;

        private enum ReadState
        {
            WaitingForHeader,
            ReceivingData,
        }

        private ReadState readState = ReadState.WaitingForHeader;
        private List<byte> receivedDataList = new List<byte>();
        private string header = "DoradHeader";
        private string footer = "DoradFooter";


        override public bool CanRead { get { return IsInputStream; } }
        override public bool CanSeek { get { return false; } }
        override public bool CanWrite { get { return !IsInputStream; } }
        override public long Length { get { throw new NotSupportedException(); } }
        override public long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public BtStream(AndroidJavaObject obj, bool IsInputStream)
        {
            JavaObject = obj;
            this.IsInputStream = IsInputStream;
        }

        override public void Flush()
        {
            /*
             * we don't do any buffering, so
             * nothing to do here
             */
        }

        override public int Read(byte[] buffer, int offset, int count)
        {
            if (!IsInputStream)
            {
                throw new NotSupportedException();
            }

            /*
             * TODO: investigate if it's possible to make the
             * byte array buffer managment more efficient
             *
             * Right now for each read call we:
             *  - create new temp JNI array
             *  - use that when calling Java's InputStream.read() method
             *  - convert that temp array to managed array
             *  - copy from the converted array to callers provided buffer array
             *
             * This involved a lot of allocations and memory copy operations,
             * perhaps there is a way to directly write at the right memory location
             * in the caller provided buffer array.
             */
            //buffer = new byte[2048];
            //count = 2048;
            /*try
            {*/
                var jniBuffer = AndroidJNI.NewSByteArray(count);
                jvalue[] args = new jvalue[3];
                args[0].l = jniBuffer;
                args[1].i = 0;
                args[2].i = count;

                IntPtr methodId = AndroidJNIHelper.GetMethodID(
                    JavaObject.GetRawClass(),
                    "read", "([BII)I");
                
                var r = AndroidJNI.CallIntMethod(
                    JavaObject.GetRawObject(),
                    methodId,
                    args);

                if (r > 0)
                {
                    var manBuff = AndroidJNI.FromSByteArray(jniBuffer);

                    // Convert sbyte array to byte array
                    byte[] byteBuff = new byte[manBuff.Length];
                    for (int i = 0; i < manBuff.Length; i++)
                    {
                        byteBuff[i] = (byte)manBuff[i];
                    }
                    Array.Copy(byteBuff, 0, buffer, offset, count);
                    string receivedData = Encoding.UTF8.GetString(buffer, offset, count);

                    //Debug.Log(message);
                    // Raise the DataReceived event with the received data

                    if (receivedData.Contains(header))
                    {
                        if (receivedData.Contains(footer) && receivedData.Contains(header))
                        {
                            message = null;
                            readState = ReadState.ReceivingData;
                            message += (receivedData.TrimEnd('\0')).TrimStart(header);
                            message = message.TrimEnd(footer);
                            readState = ReadState.WaitingForHeader;
                            OnDataReceived(message);
                        }
                        else
                        {
                            message = null;
                            readState = ReadState.ReceivingData;
                            message += (receivedData.TrimEnd('\0')).TrimStart(header);
                        }
                    }

                    else if (readState == ReadState.ReceivingData)
                    {
                        if (receivedData.Contains(footer))
                        {
                            message += (receivedData.TrimEnd('\0')).TrimEnd(footer);
                            readState = ReadState.WaitingForHeader;
                            OnDataReceived(message);
                        }
                        else
                        {
                            message += receivedData.TrimEnd('\0');
                        }

                    }
                    
                }
                else
                {
                    Debug.Log("Bluetooth communication");
                    LogcatLogger.Log("Bluetooth communication");
                }
                return r;

                /*}
                catch (Exception e)
                {
                    return 0;
                }*/

            }



        override public void Write(byte[] buffer, int offset, int count)
        {
            if (IsInputStream)
            {
                throw new NotSupportedException();
            }

            var jniBuffer = AndroidJNIHelper.ConvertToJNIArray(buffer);
            jvalue[] args = new jvalue[3];
            args[0].l = jniBuffer;
            args[1].i = offset;
            args[2].i = count;

            IntPtr methodId = AndroidJNIHelper.GetMethodID(
                JavaObject.GetRawClass(),
                "write", "([BII)V");

            AndroidJNI.CallVoidMethod(
                JavaObject.GetRawObject(),
                methodId,
                args);
        }

        override public long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        override public void SetLength(long value)
        {
            throw new NotSupportedException();
        }
        // Helper method to raise the DataReceived event
        protected virtual void OnDataReceived(string data)
        {
            DataReceived?.Invoke(this, data);
        }
    }
}
