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

    /// <summary>
    /// 出错
    /// </summary>
    event Action<string> OnErrorEvent;

    /// <summary>
    /// 连接成功事件
    /// </summary>
    event Action OnConnectedEvent;


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

    /// <summary>
    /// 获取客户端蓝牙状态
    /// </summary>
    /// <returns></returns>
    BluetoothStatus GetBluetoothDeviceStatus();

    /// <summary>
    /// 连接到设备
    /// </summary>
    /// <param name="remoteDeviceMacAddress"></param>
    void ConnectToDevice(string remoteDeviceMacAddress);

    /// <summary>
    /// 断开连接
    /// </summary>
    void Disconnect();
}
