/********************************************************************
	created:  2019-11-03 00:00:24
	filename: AndroidBLEDevice.cs
	author:	  songguangze@outlook.com
	
	purpose:  Android 蓝牙4.0 设备
*********************************************************************/
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class AndroidBLEDevice : IBluetoothDevice
{
    private AndroidJavaObject m_androidBridgeInstance = null;
    private AndroidBluetoothMessageHandler m_internalBluetoothMessageHandler = null;

    public void ConnectToDevice(string remoteDeviceMacAddress)
    {
        AndroidBridgeInstance.Call<bool>("connectDevice", remoteDeviceMacAddress);
    }

    public void Disconnect()
    {
        AndroidBridgeInstance.Call("disconnect");
    }

    public BluetoothStatus GetBluetoothDeviceStatus()
    {
        BluetoothStatus bluetoothStatus = (BluetoothStatus)AndroidBridgeInstance.Call<int>("getCurrentBluetoothStatus");
        return bluetoothStatus;
    }

    public string GetConnectedDeviceName()
    {
        throw new System.NotImplementedException();
    }

    public List<BluetoothDeviceInfo> GetPariedDevices()
    {
        string bondDeviceList = AndroidBridgeInstance.Call<string>("getPariedDevices");
        List<BluetoothDeviceInfo> result = JsonConvert.DeserializeObject<List<BluetoothDeviceInfo>>(bondDeviceList);

        return result;
    }

    public void InitializeBluetoothDevice()
    {
        m_internalBluetoothMessageHandler = new AndroidBluetoothMessageHandler();
        AndroidBridgeInstance.Call("initializeBluetoothForUnity", m_internalBluetoothMessageHandler);

    }

    public bool IsBluetoothEnabled()
    {
        return AndroidBridgeInstance.Call<bool>("isEnabled");
    }

    public void SearchDevices()
    {
        AndroidBridgeInstance.Call("searchDevices", true);
    }

    public void SendData(List<byte> sendBuffer)
    {
        AndroidBridgeInstance.Call("sendData", sendBuffer.ToArray());
    }

    public void Tick()
    {
        //主线程中处理消息
        if (this.m_internalBluetoothMessageHandler != null)
            this.m_internalBluetoothMessageHandler.Tick();
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