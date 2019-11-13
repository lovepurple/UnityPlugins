using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BluetoothProxy
{
    private static BluetoothProxy m_instance;

    private IBluetoothDevice m_device;

    private List<byte> m_lasttimeSendBufferList = null;
    private BluetoothStatus m_bluetoothStatus = BluetoothStatus.FREE;
    private EBluetoothDeviceType m_bluetoothDeviceType;

    private SkateMessageHandler m_skateMessageHandler = null;

    private BluetoothProxy()
    {
        GlobalEvents.OnBluetoothDeviceChanged += OnBluetoothDeviceTypeChangedCallback;
    }

    public void InitializeBluetoothProxy()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                if (BluetoothDeviceType == EBluetoothDeviceType.BLUETOOTH_CLASSIC)
                    m_device = new AndroidBluetoothClassicDevice();
                else
                    m_device = new AndroidBLEDevice();
                break;
            default:
                throw new System.Exception($"Platform{Application.platform.ToString()} BluetoothDevice Not Implememt");

        }

        if (m_device != null)
        {
            m_device.InitializeBluetoothDevice();

            if (m_skateMessageHandler != null)
                m_skateMessageHandler.SetBluetoothDevice(m_device);
            else
                m_skateMessageHandler = new SkateMessageHandler(m_device);

            BluetoothEvents.OnBluetoothDeviceStateChangedEvent += OnBluetoothStateChangedCallback;
        }
    }

    public void Tick()
    {
        if (m_device != null)
            m_device.Tick();
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="dataBuffer"></param>
    public void SendData(List<byte> dataBuffer)
    {
        if (BluetoothState != BluetoothStatus.CONNECTED)
        {
            Debug.LogError("设备未连接");
            return;
        }

        if (dataBuffer.Last() != '\n')
            dataBuffer.Add(Convert.ToByte('\n'));

        if (m_lasttimeSendBufferList == null)
            m_lasttimeSendBufferList = dataBuffer;

        //上一次可能有\n
        bool isNewDataBuffer = false;
        if (m_lasttimeSendBufferList.Count != dataBuffer.Count)
            isNewDataBuffer = true;
        else
        {
            for (int i = 0; i < dataBuffer.Count; ++i)
            {
                if (dataBuffer[i] != m_lasttimeSendBufferList[i])
                {
                    isNewDataBuffer = true;
                    break;
                }
            }
        }

        if (isNewDataBuffer)
        {
            m_lasttimeSendBufferList = dataBuffer;
            BluetoothDevice.SendData(dataBuffer);
        }
    }

    private void OnBluetoothStateChangedCallback(int bluetoothStatus)
    {
        this.m_bluetoothStatus = (BluetoothStatus)bluetoothStatus;
    }

    private void OnBluetoothDeviceTypeChangedCallback(EBluetoothDeviceType bluetoothDeviceType)
    {
        this.m_bluetoothDeviceType = bluetoothDeviceType;
        if (this.m_device != null)
        {
            this.m_device.Disconnect();
            this.m_device = null;

            BluetoothEvents.OnBluetoothDeviceStateChangedEvent -= OnBluetoothStateChangedCallback;
        }

    }

    public IBluetoothDevice BluetoothDevice => this.m_device;

    public BluetoothStatus BluetoothState => this.m_bluetoothStatus;

    public static BluetoothProxy Intance => m_instance ?? (m_instance = new BluetoothProxy());

    public EBluetoothDeviceType BluetoothDeviceType
    {
        get => m_bluetoothDeviceType;
        set => m_bluetoothDeviceType = value;
    }

    /// <summary>
    /// 蓝牙设备类型
    /// </summary>
    public enum EBluetoothDeviceType
    {
        NONE,
        BLUETOOTH_CLASSIC,
        BLUETOOTH_LOW_ENERGY
    }
}
