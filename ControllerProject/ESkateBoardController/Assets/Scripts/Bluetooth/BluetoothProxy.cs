using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluetoothProxy
{
    private static BluetoothProxy m_instance;

    private IBluetoothDevice m_device;


    private BluetoothProxy() { }

    public void InitializeBluetoothProxy()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                m_device = new AndroidBluetoothDevice();
                break;
            default:
                throw new System.Exception($"Platform{Application.platform.ToString()} BluetoothDevice Not Implememt");

        }
    }




    public static BluetoothProxy Intance => m_instance ?? (m_instance = new BluetoothProxy());  //??初始化的写法
}
