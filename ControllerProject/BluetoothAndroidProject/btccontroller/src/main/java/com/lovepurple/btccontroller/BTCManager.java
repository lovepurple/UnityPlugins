package com.lovepurple.btccontroller;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.Context;

import com.google.gson.Gson;

import org.json.JSONObject;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Queue;
import java.util.Set;
import java.util.UUID;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.LinkedTransferQueue;

/**
 * 经典蓝牙控制模块
 */
public class BTCManager {
    private static BTCManager _instance;

    //蓝牙连接用到的UUID
    public static String STR_UUID = "00001101-0000-1000-8000-00805F9B34FB";

    private Context _applicationContext;

    private BluetoothAdapter _bluetoothAdapter;


    private BluetoothSocket _bluetoothSocket;
    private InputStream _bluetoothRecvStream;
    private OutputStream _bluetoothSendStream;
    private ExecutorService _executorSerivicePool = Executors.newCachedThreadPool();        //线程池
    private Queue<byte[]> _sendQueue = new LinkedTransferQueue<>();
    private RecvRunable _receiveThread = null;


    //Unity的事件回调
    private UnityCallback OnGetPariedDevicesCallback;
    private UnityCallback OnErrorCallback;
    private UnityBufferCallback OnReceivedDataCallback;
    private UnityCallback OnSearchedDevicesCallback;
    private UnityCallback OnConnectedCallback;

    private Set<BluetoothDevice> _pairedBluetoothDeviceSet = null;


    //当前蓝牙状态
    private BluetoothStatus _deviceCurrentStatus = BluetoothStatus.FREE;


    private BTCManager(Context context) {
        this._applicationContext = context.getApplicationContext();         //获取应用的上下文，生命周期是整个应用
        this._bluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
    }

    /**
     * 初始化蓝牙管理器
     *
     * @param onErrorCallback 出错回调，给Unity使用
     */
    public void initialBTCManager(UnityCallback onErrorCallback) {
        this.OnErrorCallback = onErrorCallback;
    }

    /**
     * 蓝牙是否开启
     *
     * @return
     */
    public boolean isEnabled() {
        if (this._bluetoothAdapter == null)
            return false;

        return this._bluetoothAdapter.isEnabled();
    }

    /**
     * 获取已配对过的设备列表
     *
     * @param callback
     */
    public void getPariedDevices(UnityCallback callback) {
        this.OnGetPariedDevicesCallback = callback;

        Gson gson = new Gson();

        _pairedBluetoothDeviceSet = _bluetoothAdapter.getBondedDevices();
        ArrayList<BluetoothDeviceInfo> deviceInfos = new ArrayList<>();

        for (BluetoothDevice bluetoothDevice : _pairedBluetoothDeviceSet) {
            JSONObject deviceObject = new JSONObject();
            String address = bluetoothDevice.getAddress();
            String bluetoothDeviceName = bluetoothDevice.getName();

            BluetoothDeviceInfo deviceInfo = new BluetoothDeviceInfo();
            deviceInfo.deviceAddress = address;
            deviceInfo.deviceName = bluetoothDeviceName;

            deviceInfos.add(deviceInfo);
        }

        String resultStr = gson.toJson(deviceInfos);
        this.OnGetPariedDevicesCallback.sendMessage(resultStr);
    }

    /**
     * 获取已连接的设备名称(Classic蓝牙需要通过反射)
     *
     * @return
     */
    public String getConnectedDeviceName() {
        Class<BluetoothAdapter> bluetoothAdapterClass = BluetoothAdapter.class;
        try {//得到蓝牙状态的方法
            Method method = bluetoothAdapterClass.getDeclaredMethod("getConnectionState", (Class[]) null);
            //打开权限
            method.setAccessible(true);
            int state = (int) method.invoke(_bluetoothAdapter, (Object[]) null);

            if (state == BluetoothAdapter.STATE_CONNECTED) {

                Set<BluetoothDevice> devices = _bluetoothAdapter.getBondedDevices();

                for (BluetoothDevice device : devices) {

                    Method isConnectedMethod = BluetoothDevice.class.getDeclaredMethod("isConnected", (Class[]) null);
                    method.setAccessible(true);
                    boolean isConnected = (boolean) isConnectedMethod.invoke(device, (Object[]) null);

                    if (isConnected) {
                        return device.getName();
                    }
                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }

        return "";
    }

    /**
     * 连接到设备
     *
     * @param dstAddress
     */
    public void conntectDevice(String dstAddress, UnityCallback onConnectedCallback) {

        if (onConnectedCallback != null)
            this.OnConnectedCallback = onConnectedCallback;

        try {
            if (_deviceCurrentStatus != BluetoothStatus.CONNECTED) {

                if (!_bluetoothAdapter.checkBluetoothAddress(dstAddress)) {
                    sendErrorMessage("mac address is not correct! make sure it's upper case!");
                    return;
                }

                ConnectDeviceRunnable connectThread = new ConnectDeviceRunnable(dstAddress);
                _executorSerivicePool.submit(connectThread);
            } else {
                sendErrorMessage("device is Connected");
            }

        } catch (Exception e) {
            sendErrorMessage(e.getMessage());
        }
    }

    /**
     * 断开当前连接设备
     */
    public void disconnectDevice() {
        try {
            if (_deviceCurrentStatus == BluetoothStatus.CONNECTED) {

                if (_bluetoothSocket != null && _bluetoothSocket.isConnected()) {
                    _bluetoothSocket.close();
                    _bluetoothSocket = null;

                    if (_receiveThread != null) {
                        _receiveThread.KillRecv();
                        _receiveThread = null;
                    }
                    _deviceCurrentStatus = BluetoothStatus.FREE;
                }
            }
        } catch (Exception e) {
            sendErrorMessage(e.getMessage());
        }
    }

    /**
     * 发送数据
     *
     * @param messageBuffer
     */
    public void sendMessage(byte[] messageBuffer) {

        if (_deviceCurrentStatus != BluetoothStatus.CONNECTED) {
            sendErrorMessage("Send Error ,Device not Connected");
            return;
        }

        _sendQueue.add(messageBuffer);
        SendRunable sendingThread = new SendRunable();
        _executorSerivicePool.submit(sendingThread);
    }

    /**
     * 搜索设备
     *
     * @param onSearchResultCallback
     */
    public void searchDevices(UnityCallback onSearchResultCallback) {
        this.OnSearchedDevicesCallback = onSearchResultCallback;


    }

    /**
     * 获取当前设备状态
     *
     * @return
     */
    public int getCurrentBluetoothStatus() {
        return this._deviceCurrentStatus.ordinal();
    }

    /**
     * 设置
     *
     * @param bufferCallback
     */
    public void setOnReceiveMessageCallback(UnityBufferCallback bufferCallback) {
        this.OnReceivedDataCallback = bufferCallback;
    }

    private void sendErrorMessage(String errorMessage) {
        if (this.OnErrorCallback != null)
            this.OnErrorCallback.sendMessage(errorMessage);
    }

    private void OnReveiveBuffer(byte[] recvBuffer) {
        if (this.OnReceivedDataCallback != null)
            this.OnReceivedDataCallback.sendMessageBuffer(recvBuffer);

    }


    //Android 中的单例写法，需要带上Context
    public static BTCManager getInstance(Context context) {
        if (_instance == null) {
            synchronized (BTCManager.class) {
                _instance = new BTCManager(context);
            }
        }
        return _instance;
    }

    public static BTCManager getInstance() {
        if (_instance != null)
            return _instance;
        else {
            throw new NullPointerException("BTCManager not initial");
        }
    }


    /**
     * 连接线程
     */
    private class ConnectDeviceRunnable implements Runnable {

        private String _remoteDeviceAddress;

        public ConnectDeviceRunnable(String remoteDeviceAddress) {
            this._remoteDeviceAddress = remoteDeviceAddress;
        }

        @Override
        public void run() {
            try {
                BluetoothDevice remoteDevice = _bluetoothAdapter.getRemoteDevice(_remoteDeviceAddress);
                if (remoteDevice == null)
                    sendErrorMessage(_remoteDeviceAddress + " device is null");

                //取消当前设备的搜索
                _bluetoothAdapter.cancelDiscovery();
                _deviceCurrentStatus = BluetoothStatus.FREE;

                _bluetoothSocket = remoteDevice.createInsecureRfcommSocketToServiceRecord(UUID.fromString(STR_UUID));
                _bluetoothSocket.connect();

                _bluetoothRecvStream = _bluetoothSocket.getInputStream();
                _bluetoothSendStream = _bluetoothSocket.getOutputStream();

                if (_receiveThread != null)
                    _receiveThread.KillRecv();

                _receiveThread = new RecvRunable();
                _executorSerivicePool.submit(_receiveThread);

                _deviceCurrentStatus = BluetoothStatus.CONNECTED;
                if (OnConnectedCallback != null)
                    OnConnectedCallback.sendMessage(remoteDevice.getName() + " connect successed");


            } catch (Exception e) {
                sendErrorMessage(e.getMessage());
                _deviceCurrentStatus = BluetoothStatus.FREE;
            }
        }
    }

    /**
     * 发送线程
     */
    private class SendRunable implements Runnable {

        @Override
        public void run() {
            try {
                byte[] sendingBuffer = _sendQueue.poll();
                _bluetoothSendStream.write(sendingBuffer);
                _bluetoothSendStream.write('\n');
                _bluetoothSendStream.flush();
            } catch (IOException e) {
                sendErrorMessage(e.getMessage());
            }


        }
    }

    /**
     * 接受线程
     */
    private class RecvRunable implements Runnable {
        private boolean isRuning = true;

        @Override
        public void run() {
            while (isRuning) {
                if (_deviceCurrentStatus != BluetoothStatus.CONNECTED || _bluetoothRecvStream == null) {
                    continue;
                }

                try {
                    byte[] buffer = new byte[64];
                    int bufferIndex = 0;
                    while (_bluetoothRecvStream.available() > 0) {
                        byte recvByte = (byte) _bluetoothRecvStream.read();

                        if (recvByte == '\n') {
                            byte[] recvMessageBuffer = Arrays.copyOfRange(buffer, 0, bufferIndex);
                            OnReveiveBuffer(recvMessageBuffer);
                            bufferIndex = 0;
                        } else {
                            buffer[bufferIndex++] = recvByte;
                        }
                    }
                } catch (IOException e) {

                }
            }
        }

        public void KillRecv() {
            this.isRuning = false;
        }
    }
}
