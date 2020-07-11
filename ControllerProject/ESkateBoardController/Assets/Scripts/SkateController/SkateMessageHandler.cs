using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SkateMessageHandler
{
    private IBluetoothDevice m_bluetoothDevice;

    public SkateMessageHandler(IBluetoothDevice bluetoothDevice)
    {
        this.m_bluetoothDevice = bluetoothDevice;
        BluetoothEvents.OnReceiveDataEvent += OnRecvMessageBuffer;
    }

    private void OnRecvMessageBuffer(byte[] obj)
    {
        if (obj.Length > 0)
        {
            MessageDefine messageID = (MessageDefine)obj[0];
            char[] messageBody = null;
            if (obj.Length > 1)
                messageBody = Encoding.ASCII.GetChars(obj.Skip(1).ToArray());

            MessageHandler.Call((int)messageID, messageBody);
        }
    }

    public void SetBluetoothDevice(IBluetoothDevice bluetoothDevice)
    {
        this.m_bluetoothDevice = bluetoothDevice;
    }

    public static List<byte> GetSkateMessage(MessageDefine messageName)
    {
        List<byte> messageBufferList = new List<byte>() { (byte)messageName };

        return messageBufferList;
    }
}
