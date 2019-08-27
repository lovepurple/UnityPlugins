using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBluetoothDevice
{
    /// <summary>
    /// 收到数据事件
    /// </summary>
    event Action<Byte[]> OnReceiveDataEvent;

    void InitializeBluetoothDevice();

    bool IsBluetoothEnabled();

    void GetPariedDevices(Action<string> callback);

    /// <summary>
    /// 获取已连接的蓝牙设备
    /// </summary>
    /// <returns></returns>
    string GetConnectedDeviceName();

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="sendBuffer"></param>
    void SendData(byte[] sendBuffer);


}
