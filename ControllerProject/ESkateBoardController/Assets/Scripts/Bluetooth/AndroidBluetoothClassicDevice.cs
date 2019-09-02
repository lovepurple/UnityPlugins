/**
 *  Android 经典蓝牙2。0通讯
 */
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AndroidBluetoothClassicDevice : IBluetoothDevice
{
    //Android 输出的jar的BundleName
    private const string NATIVE_PLUGIN_BUNDLE_NAME = "com.lovepurple.btccontroller";
    private const string ANDROID_STRING_CALLBACK_INTERFACE = "com.lovepurple.btccontroller.UnityCallback";
    private const string ANDROID_BYTES_CALLBACK_INTERFACE = "com.lovepurple.btccontroller.UnityBufferCallback";
    private const string ANDROID_INT_CALLBACK_INTERFACE = "com.lovepurple.btccontroller.UnityIntCallback";

    //Unity Native Bridge
    private static AndroidJavaObject m_androidBridgeInstance = null;

    //接口中的事件在实现里需要再声明
    public event Action<byte[]> OnReceiveDataEvent;
    public event Action<string> OnErrorEvent;
    public event Action OnConnectedEvent;
    public event Action<int> OnBluetoothDeviceStateChangedEvent;
    public event Action<BluetoothDeviceInfo> OnSearchedDeviceEvent;
    public event Action<List<BluetoothDeviceInfo>> OnSearchFinishEvent;

    private Queue<byte[]> m_recvMessageQueue = new Queue<byte[]>();

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

        AndroidIntCallback androidIntCallback = new AndroidIntCallback(bluetoothStatus =>
        {
            OnBluetoothDeviceStateChangedEvent?.Invoke(bluetoothStatus);
        }, ANDROID_INT_CALLBACK_INTERFACE);


        AndroidBridgeInstance.Call("initialBTCManager", androidStringCallback, androidIntCallback);

        AndroidBufferCallback onReceiveBufferCallback = new AndroidBufferCallback(bufferData =>
        {
            this.m_recvMessageQueue.Enqueue(bufferData);
        }, ANDROID_BYTES_CALLBACK_INTERFACE);
        AndroidBridgeInstance.Call("setOnReceiveMessageCallback", onReceiveBufferCallback);
    }


    public List<BluetoothDeviceInfo> GetPariedDevices()
    {
        string bondDeviceList = AndroidBridgeInstance.Call<string>("getPariedDevices");
        List<BluetoothDeviceInfo> result = JsonConvert.DeserializeObject<List<BluetoothDeviceInfo>>(bondDeviceList);

        return result;
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
        AndroidBridgeInstance.Call("conntectDevice", remoteDeviceMacAddress);
    }

    public void Disconnect()
    {
        AndroidBridgeInstance.Call("disconnectDevice");
    }

    public void SearchDevices()
    {
        AndroidStringCallback onSearchDeviceCallback = new AndroidStringCallback(deviceInfo =>
        {
            BluetoothDeviceInfo bluetoothDevice = JsonConvert.DeserializeObject<BluetoothDeviceInfo>(deviceInfo);
            OnSearchedDeviceEvent?.Invoke(bluetoothDevice);
        }, ANDROID_STRING_CALLBACK_INTERFACE);

        AndroidStringCallback onSearchResultCallback = new AndroidStringCallback(devceInfoList =>
        {
            List<BluetoothDeviceInfo> bluetoothDeviceList = JsonConvert.DeserializeObject<List<BluetoothDeviceInfo>>(devceInfoList);
            OnSearchFinishEvent?.Invoke(bluetoothDeviceList);
        }, ANDROID_STRING_CALLBACK_INTERFACE);

        AndroidBridgeInstance.Call("setSearchDeviceCallback", onSearchResultCallback, onSearchDeviceCallback);

        AndroidBridgeInstance.Call("searchDevices");
    }

    public void Tick()
    {
        //主线程中处理消息
        while (this.m_recvMessageQueue.Count > 0)
        {
            byte[] recvMessageBuffer = this.m_recvMessageQueue.Dequeue();
            OnReceiveDataEvent?.Invoke(recvMessageBuffer);
        }
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
