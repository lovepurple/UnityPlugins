/**
 *  Android 经典蓝牙2。0通讯
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class AndroidBluetoothClassicDevice : IBluetoothDevice
{
    //Android 输出的jar的BundleName
    private const string NATIVE_PLUGIN_BUNDLE_NAME = "com.lovepurple.btccontroller";

    public const string ANDROID_STRING_CALLBACK_INTERFACE = "com.lovepurple.bluetoothcommom.UnityStringCallback";
    private const string ANDROID_BYTES_CALLBACK_INTERFACE = "com.lovepurple.btccontroller.UnityBufferCallback";
    private const string ANDROID_INT_CALLBACK_INTERFACE = "com.lovepurple.btccontroller.UnityIntCallback";

    //Unity Native Bridge
    private static AndroidJavaObject m_androidBridgeInstance = null;

    private AndroidBluetoothMessageHandler m_internalBluetoothMessageHandler = null;

    public bool IsBluetoothEnabled()
    {
        return AndroidBridgeInstance.Call<bool>("isEnabled");
    }

    public void InitializeBluetoothDevice()
    {
        m_internalBluetoothMessageHandler = new AndroidBluetoothMessageHandler();

        AndroidBridgeInstance.Call("initialBTCManager", m_internalBluetoothMessageHandler);
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

    public void SendData(List<byte> sendBuffer)
    {
        AndroidBridgeInstance.Call("sendMessage", sendBuffer.ToArray());
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
        AndroidBridgeInstance.Call("searchDevices");
    }

    public void Tick()
    {
        //主线程中处理消息
        if (this.m_internalBluetoothMessageHandler != null)
            this.m_internalBluetoothMessageHandler.Tick();
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