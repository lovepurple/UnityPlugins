package com.lovepurple.btccontroller;

/**
 * Unity的回调
 */
public interface UnityCallback {
    /**
     * 发送消息到Unity
     * @param msg
     */
    void sendMessage(String msg);
}
