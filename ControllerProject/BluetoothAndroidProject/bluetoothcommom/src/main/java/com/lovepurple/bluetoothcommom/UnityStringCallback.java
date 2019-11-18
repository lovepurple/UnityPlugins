package com.lovepurple.bluetoothcommom;

/**
 * Unity的回调
 */
public interface UnityStringCallback {
    /**
     * 发送消息到Unity
     * @param msg
     */
    void sendMessage(String msg);

    interface UnityIntCallback {

        void sendMessageInt(int val);
    }
}
