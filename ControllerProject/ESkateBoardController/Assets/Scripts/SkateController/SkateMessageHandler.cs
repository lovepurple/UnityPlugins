﻿using EngineCore;
using System.Linq;
using System.Text;

public class SkateMessageHandler
{
    private IBluetoothDevice m_bluetoothDevice;

    public SkateMessageHandler(IBluetoothDevice bluetoothDevice)
    {
        this.m_bluetoothDevice = bluetoothDevice;
        this.m_bluetoothDevice.OnReceiveDataEvent += OnRecvMessageBuffer;
    }

    private void OnRecvMessageBuffer(byte[] obj)
    {
        if (obj.Length > 0)
        {
            MessageDefine messageID = (MessageDefine)obj[0];
            char[] messageBody = null;
            if (obj.Length > 2)
                messageBody = Encoding.ASCII.GetChars(obj.Skip(1).ToArray());

            MessageHandler.Call((int)messageID, messageBody);
        }
    }
}
