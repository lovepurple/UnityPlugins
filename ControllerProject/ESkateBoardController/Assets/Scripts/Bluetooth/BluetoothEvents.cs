using EngineCore;
using System;
using System.Collections.Generic;

/// <summary>
/// 蓝牙消息事件
/// </summary>
public static class BluetoothEvents
{
    /// <summary>
    /// 收到数据事件
    /// </summary>
    public static SafeAction<byte[]> OnReceiveDataEvent;

    /// <summary>
    /// 出错
    /// </summary>
    public static SafeAction<string> OnErrorEvent;

    /// <summary>
    /// Log
    /// </summary>
    public static SafeAction<string> OnLogEvent;

    /// <summary>
    /// 连接成功事件
    /// </summary>
    public static SafeAction OnConnectedEvent;

    /// <summary>
    /// 蓝牙
    /// </summary>
    public static SafeAction<int> OnBluetoothDeviceStateChangedEvent;

    /// <summary>
    /// 发现设备
    /// </summary>
    public static SafeAction<BluetoothDeviceInfo> OnSearchedDeviceEvent;

    /// <summary>
    /// 扫描结束事件
    /// </summary>
    public static SafeAction<List<BluetoothDeviceInfo>> OnSearchFinishEvent;
}
