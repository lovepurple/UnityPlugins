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

    //Unity Native Bridge
    private static AndroidJavaObject m_androidBridgeInstance = null;


    /// <summary>
    /// 
    /// </summary>
    public bool IsBluetoothEnabled()
    {
        return AndroidBridgeInstance.Call<bool>("isEnabled");
    }

    public void InitializeBluetoothDevice()
    {
        throw new System.NotImplementedException();
    }

    public void GetPariedDevices(Action<string> OnGetPariedDeviceCallback)
    {
        AndroidStringCallback androidStringCallback = new AndroidStringCallback(OnGetPariedDeviceCallback);
        AndroidBridgeInstance.Call("getPariedDevices", androidStringCallback);
    }

    public string GetConnectedDeviceName()
    {
        return AndroidBridgeInstance.Call<string>("getConnectedDeviceName");
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
