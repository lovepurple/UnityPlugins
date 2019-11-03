package com.lovepurple.bluetoothcommom;

/**
 * 蓝牙管理器抽象接口
 * <p>
 * 抽象公共部分
 */
public interface IBluetoothManager {

    /**
     * 初始化蓝牙管理器
     */
    void initializeBluetoothManager();

    /**
     * 蓝牙设备是否开启
     *
     * @return
     */
    boolean isEnabled();

    /**
     * 蓝牙是否支持
     *
     * @return
     */
    boolean isSupported();

    /**
     * 获取已配对过的设备列表
     *
     * @return
     */
    String getPariedDevices();

    /**
     * 连接到远程蓝牙设备
     *
     * @param dstAddress
     */
    boolean connectDevice(final String dstAddress);

    /**
     * 断开连接
     */
    void disconnect();

    /**
     * 搜索设备
     */
    void searchDevices();

    /**
     * 发送数据
     *
     * @param bufferData
     */
    void sendData(byte[] bufferData);

    /**
     * log
     *
     * @param logType    0:Log 1:Error
     * @param logMessage
     */
    void log(int logType, String logMessage);

}


