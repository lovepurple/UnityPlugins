using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidBluetoothDevice : IBluetoothDevice
{
    //Android 输出的jar的BundleName
    private const string NATIVE_PLUGIN_BUNDLE_NAME = "com.gmurru.bleframework.BleFramework";



    //建立代理类
    private static AndroidJavaObject m_androidBridgeInstance = null;
    private static AndroidJavaObject AndroidBridgeInstance
    {
        get
        {
            if (m_androidBridgeInstance == null)
            {


            }

            return m_androidBridgeInstance;
        }
    }

    public void InitializeBluetoothDevice()
    {
        throw new System.NotImplementedException();
    }
}
