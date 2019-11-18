package com.lovepurple.bluetoothcommom;

/**
 * Android 与 Unity 传递消息Json适配器
 */
public class UnityMessageAdapter {
    public int mMessageID;
    public Object mMessageBody;

    /**
     * Unity的回调
     */
    public static interface UnityStringCallback {
        /**
         * 发送消息到Unity
         * @param msg
         */
        void sendMessage(String msg);
    }
}
