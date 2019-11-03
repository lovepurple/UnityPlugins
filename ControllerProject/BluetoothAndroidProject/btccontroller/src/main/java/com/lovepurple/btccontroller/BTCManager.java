package com.lovepurple.btccontroller;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;

import com.google.gson.Gson;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
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
    private SendRunable _sendThread = null;

    //系统广播接受
    private BroadcastReceiver _systemBroadcastReceiver;

    //Unity的事件回调
    private UnityStringCallback OnSendMessageToUnityHandler;         //发消息到Unity

    //扫描到的设备列表
    private HashMap<String, BluetoothDeviceInfo> _searchedRemoteDeviceMap = new HashMap<>();

    //当前蓝牙状态
    private BluetoothStatus _deviceCurrentStatus = BluetoothStatus.FREE;

    private BTCManager(Context context) {
        this._applicationContext = context.getApplicationContext();         //获取应用的上下文，生命周期是整个应用
        this._bluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
    }


    /**
     * 初始化蓝牙管理器
     *
     * @param onSendMessageToUnityCallback 出错回调，给Unity使用
     */
    public void initialBTCManager(UnityStringCallback onSendMessageToUnityCallback) {
        this.OnSendMessageToUnityHandler = onSendMessageToUnityCallback;

        this._systemBroadcastReceiver = new BluetoothReceiver();

        //注册IntentFilter
        //Android坑爹 ACTION_CONNECTION)STATE_CHANGE 不好用
        IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_ACL_CONNECTED);
        filter.addAction(BluetoothDevice.ACTION_ACL_DISCONNECTED);
//        filter.addAction(BluetoothDevice.ACTION_ACL_DISCONNECT_REQUESTED);        //远程设备请求断开，不需要

        _applicationContext.registerReceiver(_systemBroadcastReceiver, filter);      //Receiver里可进入的事件
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
     */
    public String getPariedDevices() {

        Gson gson = new Gson();

        ArrayList<BluetoothDeviceInfo> deviceInfos = new ArrayList<>();

        for (BluetoothDevice bluetoothDevice : _bluetoothAdapter.getBondedDevices()) {
            String address = bluetoothDevice.getAddress();
            String bluetoothDeviceName = bluetoothDevice.getName();

            BluetoothDeviceInfo deviceInfo = new BluetoothDeviceInfo();
            deviceInfo.deviceAddress = address;
            deviceInfo.deviceName = bluetoothDeviceName;
            deviceInfo.deviceBondState = 1;

            deviceInfos.add(deviceInfo);
        }

        return gson.toJson(deviceInfos);
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

    private void sendMessageToUnity(String jsonMessage) {
        if (OnSendMessageToUnityHandler != null)
            OnSendMessageToUnityHandler.sendMessage(jsonMessage);
    }

    /**
     * 发送Log到Unity
     *
     * @param logType    0:log  1:Error
     * @param logContent
     */
    private void sendLogToUnity(int logType, String logContent) {
        switch (logType) {
            case UnityMessageDefine.SEND_LOG:
            case UnityMessageDefine.SEND_ERROR:
                UnityMessageAdapter messageAdapter = new UnityMessageAdapter();
                messageAdapter.mMessageID = logType;
                messageAdapter.mMessageBody = logContent;

                Gson gson = new Gson();
                sendMessageToUnity(gson.toJson(messageAdapter));
                break;
        }

    }


    /**
     * 连接到设备
     *
     * @param dstAddress
     */
    public void conntectDevice(String dstAddress) {
        try {

            if (!_bluetoothAdapter.isEnabled()) {
                sendLogToUnity(UnityMessageDefine.SEND_ERROR, "local bluetooth not enabled");
                return;
            }
            if (_deviceCurrentStatus != BluetoothStatus.CONNECTED) {

                if (!BluetoothAdapter.checkBluetoothAddress(dstAddress)) {
                    sendLogToUnity(UnityMessageDefine.SEND_ERROR, "mac address is not correct! make sure it's upper case!");
                    return;
                }

                ConnectDeviceRunnable connectThread = new ConnectDeviceRunnable(dstAddress);
                _executorSerivicePool.submit(connectThread);
            } else {
                sendLogToUnity(UnityMessageDefine.SEND_ERROR, "device is Connected");
            }

        } catch (Exception e) {
            sendLogToUnity(UnityMessageDefine.SEND_ERROR, e.getMessage());
        }
    }

    /**
     * 断开当前连接设备
     */
    public void disconnectDevice() {
        try {
            if (checkDeviceAvailable()) {

                if (_bluetoothSocket != null && _bluetoothSocket.isConnected()) {
                    _bluetoothSocket.close();
                    _bluetoothSocket = null;

                    if (_receiveThread != null) {
                        _receiveThread.KillRecv();
                        _receiveThread = null;
                    }

                    if (_sendThread != null) {
                        _sendThread.killSend();
                        _sendThread = null;
                    }
                }
            }
        } catch (Exception e) {
            sendLogToUnity(UnityMessageDefine.SEND_ERROR, e.getMessage());
        }
    }

    /**
     * 发送数据
     *
     * @param messageBuffer
     */
    public void sendMessage(byte[] messageBuffer) {
        if (checkDeviceAvailable())
            _sendQueue.add(messageBuffer);
    }

    private boolean checkDeviceAvailable() {
        if (_deviceCurrentStatus != BluetoothStatus.CONNECTED) {
            sendLogToUnity(UnityMessageDefine.SEND_ERROR, "Bluetooth Device is disconnect");
            return false;
        }

        return true;
    }

    /**
     * 搜索设备
     *
     * @param
     */
    public void searchDevices() {
        if (checkDeviceAvailable()) {
            this._deviceCurrentStatus = BluetoothStatus.DISCOVERING;
            IntentFilter filter = new IntentFilter();
            //扫描时，需要定位权限，就算写在Manifest里 有可能也没开，需要判断
            filter.addAction(BluetoothDevice.ACTION_FOUND);

            filter.addAction(BluetoothAdapter.ACTION_DISCOVERY_FINISHED);
            filter.addAction(BluetoothAdapter.ACTION_DISCOVERY_STARTED);
            _applicationContext.registerReceiver(mDiscoveryReceiver, filter);

            if (_bluetoothAdapter.isDiscovering())
                _bluetoothAdapter.cancelDiscovery();

            _bluetoothAdapter.startDiscovery();
        }
    }

    private BroadcastReceiver mDiscoveryReceiver = new BroadcastReceiver() {

        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();

            //开始扫描
            if (BluetoothAdapter.ACTION_DISCOVERY_STARTED.equals(action)) {
            } else if (BluetoothDevice.ACTION_FOUND.equals(action)) {
                //获取扫描出的设备
                BluetoothDevice remoteDevice = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);

                if (_searchedRemoteDeviceMap.containsKey(remoteDevice.getAddress()))
                    return;

                BluetoothDeviceInfo bluetoothDeviceInfo = new BluetoothDeviceInfo();
                bluetoothDeviceInfo.deviceName = remoteDevice.getName();
                bluetoothDeviceInfo.deviceAddress = remoteDevice.getAddress();

                //新设备
                if (remoteDevice.getBondState() == BluetoothDevice.BOND_NONE) {
                    bluetoothDeviceInfo.deviceBondState = 2;
                } else if (remoteDevice.getBondState() == BluetoothDevice.BOND_BONDED) {
                    bluetoothDeviceInfo.deviceBondState = 1;
                } else
                    return;

                _searchedRemoteDeviceMap.put(bluetoothDeviceInfo.deviceAddress, bluetoothDeviceInfo);

                UnityMessageAdapter searchDeviceMessage = new UnityMessageAdapter();
                searchDeviceMessage.mMessageID = UnityMessageDefine.SEARCHED_DEVICE;
                searchDeviceMessage.mMessageBody = bluetoothDeviceInfo;
                String strSearchDeviceMessage = new Gson().toJson(searchDeviceMessage);
                sendMessageToUnity(strSearchDeviceMessage);

            } else if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equals(action)) {
                Gson gson = new Gson();


                //扫描结束，关闭
                _bluetoothAdapter.cancelDiscovery();

                UnityMessageAdapter searchDeviceMessageFinish = new UnityMessageAdapter();
                searchDeviceMessageFinish.mMessageID = UnityMessageDefine.SEARCHED_DEVICE_FINISH;
                searchDeviceMessageFinish.mMessageBody = _searchedRemoteDeviceMap.values().toArray();
                String searchResult = gson.toJson(searchDeviceMessageFinish);
                sendMessageToUnity(searchResult);

                _applicationContext.unregisterReceiver(mDiscoveryReceiver);
            }
        }
    };

    /**
     * 接受系统广播
     */
    private class BluetoothReceiver extends BroadcastReceiver {

        //所有指定Context的广播都从这里下发,这里会进入的intent是上面Register的
        @Override
        public void onReceive(Context context, Intent intent) {

            String actionType = intent.getAction();

            //通过intent.getIntExtra获取状态
            BluetoothDevice device = intent.getParcelableExtra(BluetoothAdapter.EXTRA_STATE);

            //蓝牙状态改变
            if (BluetoothDevice.ACTION_ACL_CONNECTED.equals(actionType)) {
                _deviceCurrentStatus = BluetoothStatus.CONNECTED;
            } else if (BluetoothDevice.ACTION_ACL_DISCONNECTED.equals((actionType))) {
                _deviceCurrentStatus = BluetoothStatus.FREE;
            }

            UnityMessageAdapter onDeviceStateChangedMessage = new UnityMessageAdapter();
            onDeviceStateChangedMessage.mMessageID = UnityMessageDefine.BLUETOOTH_STATE_CHANGED;
            onDeviceStateChangedMessage.mMessageBody = _deviceCurrentStatus.ordinal();
            sendMessageToUnity(new Gson().toJson(onDeviceStateChangedMessage));
        }
    }


    /**
     * 获取当前设备状态
     *
     * @return
     */
    public int getCurrentBluetoothStatus() {
        return this._deviceCurrentStatus.ordinal();
    }


    private void OnReveiveBuffer(byte[] recvBuffer) {
        UnityMessageAdapter message = new UnityMessageAdapter();
        message.mMessageID = UnityMessageDefine.SEND_MESSAGE_BUFFER;
        message.mMessageBody = new String(recvBuffer);

        sendMessageToUnity(new Gson().toJson(message));
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
                    sendLogToUnity(UnityMessageDefine.SEND_ERROR, remoteDevice.getAddress() + " device is null");

                //取消当前设备的搜索
                _bluetoothAdapter.cancelDiscovery();
                _deviceCurrentStatus = BluetoothStatus.FREE;

                _bluetoothSocket = remoteDevice.createInsecureRfcommSocketToServiceRecord(UUID.fromString(STR_UUID));
                _bluetoothSocket.connect();

                _bluetoothRecvStream = _bluetoothSocket.getInputStream();
                _bluetoothSendStream = _bluetoothSocket.getOutputStream();

                if (_receiveThread != null)
                    _receiveThread.KillRecv();

                if (_sendThread != null)
                    _sendThread.killSend();

                _receiveThread = new RecvRunable();
                _executorSerivicePool.submit(_receiveThread);

                _sendThread = new SendRunable();
                _executorSerivicePool.submit(_sendThread);
            } catch (Exception e) {
                sendLogToUnity(UnityMessageDefine.SEND_ERROR, e.getMessage());
                _deviceCurrentStatus = BluetoothStatus.FREE;
            }
        }
    }

    /**
     * 发送线程
     */
    private class SendRunable implements Runnable {

        private boolean isRunning = true;

        @Override
        public void run() {
            while (isRunning) {
                try {
                    int messageCount = _sendQueue.size();
                    //合包发送
                    for (int i = 0; i < _sendQueue.size(); ++i) {
                        byte[] messageBuffer = _sendQueue.poll();
                        _bluetoothSendStream.write(messageBuffer);

                        if (messageBuffer[messageBuffer.length - 1] != '\n')
                            _bluetoothSendStream.write('\n');
                    }

                    if (messageCount > 0)
                        _bluetoothSendStream.flush();

                } catch (IOException e) {
                    sendLogToUnity(UnityMessageDefine.SEND_ERROR, e.getMessage());
                }
            }
        }

        public void killSend() {
            this.isRunning = false;
        }
    }

    /**
     * 接受线程
     */
    private class RecvRunable implements Runnable {
        private boolean isRuning = true;
        private byte[] _recvBuffer = new byte[64];
        private int _recvBufferCount = 0;

        @Override
        public void run() {
            while (isRuning) {
                if (_deviceCurrentStatus != BluetoothStatus.CONNECTED) {
                    sendLogToUnity(UnityMessageDefine.SEND_ERROR, "device not connected");
                    continue;
                }

                try {
                    //蓝牙模块有可能一次发不出所有内容，分两次发出
                    while (_bluetoothRecvStream.available() > 0) {
                        byte recvByte = (byte) _bluetoothRecvStream.read();

                        if (recvByte == '\n') {

                            if (_recvBufferCount > 0) {
                                //会有线程问题，当前线程属于子线程，直接返回到Unity 也是子线程，如果使用
                                byte[] recvMessageBuffer = Arrays.copyOfRange(_recvBuffer, 0, _recvBufferCount);        //[from,to)
                                OnReveiveBuffer(recvMessageBuffer);
                            }

                            _recvBufferCount = 0;
                        } else {
                            _recvBuffer[_recvBufferCount++] = recvByte;
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
