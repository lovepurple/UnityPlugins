using System;
using System.Collections.Generic;

public interface IBluetoothDevice
{
    /// <summary>
    /// 初始化蓝牙设备
    /// </summary>
    void InitializeBluetoothDevice();

    /// <summary>
    /// 蓝牙是否开启
    /// </summary>
    /// <returns></returns>
    bool IsBluetoothEnabled();

    /// <summary>
    /// 获取已配对过的设备列表
    /// </summary>
    /// <returns></returns>
    List<BluetoothDeviceInfo> GetPariedDevices();

    /// <summary>
    /// 获取已连接的蓝牙设备
    /// </summary>
    /// <returns></returns>
    string GetConnectedDeviceName();

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="sendBuffer"></param>
    void SendData(List<byte> sendBuffer);

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

    /// <summary>
    /// 蓝牙扫描
    /// </summary>
    void SearchDevices();

    /// <summary>
    /// Tick
    /// </summary>
    void Tick();
}
