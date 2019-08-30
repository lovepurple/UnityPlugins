using System;
using System.Collections.Generic;

public interface IBluetoothEvents
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

    /// <summary>
    /// 蓝牙
    /// </summary>
    event Action<int> OnBluetoothDeviceStateChangedEvent;

    /// <summary>
    /// 发现设备
    /// </summary>
    event Action<BluetoothDeviceInfo> OnSearchedDeviceEvent;

    /// <summary>
    /// 扫描结束事件
    /// </summary>
    event Action<List<BluetoothDeviceInfo>> OnSearchFinishEvent;
}
