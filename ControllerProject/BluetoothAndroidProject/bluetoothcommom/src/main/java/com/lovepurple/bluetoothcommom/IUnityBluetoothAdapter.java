package com.lovepurple.bluetoothcommom;

/**
 * Unity 蓝牙Apdater
 */
public interface IUnityBluetoothAdapter {
    void initializeBluetoothForUnity(UnityStringCallback sendStringToUnityCallback);

    /**
     * 发送消息到Unity
     * @param strMessage
     */
    void sendMessageToUnity(String strMessage);

    /**
     * Unity 字节流回调
     */
    interface UnityBufferCallback {
        void sendMessageBuffer(byte[] messageBuffer);
    }
}

