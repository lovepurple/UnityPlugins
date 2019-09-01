using EngineCore;
using System.Linq;

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
            byte[] messageBody = null;
            if (obj.Length > 2)
                messageBody = obj.Skip(1).ToArray();

            MessageHandler.Call((int)messageID, messageBody);
        }
    }
}
