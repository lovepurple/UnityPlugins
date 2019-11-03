/********************************************************************
	created:  2019-11-03 00:00:24
	filename: AndroidBLEDevice.cs
	author:	  songguangze@outlook.com
	
	purpose:  Android 蓝牙4.0 设备
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

public class AndroidBLEDevice : IBluetoothDevice
{
    private AndroidJavaObject m_androidBridgeInstance = null;

    public void ConnectToDevice(string remoteDeviceMacAddress)
    {
        throw new System.NotImplementedException();
    }

    public void Disconnect()
    {
        throw new System.NotImplementedException();
    }

    public BluetoothStatus GetBluetoothDeviceStatus()
    {
        throw new System.NotImplementedException();
    }

    public string GetConnectedDeviceName()
    {
        throw new System.NotImplementedException();
    }

    public List<BluetoothDeviceInfo> GetPariedDevices()
    {
        throw new System.NotImplementedException();
    }

    public void InitializeBluetoothDevice()
    {

    }

    public bool IsBluetoothEnabled()
    {
        return AndroidBridgeInstance.Call<bool>("isEnabled");
    }

    public void SearchDevices()
    {
        throw new System.NotImplementedException();
    }

    public void SendData(List<byte> sendBuffer)
    {
        throw new System.NotImplementedException();
    }

    public void Tick()
    {
        throw new System.NotImplementedException();
    }

    public AndroidJavaObject AndroidBridgeInstance
    {
        get
        {
            if (m_androidBridgeInstance == null)
            {
                AndroidJavaClass androidJavaClass = AndroidNativeUtility.GetAndroidNativeClass("com.lovepurple.blecontroller.BLEManager");

                AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");

                m_androidBridgeInstance = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", activity);
            }

            return m_androidBridgeInstance;
        }
    }
}