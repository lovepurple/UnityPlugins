package com.lovepurple.btccontroller;

/**
 * 与Unity交互的消息定义
 *
 *  Java中的final static 相当于C#的 const
 */
public final class UnityMessageDefine {
    public final static int SEND_LOG = 0;                     //发送LOG
    public final static int SEND_ERROR = 1;                   //发送ERROR
    public final static int BLUETOOTH_STATE_CHANGED = 2;      //蓝牙状态改变
    public final static int SEARCHED_DEVICE = 3;              //蓝牙搜索到设备
    public final static int SEARCHED_DEVICE_FINISH = 4;       //蓝牙搜索结束
    public final static int SEND_MESSAGE_BUFFER = 5;          //发送Buffer到Unity
}

