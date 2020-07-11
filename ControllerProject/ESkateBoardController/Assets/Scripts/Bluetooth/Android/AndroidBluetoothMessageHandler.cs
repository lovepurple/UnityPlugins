using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

public class AndroidBluetoothMessageHandler : AndroidStringCallback
{
    private Queue<UnityAndroidMessageAdapter> m_recvMessageQueue = new Queue<UnityAndroidMessageAdapter>();

    public AndroidBluetoothMessageHandler() : base(AndroidBluetoothClassicDevice.ANDROID_STRING_CALLBACK_INTERFACE)
    {
    }

    public override void sendMessage(string msg)
    {
        UnityAndroidMessageAdapter messageFromAndroid = JsonConvert.DeserializeObject<UnityAndroidMessageAdapter>(msg);
        this.m_recvMessageQueue.Enqueue(messageFromAndroid);
    }

    public void Tick()
    {
        while (this.m_recvMessageQueue.Count > 0)
        {
            UnityAndroidMessageAdapter messageFromAndroid = this.m_recvMessageQueue.Dequeue();
            HandlerReceivedMessage(messageFromAndroid);
        }
    }

    private void HandlerReceivedMessage(UnityAndroidMessageAdapter messageFromAndroid)
    {
        string messageBody = Convert.ToString(messageFromAndroid.mMessageBody);
        switch ((UnityAndroidMessageDefine)messageFromAndroid.mMessageID)
        {
            case UnityAndroidMessageDefine.BLUETOOTH_STATE_CHANGED:
                BluetoothEvents.OnBluetoothDeviceStateChangedEvent.SafeInvoke(Convert.ToInt32(messageBody));
                break;
            case UnityAndroidMessageDefine.SEARCHED_DEVICE:
                BluetoothEvents.OnSearchedDeviceEvent.SafeInvoke(JsonConvert.DeserializeObject<BluetoothDeviceInfo>(messageBody));
                break;
            case UnityAndroidMessageDefine.SEARCHED_DEVICE_FINISH:
                BluetoothEvents.OnSearchFinishEvent.SafeInvoke(JsonConvert.DeserializeObject<List<BluetoothDeviceInfo>>(messageBody));
                break;
            case UnityAndroidMessageDefine.SEND_ERROR:
                BluetoothEvents.OnErrorEvent.SafeInvoke(messageBody);
                break;
            case UnityAndroidMessageDefine.SEND_LOG:
                BluetoothEvents.OnLogEvent.SafeInvoke(messageBody);
                break;
            case UnityAndroidMessageDefine.SEND_MESSAGE_BUFFER:
                BluetoothEvents.OnReceiveDataEvent.SafeInvoke(Encoding.ASCII.GetBytes(messageBody));
                break;
            case UnityAndroidMessageDefine.VOLUME_KEY_PRESSED:
                BluetoothEvents.OnVolumeKeyEvent.SafeInvoke(Convert.ToInt32(messageBody));
                break;
            default:
                break;
        }
    }


    public enum UnityAndroidMessageDefine
    {
        SEND_LOG = 0,                       //发送LOG
        SEND_ERROR = 1,                     //发送ERROR
        BLUETOOTH_STATE_CHANGED = 2,        //蓝牙状态改变
        SEARCHED_DEVICE = 3,                //蓝牙搜索到设备
        SEARCHED_DEVICE_FINISH,             //蓝牙搜索结束
        SEND_MESSAGE_BUFFER,                //发送Buffer到Unity
        VOLUME_KEY_PRESSED,                 //音量键事件
    }

    private class UnityAndroidMessageAdapter
    {
        public int mMessageID;
        public object mMessageBody;
    }
}


