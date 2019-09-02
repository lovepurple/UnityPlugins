package com.lovepurple.btccontroller;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;

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

    //系统广播接受
    private BroadcastReceiver _systemBroadcastReceiver;

    //Unity的事件回调
    private UnityCallback OnErrorCallback;
    private UnityBufferCallback OnReceivedDataCallback;
    private UnityCallback OnSearchedDevice;                         //扫描到设备
    private UnityCallback OnSearchedDevicesCallback;                //扫描结束，扫描到的设备列表
    private UnityIntCallback OnBluetoothStateChangCallback;     //连接状态改变回调

    //扫描到的设备列表
    private HashMap<String, BluetoothDeviceInfo> _searchedRemoteDeviceMap = new HashMap<>();

    //当前蓝牙状态
    private BluetoothStatus _deviceCurrentStatus = BluetoothStatus.FREE;

    //线程通讯，子线程通过handler
    private Handler _mainThreadHandler = null;


    private BTCManager(Context context) {
        this._applicationContext = context.getApplicationContext();         //获取应用的上下文，生命周期是整个应用
        this._bluetoothAdapter = BluetoothAdapter.getDefaultAdapter();

        _mainThreadHandler = new MainThreadMessageHandler(this._applicationContext.getMainLooper());
    }


    /**
     * 初始化蓝牙管理器
     *
     * @param onErrorCallback 出错回调，给Unity使用
     */
    public void initialBTCManager(UnityCallback onErrorCallback, UnityIntCallback onBluetoothStateChangCallback) {
        this.OnErrorCallback = onErrorCallback;
        this.OnBluetoothStateChangCallback = onBluetoothStateChangCallback;

        this._systemBroadcastReceiver = new BluetoothReceiver();

        //注册IntentFilter
        IntentFilter filter = new IntentFilter(BluetoothAdapter.ACTION_CONNECTION_STATE_CHANGED);
        filter.addAction(BluetoothAdapter.ACTION_STATE_CHANGED);

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

    /**
     * 连接到设备
     *
     * @param dstAddress
     */
    public void conntectDevice(String dstAddress) {
        try {

            if (!_bluetoothAdapter.isEnabled()) {
                sendErrorMessage("local bluetooth not enabled");
                return;
            }
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
//            if (_deviceCurrentStatus == BluetoothStatus.CONNECTED) {

                if (_bluetoothSocket != null && _bluetoothSocket.isConnected()) {
                    _bluetoothSocket.close();
                    _bluetoothSocket = null;

                    if (_receiveThread != null) {
                        _receiveThread.KillRecv();
                        _receiveThread = null;
                    }
                    _deviceCurrentStatus = BluetoothStatus.FREE;
                }
//            }
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

//        if (_deviceCurrentStatus != BluetoothStatus.CONNECTED) {
//            sendErrorMessage("Send Error ,Device not Connected");
//            return;
//        }


        _sendQueue.add(messageBuffer);
        SendRunable sendingThread = new SendRunable();
        _executorSerivicePool.submit(sendingThread);
    }

    /**
     * 搜索设备
     *
     * @param
     */
    public void searchDevices() {
        if (this._deviceCurrentStatus == BluetoothStatus.CONNECTED)
            sendErrorMessage("bluetooth is connecting ,please disconnect current device");
        else {
            this._deviceCurrentStatus = BluetoothStatus.DISCOVERING;
            IntentFilter filter = new IntentFilter();
            //扫描时，需要定位权限，就算写在Manifest里 有可能也没开，需要判断
            filter.addAction(BluetoothDevice.ACTION_FOUND);
//            if (_applicationContext.checkSelfPermission((Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED)
//                sendErrorMessage("permission android.permission.ACCESS_COARSE_LOCATION  deny");

            filter.addAction(BluetoothAdapter.ACTION_DISCOVERY_FINISHED);
            filter.addAction(BluetoothAdapter.ACTION_DISCOVERY_STARTED);
            _applicationContext.registerReceiver(mDiscoveryReceiver, filter);

            if (_bluetoothAdapter.isDiscovering())
                _bluetoothAdapter.cancelDiscovery();

            _bluetoothAdapter.startDiscovery();
        }
    }

    public void setSearchDeviceCallback(UnityCallback onSearchFinishCallback, UnityCallback onSearchDeviceCallback) {
        OnSearchedDevice = onSearchDeviceCallback;
        OnSearchedDevicesCallback = onSearchFinishCallback;
    }

    //todo 线程问题后续修改 2019-09-02 15:35:37
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

                if (OnSearchedDevice != null)
                    OnSearchedDevice.sendMessage(new Gson().toJson(bluetoothDeviceInfo));

            } else if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equals(action)) {
                Gson gson = new Gson();
                String searchResult = gson.toJson(_searchedRemoteDeviceMap.values().toArray());

                //扫描结束，关闭
                _bluetoothAdapter.cancelDiscovery();

                if (OnSearchedDevicesCallback != null)
                    OnSearchedDevicesCallback.sendMessage(searchResult);

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
            sendErrorMessage("jin le " + actionType);
            if (BluetoothAdapter.ACTION_CONNECTION_STATE_CHANGED.equals(actionType)) {       //蓝牙状态改变
                //通过intent.getIntExtra获取状态
                int bluetoothState = intent.getIntExtra(BluetoothAdapter.EXTRA_STATE, BluetoothAdapter.ERROR);

                if (bluetoothState == BluetoothAdapter.STATE_CONNECTED)
                    _deviceCurrentStatus = BluetoothStatus.CONNECTED;
                else if (bluetoothState == BluetoothAdapter.STATE_DISCONNECTING)
                    _deviceCurrentStatus = BluetoothStatus.DISCOVERING;
                else
                    _deviceCurrentStatus = BluetoothStatus.FREE;

                if (OnBluetoothStateChangCallback != null)
                    OnBluetoothStateChangCallback.sendMessageInt(bluetoothState);
            }
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
                sendErrorMessageToMainThread(e.getMessage());
            }


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
//                if (_deviceCurrentStatus != BluetoothStatus.CONNECTED || _bluetoothRecvStream == null) {
//                    continue;     //todo:state 需要再整理一下
//                }

                try {
                    //蓝牙模块有可能一次发不出所有内容，分两次发出
                    while (_bluetoothRecvStream.available() > 0) {
                        byte recvByte = (byte) _bluetoothRecvStream.read();

                        if (recvByte == '\n') {

                            if (_recvBufferCount > 0) {
                                //会有线程问题，当前线程属于子线程，直接返回到Unity 也是子线程，如果使用
                                byte[] recvMessageBuffer = Arrays.copyOfRange(_recvBuffer, 0, _recvBufferCount);        //[from,to)
                                Message msg = new Message();
                                msg.arg1 = 1;
                                msg.obj = recvMessageBuffer;
                                _mainThreadHandler.sendMessage(msg);
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

    private void sendErrorMessageToMainThread(String errorMessage) {
        Message msg = new Message();
        msg.arg1 = 0;
        msg.obj = errorMessage;
        _mainThreadHandler.sendMessage(msg);
    }

    /**
     * 主线程消息处理（子线程与主线程通讯）
     */
    private class MainThreadMessageHandler extends Handler {

        public MainThreadMessageHandler(Looper looper) {
            super(looper);
        }

        @Override
        public void handleMessage(Message msg) {
            switch (msg.arg1) {
                case 0:     //出错
                    sendErrorMessage(msg.obj.toString());
                    break;
                case 1:     //接收到新消息
                    OnReveiveBuffer((byte[]) msg.obj);
                    break;
            }

        }

    }
}
