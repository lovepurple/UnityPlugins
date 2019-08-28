/**
 *  Android 经典蓝牙2。0通讯
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidBluetoothClassicDevice : IBluetoothDevice
{
    //Android 输出的jar的BundleName
    private const string NATIVE_PLUGIN_BUNDLE_NAME = "com.lovepurple.btccontroller";
    private const string ANDROID_STRING_CALLBACK_INTERFACE = "com.lovepurple.btccontroller.UnityCallback";
    private const string ANDROID_BYTES_CALLBACK_INTERFACE = "com.lovepurple.btccontroller.UnityBufferCallback";

    //Unity Native Bridge
    private static AndroidJavaObject m_androidBridgeInstance = null;

    //接口中的事件在实现里需要再声明
    public event Action<byte[]> OnReceiveDataEvent;
    public event Action<string> OnErrorEvent;
    public event Action OnConnectedEvent;

    /// <summary>
    /// 
    /// </summary>
    public bool IsBluetoothEnabled()
    {
        return AndroidBridgeInstance.Call<bool>("isEnabled");
    }

    public void InitializeBluetoothDevice()
    {
        AndroidStringCallback androidStringCallback = new AndroidStringCallback(errorMessage =>
        {
            OnErrorEvent?.Invoke(errorMessage);
        }, ANDROID_STRING_CALLBACK_INTERFACE);
        AndroidBridgeInstance.Call("initialBTCManager", androidStringCallback);

        AndroidBufferCallback onReceiveBufferCallback = new AndroidBufferCallback(bufferData =>
        {
            OnReceiveDataEvent?.Invoke(bufferData);
        }, ANDROID_BYTES_CALLBACK_INTERFACE);
        AndroidBridgeInstance.Call("setOnReceiveMessageCallback", onReceiveBufferCallback);
    }

    public void GetPariedDevices(Action<string> OnGetPariedDeviceCallback)
    {
        AndroidStringCallback androidStringCallback = new AndroidStringCallback(OnGetPariedDeviceCallback, ANDROID_STRING_CALLBACK_INTERFACE);
        AndroidBridgeInstance.Call("getPariedDevices", androidStringCallback);
    }

    public string GetConnectedDeviceName()
    {
        return AndroidBridgeInstance.Call<string>("getConnectedDeviceName");
    }

    public void SendData(byte[] sendBuffer)
    {
        AndroidBridgeInstance.Call("sendMessage", sendBuffer);
    }

    public BluetoothStatus GetBluetoothDeviceStatus()
    {
        BluetoothStatus bluetoothStatus = (BluetoothStatus)AndroidBridgeInstance.Call<int>("getCurrentBluetoothStatus");
        return bluetoothStatus;
    }

    public void ConnectToDevice(string remoteDeviceMacAddress)
    {
        AndroidBridgeInstance.Call("conntectDevice", remoteDeviceMacAddress, new AndroidStringCallback((str) =>
        {
            OnConnectedEvent.Invoke();
        }, ANDROID_STRING_CALLBACK_INTERFACE));
    }

    public void Disconnect()
    {
        AndroidBridgeInstance.Call("disconnectDevice");
    }

    private static AndroidJavaObject AndroidBridgeInstance
    {
        get
        {
            if (m_androidBridgeInstance == null)
            {
                AndroidJavaClass androidJavaClass = AndroidNativeUtility.GetAndroidNativeClass("com.lovepurple.btccontroller.BTCManager");

                //getInstance时需要传入activity
                AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");

                m_androidBridgeInstance = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", activity);
            }
            return m_androidBridgeInstance;
        }
    }
}
